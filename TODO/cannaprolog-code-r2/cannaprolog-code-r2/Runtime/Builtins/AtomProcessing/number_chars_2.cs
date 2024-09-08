using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.AtomProcessing
{
    [PrologPredicate(Name = "number_chars", Arity = 2)]
    public class number_chars_2 : BindingPredicate
    {
        Term _number;
        Term _chars;
        Term _inumber;
        Term _ichars;
        PrologList _list;
        public number_chars_2(IPredicate continuation, IEngine engine, Term number, Term chars)
            : base(continuation,engine)
        {
            _inumber = number;
            _ichars = chars;
        }

        public override PredicateResult Call()
        {
            _number = _inumber.Dereference();
            _chars = _ichars.Dereference();
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
            Number n=null;
            int i;
            double d;
            if (Int32.TryParse(sb.ToString(),out i))
            {
                n = new Integer(i);
            }
            else
            {
                if(Double.TryParse(sb.ToString(),out d))
                {
                    n = new Floating(d);
                }
            }
            if (n == null)
            {
                throw new SyntaxErrorException("illegal_number", this);
            }
            
            _number.Unify(n,Engine.BoundedVariables,false);
            return PredicateResult.Success;
        }

        private PredicateResult Split()
        {
            string numname = _number.ToString();

            _list = new PrologList();
            foreach (char c in numname)
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
            if (!_number.IsBound && !_chars.IsBound)
            {
                throw new InstantiationException(this);
            }

            if (_number.IsBound && !_number.IsNumber)
            {
                throw new TypeMismatchException(ValidTypes.Number, _number, this);
            }

            if (!_number.IsBound && !_chars.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _chars, this);
            }

           
        }


    }
}
