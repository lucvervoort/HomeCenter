/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Canna.Prolog.Runtime.Objects
{
    /// <summary>
    /// Clause is a head functor and zero or more subgoals
    /// </summary>

    public class Clause
    {
        Structure head;
        Structure body;

        public Structure Head
        {
            get
            {
                return head;
            }
            set
            {
                head = value;
            }
        }

        public Structure Body
        {
            get
            {
                return body;
            }
        }


        public void AppendSubGoals(Structure f)
        {
            body = f;
        }



        void Init()
        {
            //body = new SubclausesList();
        }

        public Clause()
        {
            Init();
        }
        /// <summary>
        /// Append a term to the body
        /// </summary>
        /// <param name="t"></param>


        public override string ToString()
        {
            return ToString(new WriteOptions());
        }

        public string ToString(WriteOptions opts)
        {
            string s = string.Empty;
            if (head != null)
                s = head.ToString(opts);
            if (Body != null)
            {
                s = s + " :- ";
                s = s + Body.ToString(opts);
            }
            s = s + ".";
            return s;
        }

        public Structure ToFunctor()
        {
            if (Body!=null)
            {
                return new Structure(":-",Head,Body);
            }
            else//It's a fact
            {
                return Head;
            }
            
        }

        public static Clause FromFunctor(Structure functor)
        {
            if (functor != null)
			{
				Clause c = new Clause();
                if ((functor.Name == ":-") && (functor.Arity == 2))
				{
                    c.Head = (Structure)functor[0];
                    c.AppendSubGoals((Structure)functor[1]);
				}
				else
				{
                    c.Head = functor;
				}
				return c;
			}
			else
				return null;
        }

        public Clause Copy(Variables v)
        {
            Clause c = new Clause();
            c.head = (Structure)Head.Copy(v);
            if (Body != null)
            {
                c.body = (Structure)Body.Copy(v);
            }
            return c;
        }
    }

    public class ClausesList : List<Clause>
    {

    }

    public class MalformedClauseException : PrologException
    {
        public MalformedClauseException()
            : base("Malformed Clause")
        { }
    }
}
