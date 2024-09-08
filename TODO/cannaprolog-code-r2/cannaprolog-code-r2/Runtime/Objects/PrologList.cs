/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace Canna.Prolog.Runtime.Objects
{
    public class PrologList : Structure, IEnumerable<Term>
    {
        public PrologList()
            : base(".")
        { }

        public PrologList(Term t)
            : base(".", t, PrologList.EmptyList)
        { }

        public PrologList(params Term[] terms)
            : base(".", terms)
        {

        }
        public override Term Copy(Variables v)
        {
            PrologList f = new PrologList();
            foreach (Term t in Args)
            {
                f.AddArg(t.Copy(v));
            }
            return f;
        }

        public override Term Dereference()
        {
            if (isEmpty()) return this;
            PrologList f = new PrologList();
            foreach (Term t in Args)
            {
                f.AddArg(t.Dereference());
            }
            return f;

        }

        public override void Accept(ITermVisitor visitor)
        {
            visitor.VisitList(this);
        }


        /// <summary>
        /// An empty list is in prolog the .() functor.
        /// </summary>
        private static PrologList s_emptylist = new PrologList();

        public static PrologList EmptyList
        {
            get { return PrologList.s_emptylist; }
            set { PrologList.s_emptylist = value; }
        }



        public Term Head
        {
            get { return args[0]; }
            set { args[0] = value; }
        }
        public Term Tail
        {
            get { return args[1]; }
            set { args[1] = value; }
        }

        public virtual bool isEmpty()
        {
            return Arity == 0;
        }

        public override bool IsList
        {
            get
            {
                return true;
            }
        }
        public PrologList Append(PrologList newitem)
        {

            if (isEmpty())
            {
                return newitem;
            }
            else
            {
                Tail = ((PrologList)Tail).Append(newitem);
                return this;
            }
        }


        public override void Write(StreamTerm output, WriteOptions options)
        {
            if (isEmpty())
            {
                output.Write("[]");
                return;
            }
            output.Write('[');
            PrologList list = this;
            while (!list.isEmpty())
            {
                list[0].Write(output,options);

                Term next = list[1];
                if (next is PrologList)
                {
                    if (next.IsList)
                    {
                        list = (PrologList)next;
                        if (!list.isEmpty())
                        {
                            output.Write(", ");
                        }
                        continue;
                    }
                }
                output.Write('|');
                next.Write(output, options);
                list = PrologList.EmptyList;
            }
            output.Write(']');
        }

        public override object ObjectValue
        {
            get
            {
                ArrayList list = new ArrayList();
                foreach (Term t in this)
                {
                    list.Add(t.ObjectValue);
                }
                return list;
            }
        }

        #region IEnumerable<Term> Members

        public IEnumerator<Term> GetEnumerator()
        {
            PrologList current = this;
            while (!(current.isEmpty()))
            {
                yield return current.Head;
                current = (PrologList)current.Tail;
            }
           
        }

        #endregion



        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
}

}
