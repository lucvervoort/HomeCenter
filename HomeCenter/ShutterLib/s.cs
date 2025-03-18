using System;
using System.Runtime.InteropServices;
 
interface DllLoadUtils 
{
    IntPtr LoadLibrary(string fileName);
    void FreeLibrary(IntPtr handle);
    IntPtr GetProcAddress(IntPtr dllHandle, string name);
}

public class DllLoadUtilsWindows : DllLoadUtils 
{
    void DllLoadUtils.FreeLibrary(IntPtr handle) 
    {
      FreeLibrary(handle);
    }

    IntPtr DllLoadUtils.GetProcAddress(IntPtr dllHandle, string name) 
    {
      return GetProcAddress(dllHandle, name);
    }

    IntPtr DllLoadUtils.LoadLibrary(string fileName) 
    {
      return LoadLibrary(fileName);
    }

    [DllImport("kernel32")]
    private static extern IntPtr LoadLibrary(string fileName);

    [DllImport("kernel32.dll")]
    private static extern int FreeLibrary(IntPtr handle);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetProcAddress (IntPtr handle, string procedureName);
}

internal class DllLoadUtilsLinux : DllLoadUtils 
{
    public IntPtr LoadLibrary(string fileName) 
	{
            return dlopen(fileName, RTLD_NOW);
    }

    public void FreeLibrary(IntPtr handle) 
	{
            dlclose(handle);
    }

    public IntPtr GetProcAddress(IntPtr dllHandle, string name) 
	{
      // clear previous errors if any
      dlerror();
      var res = dlsym(dllHandle, name);
      var errPtr = dlerror();
      if (errPtr != IntPtr.Zero) 
      {
        throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errPtr));
      }
      return res;
    }

    const int RTLD_NOW = 2;

    [DllImport("libdl.so")]
    private static extern IntPtr dlopen(String fileName, int flags);
        
    [DllImport("libdl.so")]
    private static extern IntPtr dlsym(IntPtr handle, String symbol);

    [DllImport("libdl.so")]
    private static extern int dlclose(IntPtr handle);

    [DllImport("libdl.so")]
    private static extern IntPtr dlerror();
}

public class HelloWorld
{
    private static bool IsLinux() 
	{
      var p = (int) Environment.OSVersion.Platform;
      return (p == 4) || (p == 6) || (p == 128);
    }

	internal delegate void MyCrossplatformBar(int a, int b);

    public void foo() 
	{
      DllLoadUtils dllLoadUtils = IsLinux() ? (DllLoadUtils) new DllLoadUtilsLinux() : new DllLoadUtilsWindows();
      string libraryName;

      if (IsLinux()) 
      {
        libraryName = IntPtr.Size == 8 ? "mylib64.so" : "mylib32.so";
      } 
      else 
      {
        libraryName = IntPtr.Size == 8 ? "mylib64.dll" : "mylib32.dll";
      }

      var dllHandle = dllLoadUtils.LoadLibrary(libraryName);
      var functionHandle = dllLoadUtils.GetProcAddress(dllHandle, "MyCrossplatformBar");

      var method = (MyCrossplatformBar) Marshal.GetDelegateForFunctionPointer(functionHandle, typeof (MyCrossplatformBar));
      method(10, 15);
    }

	internal delegate void _ShutterLibDebugOn();
	internal delegate void _ShutterLibDebugOff();
	internal delegate void _ShutterLibInit();
	internal delegate void _ShutterLibShutdown();
	internal delegate void _ShutterLibOn(int board, int relay, int secs);

    static public void Main()
    {
        Console.WriteLine ("Hello Mono World");
	    if( IsLinux() )
	    {
		    Console.WriteLine("Linux found");
	    }
        DllLoadUtils dllLoadUtils = IsLinux() ? (DllLoadUtils) new DllLoadUtilsLinux() : new DllLoadUtilsWindows();
	    var dllHandle = dllLoadUtils.LoadLibrary("libshutter.so");

	    if(dllHandle != (IntPtr)0)
	    {
        	var functionDebugOn = dllLoadUtils.GetProcAddress(dllHandle, "ShutterLibDebugOn");
        	var debugOn = (_ShutterLibDebugOn) Marshal.GetDelegateForFunctionPointer(functionDebugOn, typeof (_ShutterLibDebugOn));
        	debugOn();
        	var functionInit = dllLoadUtils.GetProcAddress(dllHandle, "ShutterLibInitialize");
        	var init = (_ShutterLibInit) Marshal.GetDelegateForFunctionPointer(functionInit, typeof (_ShutterLibInit));
        	init();

        	var functionOn = dllLoadUtils.GetProcAddress(dllHandle, "ShutterLibOn");
        	var shutterOn = (_ShutterLibOn) Marshal.GetDelegateForFunctionPointer(functionOn, typeof (_ShutterLibOn));

		    shutterOn(1,7,25);

        	var functionShutdown = dllLoadUtils.GetProcAddress(dllHandle, "ShutterLibShutdown");
        	var shutdown = (_ShutterLibShutdown) Marshal.GetDelegateForFunctionPointer(functionShutdown, typeof (_ShutterLibShutdown));
        	shutdown();
        	var functionDebugOff = dllLoadUtils.GetProcAddress(dllHandle, "ShutterLibDebugOff");
        	var debugOff = (_ShutterLibDebugOff) Marshal.GetDelegateForFunctionPointer(functionDebugOff, typeof (_ShutterLibDebugOff));
        	debugOff();
	    }
    }
}
