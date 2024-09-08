/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Canna.Prolog.Runtime.Objects
{
    internal class TermComparer : ITermComparer, ITermVisitor, IEqualityComparer<Term>
    {
        int ret=int.MinValue;
        Term other;
        public TermComparer()
        {
        }


        #region ITermComparer Members

        public int Compare(Term t1, Term t2)
        {
            other = t2;
            t1.Accept(this);
            return ret;
        }

        #endregion

        #region ITermVisitor Members

        public void VisitVar(Var var)
        {
           ret = new VarComparer(var).CompareTo(other);
        }

        public void VisitInteger(Integer integer)
        {
            ret = new IntegerComparer(integer).CompareTo(other);
        }

        public void VisitFloat(Floating floating)
        {
            ret = new FloatingComparer(floating).CompareTo(other);
        }

        public void VisitStruct(Structure structure)
        {
            ret = new StructureComparer(structure).CompareTo(other);
        }

        public void VisitList(PrologList list)
        {
            new StructureComparer((Structure)list).CompareTo(other);
        }

        #endregion


        //inner classes
        class VarComparer : ITermVisitor
        {
            Var _var;
            int ret;
            public VarComparer(Var v)
            { _var = v;}

            public int CompareTo(Term t)
            {
                t.Accept(this);
                return ret;
            }

            #region ITermVisitor Members

            public void VisitVar(Var var)
            {
                ret = string.Compare(_var.Name, var.Name);
            }

            public void VisitInteger(Integer integer)
            {
                ret = -1;
            }

            public void VisitFloat(Floating floating)
            {
                ret = -1;
            }

            public void VisitStruct(Structure structure)
            {
                ret = -1;
            }

            public void VisitList(PrologList list)
            {
                ret = -1;
            }

            #endregion
        }

        class IntegerComparer : ITermVisitor
        {
            Integer _int;
            int ret;

            public IntegerComparer(Integer i)
            {
                _int = i;
            }

            public int CompareTo(Term t)
            {
                t.Accept(this);
                return ret;
            }

            #region ITermVisitor Members

            public void VisitVar(Var var)
            {
                ret = 1;
            }

            public void VisitInteger(Integer integer)
            {
                ret = _int.Value.CompareTo(integer.Value);
            }

            public void VisitFloat(Floating floating)
            {
                ret = ((double)_int.Value).CompareTo(floating);
            }

            public void VisitStruct(Structure structure)
            {
                ret = -1;
            }

            public void VisitList(PrologList list)
            {
                ret = -1;
            }

            #endregion
}

        class FloatingComparer : ITermVisitor
        {

            Floating _float;
            int ret;

            public FloatingComparer(Floating f)
        {
            _float = f;
        }

            public int CompareTo(Term t)
            {
                t.Accept(this);
                return ret;
            }

            #region ITermVisitor Members

            public void VisitVar(Var var)
            {
                ret = 1;
            }

            public void VisitInteger(Integer integer)
            {
                ret = _float.Value.CompareTo((double)integer.Value);
            }

            public void VisitFloat(Floating floating)
            {
                ret = _float.Value.CompareTo(floating.Value);
            }

            public void VisitStruct(Structure structure)
            {
                ret = -1;
            }

            public void VisitList(PrologList list)
            {
                ret = -1;
            }

            #endregion
}

        class StructureComparer : ITermVisitor
        {
            Structure _struct;
            int ret;

            public StructureComparer(Structure str)
            {
                _struct = str;
            }

            public int CompareTo(Term t)
            {
                t.Accept(this);
                return ret;
            }

            #region ITermVisitor Members

            public void VisitVar(Var var)
            {
                ret = 1;
            }

            public void VisitInteger(Integer integer)
            {
                ret = 1;
            }

            public void VisitFloat(Floating floating)
            {
                ret = 1;
            }

            public void VisitStruct(Structure structure)
            {
                int i = _struct.Arity.CompareTo(structure.Arity);
                if (i == 0)
                {
                    i = _struct.Name.CompareTo(structure.Name);
                }
                int j = 0;
                while((i == 0)&&(j<_struct.Arity))
                {
                    i = new TermComparer().Compare(_struct[j], structure[j]);
                    ++j;
                }
                ret = i;
            }

            public void VisitList(PrologList list)
            {
                //Exception!
                VisitStruct((Structure)list);
            }

            #endregion
}

#region IEqualityComparer<Term> Members

public bool Equals(Term x, Term y)
{
    return (this.Compare(x, y) == 0);
}

public int GetHashCode(Term obj)
{
    return obj.GetHashCode();
}

#endregion
}


}
