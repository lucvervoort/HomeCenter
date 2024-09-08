using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.AtomProcessing
{
    [PrologPredicate(Name = "atom_length", Arity = 2)]
    public class atom_length_2 : BindingPredicate
    {

        Term _atom;
        Term _length;
        Term _iatom, _ilength;
        public atom_length_2(IPredicate continuation, IEngine engine, Term atom, Term length)
            : base(continuation, engine)
        {
            _iatom = atom;
            _ilength = length;
        }

        public override PredicateResult Call()
        {
            _atom = _iatom.Dereference();
            _length = _ilength.Dereference();
            if (!_atom.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (!_atom.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _atom,this);
            }
            if (_length.IsBound && !(_length.IsInteger))
            {
                throw new TypeMismatchException(ValidTypes.Integer, _length,this);
            }
            if (_length.IsBound && ((Integer)_length).Value < 0)
            {
                throw new DomainException(ValidDomains.not_less_than_zero, _length,this);
            }
            Structure atom = _atom as Structure;
            if (!_length.UnifyWithInteger(new Integer(atom.Name.Length), Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }
}
