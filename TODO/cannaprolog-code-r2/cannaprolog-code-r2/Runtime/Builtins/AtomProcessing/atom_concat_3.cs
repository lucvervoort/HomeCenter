using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.AtomProcessing
{
    [PrologPredicate(Name = "atom_concat", Arity = 3)]
    public class atom_concat_3 : BindingPredicate
    {
        Term _istart;
        Term _iend;
        Term _iwhole;

        Term _start;
        Term _end;
        Term _whole;

        Structure _currentStart;

        public atom_concat_3(IPredicate continuation, IEngine engine, Term start, Term end, Term whole)
            : base(continuation,engine)
        {
            _istart = start;
            _iend = end;
            _iwhole = whole;
        }

        public override PredicateResult Call()
        {
            _start = _istart.Dereference();
            _end = _iend.Dereference();
            _whole = _iwhole.Dereference();
            _currentStart = null;
            if (!_start.IsBound && !_whole.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (!_end.IsBound && !_whole.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (_start.IsBound && !_start.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _start, this);
            }
            if (_end.IsBound && !_end.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _end, this);
            }
            if (_whole.IsBound && !_whole.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _whole, this);
            }
            PredicateResult res = SwitchOperation();
            return CallContinuation(res);

        }

        private PredicateResult SwitchOperation()
        {
            if (!_whole.IsBound)
            {
                return Concatenate();
            }
            if (_end.IsBound)
            {
                return DetermineStart();
            }
            if (_start.IsBound)
            {
                return DetermineEnd();
            }
            _currentStart = null;
            return EnumerateConcatenations();
        }

        private PredicateResult EnumerateConcatenations()
        {
            string whole = ((Structure)_whole).Name;

            if (_currentStart == null)
            {
                _currentStart = new Structure("");
            }
            else
            {
                if (_currentStart.Name.Length < whole.Length)
                {
                    _currentStart.Name = whole.Substring(0, _currentStart.Name.Length + 1);
                }
            }
            if (_currentStart.Name.Length < whole.Length)
            {
                Engine.AddChoicePoint(this);
            }

            if (_start.UnifyWithStructure(_currentStart, Engine.BoundedVariables, false))
            {
                return (DetermineEnd());
            }
            else return Fail();
        }

        private PredicateResult DetermineEnd()
        {
            string whole = ((Structure)_whole).Name;
            string start = ((Structure)_start.Dereference()).Name;
            if (whole.StartsWith(start))
            {
                Structure end = new Structure(whole.Substring(start.Length,whole.Length-start.Length));
                if (end.Unify(_end, Engine.BoundedVariables, false))
                {
                    return PredicateResult.Success;
                }
            }
            return Fail();
        }

        private PredicateResult DetermineStart()
        {
            string whole = ((Structure)_whole).Name;
            string end = ((Structure)_end).Name;
            if (whole.EndsWith(end))
            {
                Structure start = new Structure(whole.Substring(0,whole.Length-end.Length));
                if (start.Unify(_start, Engine.BoundedVariables, false))
                {
                    return PredicateResult.Success;
                }
            }
            return Fail();

        }

        private PredicateResult Concatenate()
        {
            Structure whole = new Structure(((Structure)_start).Name +
                ((Structure)_end).Name);
            if (!whole.Unify(_whole, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            else
            {
                return PredicateResult.Success;
            }
        }

        public override PredicateResult Redo()
        {
            return  CallContinuation(EnumerateConcatenations());
      
        }

    }
}
