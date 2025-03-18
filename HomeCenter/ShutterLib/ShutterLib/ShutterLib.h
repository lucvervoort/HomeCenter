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

// This class is exported from the dll
class SHUTTERLIB_API CShutterLib {
public:
	CShutterLib(void);
	// TODO: add your methods here.
};

extern SHUTTERLIB_API int nShutterLib;

SHUTTERLIB_API int fnShutterLib(void);
