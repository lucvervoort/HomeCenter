/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/


using System.Runtime.InteropServices;
namespace Canna.Prolog.Runtime.Utils
{
    public class PerformanceMeter
    {
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);

        long ctr1 = 0, ctr2 = 0, freq = 0;

        public bool Start()
        {
            return QueryPerformanceCounter(ref ctr1) != 0;
        }

        public void Stop()
        {
            QueryPerformanceCounter(ref ctr2);
        }

        public long Freq
        {
            get
            {
                QueryPerformanceFrequency(ref freq);
                return freq;
            }
        }

        public double GetElapsedTimeInSeconds()
        {
            return (ctr2 - ctr1) * 1.0 / Freq;
        }


    }
}
