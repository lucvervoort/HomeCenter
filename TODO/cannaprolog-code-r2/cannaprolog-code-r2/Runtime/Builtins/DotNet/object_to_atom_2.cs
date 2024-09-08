using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.DotNet
{
    [PrologPredicate(Name = "object_to_atom", Arity = 2)]
    public class object_to_atom_2 : BasePredicate
    {

        Term _ob, _iob, _atom, _iatom;

        public object_to_atom_2(IPredicate continuation, IEngine engine,
            Term ob, Term atom):base(continuation,engine)
        {
            _iob = ob;
            _iatom = atom;
        }

        public override PredicateResult Call()
        {
            _ob = _iob.Dereference();
            _atom = _iatom.Dereference();
            ErrorCheck();
            if (_ob.IsBound)
            {
                Structure atom = new Structure(_ob.ObjectValue.ToString());
                if (!atom.Unify(_atom, Engine.BoundedVariables, false))
                {
                    return Fail();
                }
            }
            else
            {
                if (!_atom.IsAtom)
                {
                    throw new TypeMismatchException(ValidTypes.Atom, _atom, this);
                }
                ObjectTerm ot = new GenericObjectTerm<string>(((Structure)_atom).Name);
                _ob.UnifyWithObject(ot, Engine.BoundedVariables, false);
            }
            return base.Call();
        }

        private void ErrorCheck()
        {
            if (!_ob.IsBound && !_atom.IsBound)
            {
                throw new InstantiationException(this);
            }
        }
    }
}
