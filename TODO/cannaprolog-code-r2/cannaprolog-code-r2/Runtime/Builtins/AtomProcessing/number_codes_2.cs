using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.AtomProcessing
{
    [PrologPredicate(Name = "number_codes", Arity = 2)]
    public class number_codes_2 : BindingPredicate
    {
        Term _number;
        Term _codes;
        Term _inumber;
        Term _icodes;
        PrologList _list;
        public number_codes_2(IPredicate continuation, IEngine engine, Term number, Term codes)
            : base(continuation,engine)
        {
            _inumber = number;
            _icodes = codes;
        }

        public override PredicateResult Call()
        {
            _number = _inumber.Dereference();
            _codes = _icodes.Dereference();
            ErrorCheck();
            if (_number.IsBound)
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
            Number n = null;
            int i;
            double d;
            if (Int32.TryParse(sb.ToString(), out i))
            {
                n = new Integer(i);
            }
            else
            {
                if (Double.TryParse(sb.ToString(), out d))
                {
                    n = new Floating(d);
                }
            }
            if (n == null)
            {
                throw new SyntaxErrorException("illegal_number", this);
            }

            _number.Unify(n, Engine.BoundedVariables, false);
            return PredicateResult.Success; 
        }

        private PredicateResult Split()
        {
            string numname = _number.ToString();
            _list = new PrologList();
            foreach (char c in numname)
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
            if (!_number.IsBound && !_codes.IsBound)
            {
                throw new InstantiationException(this);
            }

            if (_number.IsBound && !_number.IsNumber)
            {
                throw new TypeMismatchException(ValidTypes.Number, _number, this);
            }

            if (!_number.IsBound && !_codes.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _codes, this);
            }

           
        }


    }
}
