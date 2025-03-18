// ShutterLib.cpp : Defines the exported functions for the DLL.
//

#include "pch.h"
#include "framework.h"
#include "ShutterLib.h"


// This is an example of an exported variable
SHUTTERLIB_API int nShutterLib=0;

// This is an example of an exported function.
SHUTTERLIB_API int fnShutterLib(void)
{
    return 0;
}

// This is the constructor of a class that has been exported.
CShutterLib::CShutterLib()
{
    return;
}
