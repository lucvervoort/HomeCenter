#ifndef _SHUTTER_H
#define _SHUTTER_H

// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the SHUTTERLIB_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// SHUTTERLIB_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.

#ifdef SHUTTERLIB_EXPORTS
#define SHUTTERLIB_API __declspec(dllexport)
#else
#define SHUTTERLIB_API __declspec(dllimport)
#endif

//// This class is exported from the dll
//class SHUTTERLIB_API CShutterLib {
//public:
//	CShutterLib(void);
//	// TODO: add your methods here.
//};
//extern SHUTTERLIB_API int nShutterLib;
//SHUTTERLIB_API int fnShutterLib(void);

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

extern "C" void ShutterLibDebugOn();
extern "C" void ShutterLibDebugOff();

extern "C" void ShutterLibInitialize();
extern "C" void ShutterLibShutdown();

extern "C" void ShutterLibAllOff();
extern "C" void ShutterLibOn( int board, int relay, int onSecs );
extern "C" void ShutterLibOnMany( int board, int switches[], int switchesCount, int s );

#endif

