using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.AtomProcessinh
{
    [PrologPredicate(Name = "atom_chars", Arity = 2)]
    public class atom_chars_2 : BindingPredicate
    {
        Term _atom;
        Term _chars;
        Term _iatom;
        Term _ichars;
        PrologList _list;
        public atom_chars_2(IPredicate continuation, IEngine engine, Term atom, Term chars)
            : base(continuation,engine)
        {
            _iatom = atom;
            _ichars = chars;
        }

        public override PredicateResult Call()
        {
            _atom = _iatom.Dereference();
            _chars = _ichars.Dereference();
            ErrorCheck();
            if (_atom.IsBound)
            {
                return CallContinuation(Split());
            }
            else
            {
                return CallContinuation(Assemble());
            }
        }

        private PredicateResult Assemble()
        {
            StringBuilder sb = new StringBuilder();
            _list = _chars as PrologList;
            foreach (Term t in _list)
            {
                Structure str = t as Structure;
                if (str == null)
                    throw new TypeMismatchException(ValidTypes.Character, t, this);
                if(str.Name.Length != 1)
                    throw new TypeMismatchException(ValidTypes.Character, t, this);
                sb.Append(str.Name);
            }
            Structure newStruct = new Structure(sb.ToString());
            _atom.UnifyWithStructure(newStruct,Engine.BoundedVariables,false);
            return PredicateResult.Success;
        }

        private PredicateResult Split()
        {
            Structure strAtom = (Structure)_atom;
            _list = new PrologList();
            foreach (char c in strAtom.Name)
            {
                _list = _list.Append(new PrologList(new Structure(c.ToString())));
            }
            if (!_list.Unify(_chars,Engine.BoundedVariables,false))
            {
                return Fail();
            }
            return PredicateResult.Success;
        }

        private void ErrorCheck()
        {
            if (!_atom.IsBound && !_chars.IsBound)
            {
                throw new InstantiationException(this);
            }

            if (_atom.IsBound && !_atom.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _atom, this);
            }

            if (!_atom.IsBound && !_chars.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _chars, this);
            }

           
        }



    }
}
