
#include "pch.h"
#include "framework.h"

#include <sys/types.h>
#include <stdint.h>
#include <stdio.h>
#include <malloc.h>
#include <errno.h>
#include <string.h>
#include <fcntl.h>
#include <stdlib.h>

#if ! defined(WIN32_LEAN_AND_MEAN)
#include <unistd.h>
#include <sys/ioctl.h>
#include <termios.h>
#include <dirent.h>
#include <getopt.h>
#include <error.h>
#else
#define close(p)
#define sleep(p)
#define error(x, y, z)
#endif

#include <iostream>

#include "shutter.h"

//// This is an example of an exported variable
//SHUTTERLIB_API int nShutterLib = 0;

//// This is an example of an exported function.
//SHUTTERLIB_API int fnShutterLib(void)
//{
//	return 0;
//}

//// This is the constructor of a class that has been exported.
//CShutterLib::CShutterLib()
//{
//	return;
//}


bool debugOn = false;

extern "C" void ShutterLibDebugOn()
{
	debugOn = true;
}

extern "C" void ShutterLibDebugOff()
{
	debugOn = false;
}

#define DEBUG_TRACE_ACTIVE true
#if defined( DEBUG_TRACE_ACTIVE )
#define DEBUG_TRACE( msg ) if(debugOn) std::cout << "[" << __FILE__ << "," << __LINE__ << "] " << msg << std::endl
#else
#define DEBUG_TRACE( msg )
#endif

#define BOARD_COUNT 3
// indices 0, 1, 2

// dressing, bathroom, bedroom:
#define BOARD_A 2
// living room:
#define BOARD_B 1
// study, kitchen:
#define BOARD_C 0

const char* devices[] = { "/dev/ttyACM0", "/dev/ttyACM1", "/dev/ttyACM2" };

int* on_map = 0;
int* off_map = 0;
int* toggle_map = 0;
int* cycle_map = 0;
int* status_map = 0;

int query_firmware = 0;
int debug = 0;
int simulate = 0;

char* window = 0;
char* direction = 0;

int open_device(const char* devname)
{
	DEBUG_TRACE( "-> open_device: " << devname );
	int fd = 0;
#if ! defined(WIN32_LEAN_AND_MEAN)
	struct termios ios;

	fd = open(devname, O_RDWR | O_NOCTTY);
	if (fd < 0)
		error(1, errno, "open(%s)", devname);

	/* Switch to 19200, raw mode. */
	tcgetattr(fd, &ios);
	cfsetispeed(&ios, B19200);
	cfsetospeed(&ios, B19200);
	ios.c_iflag &= ~(BRKINT | ICRNL | INPCK | ISTRIP | IXON);
	ios.c_oflag &= ~OPOST;
	ios.c_cflag &= ~(CSTOPB | PARENB | PARODD | CSIZE);
	ios.c_cflag |= (CS8 | CLOCAL | CREAD);
	ios.c_lflag &= ~(ECHO | ICANON | IEXTEN | ISIG);
	ios.c_cc[VMIN] = 5; ios.c_cc[VTIME] = 8;
	ios.c_cc[VMIN] = 0; ios.c_cc[VTIME] = 0;
	ios.c_cc[VMIN] = 2; ios.c_cc[VTIME] = 0;
	ios.c_cc[VMIN] = 0; ios.c_cc[VTIME] = 8;
	tcsetattr(fd ,TCSAFLUSH, &ios);
#endif
	DEBUG_TRACE( "<- open_device" );
	return fd;
}

#define PACKET_STX   0x04
#define PACKET_ETX   0x0f
#define GET_STATUS   0x18
#define TURN_ON      0x11
#define TURN_OFF     0x12
#define TOGGLE       0x14
#define GET_VERSION  0x71

#define for_each_relay(n, map)			\
	for (n = 0; n < 8; n++)			\
		if ((map) & (1 << n))

struct k8090_packet {
	struct k8090_payload {
		uint8_t stx;
		uint8_t command;
		uint8_t mask;
		uint8_t param1;
		uint8_t param2;
	} payload;
	uint8_t checksum;
	uint8_t etx;
} 
#if ! defined(WIN32_LEAN_AND_MEAN)
__attribute__((packed))
#endif
;

uint8_t do_checksum(struct k8090_packet::k8090_payload *p)
{
	uint8_t checksum, *r;

	for (checksum = 0, r = (uint8_t *)p; r < (uint8_t *)(p + 1); r++)
		checksum += *r;

	return ~checksum + 1;
}

void dump_packet(const char *dir, struct k8090_packet *pkt)
{
	printf("%s { %.2x, %.2x, %.2x, %.2x, %.2x, %.2x, %.2x }\n",
	       dir,
	       pkt->payload.stx, 
	       pkt->payload.command,
	       pkt->payload.mask,
	       pkt->payload.param1,
	       pkt->payload.param2,
	       pkt->checksum,
	       pkt->etx);
}

void write_device(int fd, void* p)
{
  struct k8090_packet *pkt = (struct k8090_packet*)p;

  DEBUG_TRACE( "-> write_device: " << fd );
  if( simulate == 0 )
  {
	int ret = 0;

	pkt->payload.stx = PACKET_STX;
	pkt->checksum = do_checksum(&pkt->payload);
	pkt->etx = PACKET_ETX;

	if (debug)
		dump_packet("=>", pkt);

#if ! defined(WIN32_LEAN_AND_MEAN)
	ret = write(fd, pkt, sizeof(*pkt));
#endif
	if (ret < 0)
	{
		error(1, errno, "write");
	}
  }
  else
  {
	std::cout << "Simulation mode: nothing written to device" << std::endl;
  }
  DEBUG_TRACE( "<- write_device" );
}

void read_device(int fd, void* p)
{
  struct k8090_packet *pkt = (struct k8090_packet*)p;
  DEBUG_TRACE( "-> read_device: " << fd );
  int ret = 0;
#if ! defined(WIN32_LEAN_AND_MEAN)
  ret = read(fd, pkt, sizeof(*pkt));
#endif
  if (ret < 0)
  {
	  error(1, errno, "read");
  }

  if (debug)
	dump_packet("<=", pkt);

  if (do_checksum(&pkt->payload) != pkt->checksum)
  {
	  error(1, EINVAL, "bad checksum on read");
  }
  DEBUG_TRACE( "<- read_device" );
}

void init_packet(void* p, int command)
{
  DEBUG_TRACE( "-> init_packet" );
  struct k8090_packet *pkt = (struct k8090_packet*)p;
  memset(pkt, 0, sizeof(*pkt));
  pkt->payload.command = command;
  DEBUG_TRACE( "<- init_packet" );
}

void power_on(int fd, int m)
{
	DEBUG_TRACE( "-> power_on" );
	struct k8090_packet pkt;

	init_packet(&pkt, TURN_ON);
	pkt.payload.mask = m;
	write_device(fd, &pkt);
	DEBUG_TRACE( "<- power_on" );
}

void power_off(int fd, int m)
{
	DEBUG_TRACE( "-> power_off" );
	struct k8090_packet pkt;

	init_packet(&pkt, TURN_OFF);
	pkt.payload.mask = m;
	write_device(fd, &pkt);
	DEBUG_TRACE( "<- power_off" );
}

void power_toggle(int fd, int m)
{
	DEBUG_TRACE( "-> power_toggle" );
	struct k8090_packet pkt;

	init_packet(&pkt, TOGGLE);
	pkt.payload.mask = m;
	write_device(fd, &pkt);
	DEBUG_TRACE( "<- power_toggle" );
}

void MySleep( int sleepSecs )
{
  	DEBUG_TRACE( "Sleeping for " << sleepSecs << " seconds..." );
  	std::cout << sleepSecs << ": " << std::flush;
  	for( int i = 0; i < sleepSecs; i++ )
  	{
    		if( i > 0 )
			std::cout << ", ";
    		std::cout << (i+1) << std::flush;
    		sleep( 1 );
  	}
  	std::cout << std::endl;
}

void power_status(int fd, int m)
{
	DEBUG_TRACE( "-> power_status" );
	struct k8090_packet pkt;
	int n = 0;

	init_packet(&pkt, GET_STATUS);
	write_device(fd, &pkt);
	read_device(fd, &pkt);

	for_each_relay(n, m) 
	{
		printf("%-8s => %s%s\n", "?",
		       (pkt.payload.param1 & (1 << n)) ? "ON" : "OFF",
		       (pkt.payload.param2 & (1 << n)) ? " (TIMED)" : "");
	}
	DEBUG_TRACE( "<- power_status" );
}

void firmware_version(int fd)
{
	DEBUG_TRACE( "-> firmware_version" );
	struct k8090_packet pkt;

	init_packet(&pkt, GET_VERSION);
	write_device(fd, &pkt);
	read_device(fd, &pkt);

	printf("%u.%d\n", pkt.payload.param1 - 16 + 2010, pkt.payload.param2);
	DEBUG_TRACE( "<- firmware_version" );
}


void ShutterLibAllOff()
{
	DEBUG_TRACE( "-> ShutterLibAllOff" );
	for( int board = 0; board < BOARD_COUNT; board++ )
	{
		int fd = open_device(devices[board]);
		off_map[board] = 0;
		for( int relay = 1; relay <= 8; relay++ )
		{
			off_map[board] |= (1 << (relay - 1));
			power_off(fd, off_map[board]);
		}
		off_map[board] = 0;
		close(fd);
	}
	DEBUG_TRACE( "<- ShutterLibAllOff" );
}

void ShutterLibOn( int board, int relay, int onSecs )
{
	DEBUG_TRACE( "-> ShutterLibOn( board: " << board << ", relay: " << relay << ", secs: " << onSecs << " )" );
	ShutterLibAllOff();
	int fd = open_device(devices[board]);
	on_map[board] |= (1 << (relay - 1));
	power_on(fd, on_map[board]);
	on_map[board] = 0;
	MySleep( onSecs );
	off_map[board] |= (1 << (relay - 1));
	power_off(fd, off_map[board]);
	off_map[board] = 0;
	close(fd);
	ShutterLibAllOff();
	DEBUG_TRACE( "<- ShutterLibOn" );
}

void ShutterLibOnMany( int board, int switches[], int switchesCount, int s )
{
	ShutterLibAllOff();
	int fd = open_device(devices[board]);
	for( int i = 0; i < switchesCount; i++ )
	{
		on_map[board] |= (1 << (switches[i] - 1));
	}
	power_on(fd, on_map[board]);
	on_map[board] = 0;
	MySleep( s );
	for( int i = 0; i < switchesCount; i++ )
	{
		off_map[board] |= (1 << (switches[i] - 1));
	}
	power_off(fd, off_map[board]);
	off_map[board] = 0;
	close(fd);
} 

extern "C" void ShutterLibShutdown()
{
	DEBUG_TRACE( "-> ShutterLibShutdown" );
	free( window );
	free( direction );
	free( on_map );
	free( off_map );
	free( toggle_map );
	free( cycle_map );
	free( status_map );
	DEBUG_TRACE( "<- ShutterLibShutdown" );
}


extern "C" void ShutterLibInitialize()
{
	DEBUG_TRACE( "-> ShutterLibInitialize" );
    window = _strdup( "unknown" );
    direction = _strdup( "unknown" );

    on_map = (int*)realloc( on_map, BOARD_COUNT * sizeof( int ) );
    off_map = (int*)realloc( off_map, BOARD_COUNT * sizeof( int ) );
    toggle_map = (int*)realloc( toggle_map, BOARD_COUNT * sizeof( int ) );
    cycle_map = (int*)realloc( cycle_map, BOARD_COUNT * sizeof( int ) );
    status_map = (int*)realloc( status_map, BOARD_COUNT * sizeof( int ) );
	DEBUG_TRACE( "<- ShutterLibInitialize" );
}

