using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.AtomProcessing
{
    [PrologPredicate(Name = "sub_atom", Arity = 5)]
    public class sub_atom_5 : BindingPredicate
    {
        Term _atom, _before, _length, _after, _subatom;
        Term _iatom, _ibefore, _ilength, _iafter, _isubatom;
        string stratom;
        Integer before, length, after;


        public sub_atom_5(IPredicate continuation, IEngine engine, Term atom, Term before, Term length,
             Term after, Term subatom)
            : base(continuation,engine)
        {
            _iatom = atom;
            _ibefore = before;
            _ilength = length;
            _iafter = after;
            _isubatom = subatom;

        }

        public override PredicateResult Call()
        {
            _atom = _iatom.Dereference();
            _before = _ibefore.Dereference();
            _length = _ilength.Dereference();
            _after = _iafter.Dereference();
            _subatom = _isubatom.Dereference();
            ErrorCheck();
            stratom = ((Structure)_atom).Name;

            before = new Integer(0);
            length = new Integer(0);
            after = new Integer(stratom.Length);
            return CallContinuation(Scompose());
        }

        private PredicateResult Scompose()
        {
            PredicateResult res = PredicateResult.Failed;
            Engine.AddChoicePoint(this);
            if (_before.UnifyWithInteger(before, Engine.BoundedVariables, false))
                if (_length.UnifyWithInteger(length, Engine.BoundedVariables, false))
                    if (_after.UnifyWithInteger(after, Engine.BoundedVariables, false))
                    {
                        Structure str = new Structure(stratom.Substring(before.Value, length.Value));
                        if (_subatom.UnifyWithStructure(str, Engine.BoundedVariables, false))
                        {
                            res = PredicateResult.Success;
                        }
                    }
            return res;
        }

        public override PredicateResult Redo()
        {
            if (!NextScomposition())
                return Fail();
            return CallContinuation(Scompose());
        }

        private bool NextScomposition()
        {
            length.Value++;
            after.Value--;
            if (after.Value < 0)
            {
                before.Value++;
                length.Value = 0;
                after.Value = stratom.Length - before.Value;
            }
            return after.Value >= 0;
        }

        private void ErrorCheck()
        {
            if (!_atom.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (!_atom.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _atom, this);
            }
            if (_subatom.IsBound && !_subatom.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _subatom, this);
            }
            if (_before.IsBound && !_before.IsInteger)
            {
                throw new TypeMismatchException(ValidTypes.Integer, _before, this);
            }
            if (_length.IsBound && !_length.IsInteger)
            {
                throw new TypeMismatchException(ValidTypes.Integer, _length, this);
            }
            if (_after.IsBound && !_after.IsInteger)
            {
                throw new TypeMismatchException(ValidTypes.Integer, _after, this);
            }
            //TODO: check not_less_than_zero
        }
    }
}
