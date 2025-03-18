#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <stdint.h>
#include <malloc.h>
#include <errno.h>
#include <string.h>
#include <fcntl.h>

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
#define strdup _strdup
#endif

#include <vector>
#include <iostream>

// We need MQTT client
#include "Network/Clients/MQTT.hpp"
// We need URL parsing too
#include "Network/Address.hpp"
// We need command line parsing too to avoid NIH
#include "Platform/Arguments.hpp"
#include "Logger/Logger.hpp"

typedef Strings::FastString String;

// dressing, bathroom, bedroom:
#define BOARD_A 2
// living room:
#define BOARD_B 1
// study, kitchen:
#define BOARD_C 0

/*
dressing up BOARD_A 1
dressing down BOARD_A 2
bedroomside up BOARD_A 3
bedroomside down BOARD_A 4
bathroom up BOARD_A 5
bathroom down BOARD_A 6
bedroomgarden up BOARD_A 7
bedroomgarden down BOARD_A 8

livingroomfront up BOARD_B 1
livingroomfront down BOARD_B 2
livingroomsidefront up BOARD_B 3
livingroomsidefront down BOARD_B 4
livingroomsidegarden up BOARD_B 5
livingroomsidegarden down BOARD_B 6
livingroomgarden up BOARD_B 8
livingroomgarden down BOARD_B 7

kitchenside up BOARD_C 1
kitchenside down BOARD_C 2
studyside up BOARD_C 3
studyside down BOARD_C 4
studygarden up BOARD_C 5
studygarden down BOARD_C 6
kitchenfront up BOARD_C 7
kitchenfront down BOARD_C 8
*/

bool debugOn = false;

void ShutterLibDebugOn()
{
	debugOn = true;
}

void ShutterLibDebugOff()
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

void ShutterLibShutdown()
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


void ShutterLibInitialize()
{
	DEBUG_TRACE( "-> ShutterLibInitialize" );
    window = strdup( "unknown" );
    direction = strdup( "unknown" );

    on_map = (int*)realloc( on_map, BOARD_COUNT * sizeof( int ) );
    off_map = (int*)realloc( off_map, BOARD_COUNT * sizeof( int ) );
    toggle_map = (int*)realloc( toggle_map, BOARD_COUNT * sizeof( int ) );
    cycle_map = (int*)realloc( cycle_map, BOARD_COUNT * sizeof( int ) );
    status_map = (int*)realloc( status_map, BOARD_COUNT * sizeof( int ) );
	DEBUG_TRACE( "<- ShutterLibInitialize" );
}

std::vector<std::string> split(std::string& s, const std::string& delimiter) 
{
    std::vector<std::string> tokens;
    size_t pos = 0;
    std::string token;
    while ((pos = s.find(delimiter)) != std::string::npos) 
    {
        token = s.substr(0, pos);
        tokens.push_back(token);
        s.erase(0, pos + delimiter.length());
    }
    tokens.push_back(s);

    return tokens;
}

struct InitLogger 
{
    InitLogger(bool withDump) 
    { 
        const unsigned int logMask = ::Logger::Creation|::Logger::Error|::Logger::Network|::Logger::Connection|::Logger::Content|::Logger::Deletion|(withDump ? ::Logger::Dump : 0);
        ::Logger::setDefaultSink(new ::Logger::DebugConsoleSink(logMask)); 
    }
};

struct MessageReceiver : public Network::Client::MessageReceived
{
    void messageReceived(const Network::Client::MQTTv5::DynamicStringView & topic, const Network::Client::MQTTv5::DynamicBinDataView & payload,
                         const uint16 packetIdentifier, const Network::Client::MQTTv5::PropertiesView & properties)
    {
        fprintf(stdout, "Msg received: (%04X)\n", packetIdentifier);
        fprintf(stdout, "  Topic: %.*s\n", topic.length, topic.data);
        fprintf(stdout, "  Payload: %.*s\n", payload.length, payload.data);   
        std::string payloadContents(payload.data, payload.data+payload.length);     
        if(!strncmp(topic.data, "Shutters", 8))
        {
            std::vector<std::string> v = split (payloadContents, std::string("/"));

            bool dressing = false;
            bool dressingTargetState = false; // closed                
            bool bedroomSide = false;
            bool bedroomSideTargetState = false; // closed                
            bool bathroom = false;
            bool bathroomTargetState = false;
            bool bedroomGarden = false;
            bool bedroomGardenTargetState = false;
            bool livingRoomFront = false;
            bool livingRoomFrontTargetState = false;
            bool livingRoomSideFront = false;
            bool livingRoomSideFrontTargetState = false;
            bool livingRoomSideGarden = false;
            bool livingRoomSideGardenTargetState = false;
            bool livingRoomGarden = false;
            bool livingRoomGardenTargetState = false;
            bool kitchenFront = false;
            bool kitchenFrontTargetState = false;
            bool kitchenSide = false;
            bool kitchenSideTargetState = false;
            bool studySide = false;
            bool studySideTargetState = false;
            bool studyGarden = false;
            bool studyGardenTargetState = false;

            for (auto i : v) 
            {
                std::cout << i << std::endl;
                bool setDebugState = false;
                std::string window("");                
                if( i == "setdebugstate" )
                {
                      setDebugState = true;
                      window = "";
                }             
                else if( i == "on" )
                {
                      if(setDebugState) ShutterLibDebugOn();
                      setDebugState = false;
                      window = "";
                }
                else if( i ==  "off")
                {
                      if(setDebugState) ShutterLibDebugOff();
                      setDebugState = false;
                      window = "";
                }
                else if( i == "dressing")
                {
                      window = i;
                      setDebugState = false;
                      dressing = true;
                }
                else if( i == "bedroomside")
                {
                      window = i;
                      setDebugState = false;                   
                      bedroomSide = true;
                }
                else if (i ==  "bathroom")
                {
                      window = i;
                      setDebugState = false;                 
                      bathroom = true;
                }
                else if( i == "bedroomgarden")
                {
                      window = i;
                      setDebugState = false;                
                      bedroomGarden = true;
                      }
                else if( i == "livingroomfront")
                {
                      window = i;
                      setDebugState = false;
                      livingRoomFront = true;
                }
                else if( i == "livingroomsidefront")
                {
                      window = i;
                      setDebugState = false;
                      livingRoomSideFront = true;
                }
                else if( i == "livingroomsidegarden")
                {
                      window = i;
                      setDebugState = false;
                      livingRoomSideGarden = true;
                }
                else if( i == "livingroomgarden")
                {
                      window = i;
                      setDebugState = false;
                      livingRoomGarden = true;
                }
                else if( i == "kitchenside")
                {
                      window = i;
                      setDebugState = false;
                      kitchenSide = true;
                }
                else if( i == "kitchenfront")
                {
                      window = i;
                      setDebugState = false;
                      kitchenFront = true;
                }
                else if( i == "studyside")
                {
                      window = i;
                      setDebugState = false;
                      studySide = true;
                }
                else if( i == "studygarden")
                {
                      window = i;
                      setDebugState = false;                   
                      studyGarden = true;
                }
                else if( i == "up")
                {
                      if(window == "dressing" && dressingTargetState == false) dressingTargetState = true;
                      else if(window == "bedroomSide" && bedroomSideTargetState == false) bedroomSideTargetState = true;
                      else if(window == "bathroom" && bathroomTargetState == false) bathroomTargetState = true;
                      else if(window == "bedroomGarden" && bedroomGardenTargetState == false) bedroomGardenTargetState = true;
                      else if(window == "livingRoomFront" && livingRoomFrontTargetState == false) livingRoomFrontTargetState = true;
                      else if(window == "livingRoomSideFront" && livingRoomFrontTargetState == false) livingRoomFrontTargetState = true;
                      else if(window == "livingRoomSideGarden" && livingRoomSideGardenTargetState == false) livingRoomSideGardenTargetState = true;
                      else if(window == "livingRoomGarden" && livingRoomGardenTargetState == false) livingRoomGardenTargetState = true;
                      else if(window == "kitchenFront" && kitchenFrontTargetState == false) kitchenFrontTargetState = true;
                      else if(window == "kitchenSide" && kitchenSideTargetState == false) kitchenSideTargetState = true;
                      else if(window == "studySide" && studySideTargetState == false) studySideTargetState = true;
                      else if(window == "studyGarden" && studyGardenTargetState == false) studyGardenTargetState = true;
                      window = "";
                }
                else if( i == "down")
                {
                      if(window == "dressing" && dressingTargetState == true) dressingTargetState = false;
                      else if(window == "bedroomSide" && bedroomSideTargetState == true) bedroomSideTargetState = false;
                      else if(window == "bathroom" && bathroomTargetState == true) bathroomTargetState = false;
                      else if(window == "bedroomGarden" && bedroomGardenTargetState == true) bedroomGardenTargetState = false;
                      else if(window == "livingRoomFront" && livingRoomFrontTargetState == true) livingRoomFrontTargetState = false;
                      else if(window == "livingRoomSideFront" && livingRoomFrontTargetState == true) livingRoomFrontTargetState = false;
                      else if(window == "livingRoomSideGarden" && livingRoomSideGardenTargetState == true) livingRoomSideGardenTargetState = false;
                      else if(window == "livingRoomGarden" && livingRoomGardenTargetState == true) livingRoomGardenTargetState = false;
                      else if(window == "kitchenFront" && kitchenFrontTargetState == true) kitchenFrontTargetState = false;
                      else if(window == "kitchenSide" && kitchenSideTargetState == true) kitchenSideTargetState = false;
                      else if(window == "studySide" && studySideTargetState == true) studySideTargetState = false;
                      else if(window == "studyGarden" && studyGardenTargetState == true) studyGardenTargetState = false;                      
                      window = "";
                }
            }        

        if(dressing && dressingTargetState == true) ShutterLibOn( BOARD_A, 1, 20 );
        else if(dressing && dressingTargetState == false) ShutterLibOn(BOARD_A, 2, 20);
        if(bedroomSide && bedroomSideTargetState == true) ShutterLibOn( BOARD_A, 3, 20);
        else if(bedroomSide && bedroomSideTargetState == false) ShutterLibOn( BOARD_A, 4, 20);
        if(bathroom && bathroomTargetState == true) ShutterLibOn( BOARD_A, 5, 20);
        else if(bathroom && bathroomTargetState == false) ShutterLibOn( BOARD_A, 6, 20);
        if(bedroomGarden && bedroomGardenTargetState == true) ShutterLibOn( BOARD_A, 7, 20);
        else if(bedroomGarden && bedroomGardenTargetState == false) ShutterLibOn( BOARD_A, 8, 20);

        if(livingRoomFront && livingRoomFrontTargetState == true) ShutterLibOn( BOARD_B, 1, 20 );
        else if(livingRoomFront && livingRoomFrontTargetState == false) ShutterLibOn(BOARD_B, 2, 20);
        if(livingRoomSideFront && livingRoomSideFrontTargetState == true) ShutterLibOn( BOARD_B, 3, 20);
        else if(livingRoomSideFront && livingRoomSideFrontTargetState == false) ShutterLibOn( BOARD_B, 4, 20);
        if(livingRoomSideGarden && livingRoomSideGardenTargetState == true) ShutterLibOn( BOARD_B, 5, 20);
        else if(livingRoomSideGarden && livingRoomSideGardenTargetState == false) ShutterLibOn( BOARD_B, 6, 20);
        if(livingRoomGarden && livingRoomGardenTargetState == true) ShutterLibOn( BOARD_B, 8, 30);
        else if(livingRoomGarden && livingRoomGardenTargetState == false) ShutterLibOn( BOARD_B, 7, 30);

        if(kitchenSide && kitchenSideTargetState == true) ShutterLibOn( BOARD_C, 1, 20 );
        else if(kitchenSide && kitchenSideTargetState == false) ShutterLibOn(BOARD_C, 2, 20);
        if(studySide && studySideTargetState == true) ShutterLibOn( BOARD_C, 3, 20);
        else if(studySide && studySideTargetState == false) ShutterLibOn( BOARD_C, 4, 20);
        if(studyGarden && studyGardenTargetState == true) ShutterLibOn( BOARD_C, 5, 20);
        else if(studyGarden && studyGardenTargetState == false) ShutterLibOn( BOARD_C, 6, 20);
        if(kitchenFront && kitchenFrontTargetState == true) ShutterLibOn( BOARD_C, 7, 20);
        else if(kitchenFront && kitchenFrontTargetState == false) ShutterLibOn( BOARD_C, 8, 20);       
    }

#if MQTTUseAuth == 1
    bool authReceived(const ReasonCodes reasonCode, const DynamicStringView & authMethod, const DynamicBinDataView & authData, const PropertiesView & properties)
    {
        fprintf(stdout, "Auth packet received\n");
        fprintf(stdout, "  AuthMethod: %.*s\n", authMethod.length, authMethod.data);
        fprintf(stdout, "  AuthData: %.*s\n", authData.length, authData.data);
        fprintf(stdout, "  Reason Code: %d\n", (int)reasonCode);

        if (authData.length != strlen("Whizz") || memcmp(authData.data, "Whizz", authData.length))
        {
            fprintf(stdout, "Bad authentication answer from server");
            return false;
        }
        DynamicBinDataView data(strlen("Bees"), (const uint8*)"Bees");
        if (Network::Client::MQTTv5::ErrorType ret = client->auth(Protocol::MQTT::V5::ContinueAuthentication, authMethod, data))
        {
            fprintf(stdout, "Failed auth with error: %d\n", (int)ret);
            return false;
        }
        return true;
    }
    Network::Client::MQTTv5 * client;
#endif
};

String publishTopic, publishMessage;

String publish(const char * topic, const char * message)
{
    // Remember the message to publish that we'll do once connected
    publishTopic = topic; 
    publishMessage = message;
    return "";
}

Network::Client::MQTTv5::QoSDelivery QoS = Network::Client::MQTTv5::QoSDelivery::AtMostOne;
String setQoS(const String & qos)
{
    if (qos == "0" || qos.caselessEqual("atmostone")) QoS = Network::Client::MQTTv5::QoSDelivery::AtMostOne;
    else if (qos == "1" || qos.caselessEqual("atleastone")) QoS = Network::Client::MQTTv5::QoSDelivery::AtLeastOne;
    else if (qos == "2" || qos.caselessEqual("exactlyone")) QoS = Network::Client::MQTTv5::QoSDelivery::ExactlyOne;
    else
    {
        return "Please specify either 0 or atleastone, 1 or atmostone, 2 or exactlyone for QoS option";
    }
    return "";
}

volatile bool cont = true;
void ctrlc(int sig)
{
    if (sig == SIGINT) cont = false;
}

#if MQTTUseTLS == 1
struct ScopeFile
{
    FILE * f;
    operator FILE *() const { return f; }
    ScopeFile(const char * path) : f(fopen(path, "rb")) {}
    ~ScopeFile() { if (f) fclose(f); }
};

String readFile(const String & path)
{
    String ret;
    ScopeFile f(path);
    if (!f) return ret;
    if (fseek(f, 0,  SEEK_END)) return ret;
    long size = ftell(f);
    if (fseek(f, 0, SEEK_SET)) return ret;

    if (!size || size > 2048*1024) return ret;
    int r = fread(ret.Alloc(size), 1, size, f);
    ret.releaseLock(r);
    return ret;
}
#endif

int main(int argc, const char ** argv)
{
    String server;
    String username;
    String password;
    String clientID;
    String subscribe;
    String certFile;

    unsigned keepAlive = 300;
    bool   dumpComm = false;
    bool   retainPublishedMessage = false;

    Arguments::declare(server, "The server URL (for example 'mqtt.mine.com:1883')", "server");
    Arguments::declare(username, "The username to use", "username");
    Arguments::declare(password, "The password to use", "password", "pw");
    Arguments::declare(clientID, "The client identifier to use", "clientid");
    Arguments::declare(keepAlive, "The client keep alive time", "keepalive");
    Arguments::declare(publish, "Publish on the topic the given message", "publish", "pub");
    Arguments::declare(retainPublishedMessage, "Retain published message", "retain");
    Arguments::declare(setQoS, "Quality of service for publishing or subscribing", "qos");
    Arguments::declare(subscribe, "The subscription topic", "subscribe", "sub");
    Arguments::declare(certFile, "Expected broker certificate in DER format", "der");

    Arguments::declare(dumpComm, "Dump communication", "verbose");

    String error = Arguments::parse(argc, argv);
    if (error)
    {
        fprintf(stderr, "%s\n", (const char*)error);
        return argc != 1;
    }
    if (!server) return fprintf(stderr, "No server URL given. Leaving...\n");

    InitLogger initLogger(dumpComm);


    // Ok, parse the given URL
    // Add a scheme if none provided
    if (!server.fromFirst("://")) server = "mqtt://" + server;
    Network::Address::URL serverURL(server);
    uint16 port = serverURL.stripPortFromAuthority(1883);

    MessageReceiver receiver;

#if MQTTUseTLS == 1
    Protocol::MQTT::Common::DynamicBinaryData brokerCert;
    if (certFile)
    {
        // Load the certificate if provided
        String certContent = readFile(certFile);
        brokerCert = Protocol::MQTT::Common::DynamicBinaryData(certContent.getLength(), (const uint8*)certContent);
    }
    Protocol::MQTT::Common::DynamicBinDataView certView(brokerCert);
    Network::Client::MQTTv5 client(clientID, &receiver, certFile ? &certView : (Network::Client::MQTTv5::DynamicBinDataView*)0);
#else
    Network::Client::MQTTv5 client(clientID, &receiver);
#endif
    Network::Client::MQTTv5::DynamicBinDataView pw(password.getLength(), (const uint8*)password);

#if MQTTUseAuth == 1
    receiver.client = &client;
    Protocol::MQTT::V5::Property<Network::Client::MQTTv5::DynamicStringView> method(Protocol::MQTT::V5::AuthenticationMethod, Network::Client::MQTTv5::DynamicStringView("DumbledoreOffice"));
    Protocol::MQTT::V5::Property<Network::Client::MQTTv5::DynamicBinDataView> data(Protocol::MQTT::V5::AuthenticationData, Network::Client::MQTTv5::DynamicBinDataView(strlen("Fizz"), (const uint8*)"Fizz"));
    Protocol::MQTT::V5::Properties props;

    props.append(&method);
    props.append(&data);

    if (Network::Client::MQTTv5::ErrorType ret = client.connectTo(serverURL.getAuthority(), port, serverURL.getScheme().caselessEqual("mqtts"),
                                                                  (uint16)min(65535U, keepAlive), true, username ? (const char*)username : nullptr, password ? &pw : nullptr, nullptr, QoS, false, &props))
#else
    if (Network::Client::MQTTv5::ErrorType ret = client.connectTo(serverURL.getAuthority(), port, serverURL.getScheme().caselessEqual("mqtts"),
                                                                  (uint16)min(65535U, keepAlive), true, username ? (const char*)username : nullptr, password ? &pw : nullptr))
#endif
    {
        return fprintf(stderr, "Failed connection to %s with error: %d\n", (const char*)serverURL.asText(), (int)ret);
    }
    printf("Connected to %s\n", (const char*)serverURL.asText());

    ShutterLibInitialize();

    // Check if we have some subscription
    if (subscribe)
    {
        if (Network::Client::MQTTv5::ErrorType ret = client.subscribe(subscribe, Protocol::MQTT::V5::GetRetainedMessageAtSubscriptionTime, true, QoS, retainPublishedMessage))
        {
            return fprintf(stderr, "Failed subscribing to %s with error: %d\n", (const char*)subscribe, (int)ret);
        }
        printf("Subscribed to %s\nWaiting for messages...\n", (const char*)subscribe);

        // Then enter the event loop here
        signal(SIGINT, ctrlc);
        while (cont)
        {
            if (Network::Client::MQTTv5::ErrorType ret = client.eventLoop())
                return fprintf(stderr, "Event loop failed with error: %d\n", (int)ret);

        }

#if MQTTUseUnsubscribe == 1
        // Unsubscribe from the topic here
        Protocol::MQTT::V5::UnsubscribeTopic topic((const char*)subscribe, true);
        if (Network::Client::MQTTv5::ErrorType ret = client.unsubscribe(topic, 0))
        {
            return fprintf(stderr, "Failed unsubscribing to %s with error: %d\n", (const char*)subscribe, (int)ret);
        }
        // Run the event loop once more to fetch the unsubscribe ACK (not absolutely required when leaving, but for sample code
        if (Network::Client::MQTTv5::ErrorType ret = client.eventLoop())
            return fprintf(stderr, "Event loop failed with error: %d\n", (int)ret);

        Network::Client::MQTTv5::ErrorType ret = client.getUnsubscribeResult();
        fprintf(ret == 0 ? stdout : stderr, "Unsubscribe result: %d\n", (int)ret);
#endif

        return 0;
    }

    // Check if we have something to publish
    if (publishTopic)
    {
        // Publish
        if (Network::Client::MQTTv5::ErrorType ret = client.publish(publishTopic, publishMessage, publishMessage.getLength(), retainPublishedMessage, QoS))
        {
            return fprintf(stderr, "Failed publishing %s to %s with error: %d\n", (const char*)publishMessage, (const char*)publishTopic, (int)ret);
        }
        printf("Published %s to %s\n", (const char*)publishMessage, (const char*)publishTopic);
        return 0;
    }

    return 0;
}
