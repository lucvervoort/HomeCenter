using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using System.Collections;

namespace Canna.Prolog.Runtime.Builtins.DotNet
{
    class TermConverter : ITermVisitor
    {
        Term theTerm;
        ObjectTerm result = null;
        public TermConverter(Term t)
        {
            theTerm = t;
        }

        public ObjectTerm Convert()
        {
            theTerm.Accept(this);

            return result; 
        }

        #region ITermVisitor Members

        public void VisitVar(Var var)
        {
            if (var.IsBound)
            {
                var.Dereference().Accept(this);
            }
            else
            {
                throw new InstantiationException(this);
            }
        }

        public void VisitInteger(Integer integer)
        {
            result = new GenericObjectTerm<int>(integer.Value);
        }

        public void VisitFloat(Floating floating)
        {
            result = new GenericObjectTerm<double>(floating.Value);
        }

        public void VisitStruct(Structure structure)
        {
            if (structure.IsAtom)
            {
                result = new GenericObjectTerm<string>(structure.Name);
            }
            else
            {
                result = new GenericObjectTerm<ArrayList>((ArrayList)structure.ObjectValue);
            }
        }

        public void VisitList(PrologList list)
        {
            result = new GenericObjectTerm<ArrayList>((ArrayList)list.ObjectValue);
        }

        #endregion
    }
}
