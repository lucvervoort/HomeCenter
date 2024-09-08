/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Compiler
{
    class PrologProgram : Dictionary<string, ClausesList>
    {
        string _name;
        string _module = null;
        List<string> _publicPredicates = new List<string>() ;

        public List<string> PublicPredicates
        {
          get { return _publicPredicates; }
          set { _publicPredicates = value; }
        }

        public string Module
        {
            get { return _module; }
            set { _module = value; }
        }

        public bool IsModule
        {
            get { return Module != null && Module.Length > 0; }
        }
        List<Structure> _initGoals = new List<Structure>();

        public List<Structure> InitializationGoals
        {
            get { return _initGoals; }
            set { _initGoals = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public PrologProgram(string name)
        {
            _name = name;
        }

        public bool ContainsKey(Structure f)
        {
            string predicate = f.GetPI().GetPredicateName();
            return ContainsKey(predicate);
        }

        public void Add(Clause c)
        {
            if (c.Head != null)
            {
                string predicate = c.Head.GetPI().GetPredicateName();
                if (!ContainsKey(predicate))
                {
                    this.Add(predicate, new ClausesList());
                }
                this[predicate].Add(c);

            }
            else
            {
                throw new PrologException("Null Head");
            }
        }
    }

}
