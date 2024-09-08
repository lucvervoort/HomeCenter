/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Canna.Prolog.Runtime.Objects
{
    public class TermList : List<Term>
    {
        public void Unbind()
        {
            foreach (Term t in this)
            {
                t.Unbind();
            }
        }

        public void Dereference()
        {
            foreach (Term t in this)
            {
                t.Dereference();
            }
        }
    }

    //changed on 24/06 to accomodate variables as subclauses
    public class SubclausesList : List<Term>
    {
        public void Append(Term t)
        {
            if (t is Structure)
            {
                Structure s = (Structure)t;
                if (s.Name == ",")
                {
                    if ((s.Args[0] is Structure)||s.Args[0] is Var)
                    {
                        Add(s.Args[0]);
                    }
                    else
                    {
                        throw new PrologException("Append: wrong type");
                    }
                    if (s.Arity == 2)
                    {
                        Append(s.Args[1]);
                    }
                    else
                    {
                        throw new MalformedClauseException();
                    }
                }
                else
                {
                    Add(s);
                }
            }
            else if (t is Var)
            {
                Add(t);
            }
            else
            {
                throw new PrologException("Append: wrong type");
            }
        }
    }

    public class VarList : List<Var>
    {
        public void Unbind()
        {
            foreach (Var v in this)
            {
                v.Unbind();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            int i = 0;
            foreach (Var v in this)
            {
                sb.Append(v.Name);
                sb.Append("->");
                sb.Append(v.Dereference().ToString());
                if (++i < Count) sb.Append(",");
            }
            sb.Append("}");
            return sb.ToString();
        }

        public PrologList ToPrologList()
        {
            PrologList plist = new PrologList();
            foreach(Var var in this)
            {
                plist = plist.Append(new PrologList(var));
            }
            return plist;
        }

    }


}
