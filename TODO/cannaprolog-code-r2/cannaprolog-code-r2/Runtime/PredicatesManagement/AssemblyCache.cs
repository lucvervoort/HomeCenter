/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime
{
    
    class AssemblyCache : Dictionary<string, PrologTextInfo>
    {
        private static AssemblyCache _theCache = new AssemblyCache();



        public static AssemblyCache Current
        {
            get { return _theCache; }
        }

        public void Add(string name, Assembly assembly, string prologfile)
        {
            this.Add(name, new PrologTextInfo(assembly, prologfile));
        }
        


    }

    class PrologTextInfo
    {
        string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        Assembly assembly;

        public Assembly Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }

        public PrologTextInfo(Assembly assembly, string prologtext)
        {
            filename = prologtext;
            this.assembly = assembly;
        }
    }
}
