using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.AtomProcessing
{
    [PrologPredicate(Name = "atom_codes", Arity = 2)]
    public class atom_codes_2 : BindingPredicate
    {
        Term _atom;
        Term _codes;
        Term _iatom;
        Term _icodes;
        PrologList _list;
        public atom_codes_2(IPredicate continuation, IEngine engine, Term atom, Term codes)
            : base(continuation,engine)
        {
            _iatom = atom;
            _icodes = codes;
        }

        public override PredicateResult Call()
        {
            _atom = _iatom.Dereference();
            _codes = _icodes.Dereference();
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
            _list = _codes as PrologList;
            foreach (Term t in _list)
            {
                Integer code = t as Integer;
                if (code == null)
                    throw new RepresentationException(RepresentationFlags.CharacterCode, this);

                sb.Append((char)code.Value);
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
                _list = _list.Append(new PrologList(new Integer((int)c)));
            }
            if (!_list.Unify(_codes,Engine.BoundedVariables,false))
            {
                return Fail();
            }
            return PredicateResult.Success;
        }

        private void ErrorCheck()
        {
            if (!_atom.IsBound && !_codes.IsBound)
            {
                throw new InstantiationException(this);
            }

            if (_atom.IsBound && !_atom.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _atom, this);
            }

            if (!_atom.IsBound && !_codes.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _codes, this);
            }

           
        }


    }
}
