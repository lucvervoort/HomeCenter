using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.TermComposition
{
    [PrologPredicate(Name = "arg", Arity = 3)]
    public class arg_3 : BindingPredicate
    {
        Term _arg;
        Term _term;
        Term _value;
        Term _iarg;
        Term _iterm;
        Term _ivalue;

        Structure str;
        Integer _internalArg;

        public arg_3(IPredicate continuation, IEngine engine, Term arg, Term term, Term val)
            : base(continuation,engine)
        {
            _iarg = arg;
            _iterm = term;
            _ivalue = val;
        }

        public override PredicateResult Call()
        {
            _arg = _iarg.Dereference();
            _term = _iterm.Dereference();
            _value = _ivalue.Dereference();
            _internalArg = null;
            if (!_term.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (_term.IsAtom)
            {
                return Fail();
            }
            //if (!_term.IsCompound || )
            //{
            //    throw new TypeMismatchException(ValidTypes.C, _term);
            //}
            str = _term as Structure;
            if (_arg.IsBound)
            {
                return ExtractArg();
            }
            else
            {
                _internalArg = new Integer(0);
                return IterateArgs();
            }
        }

        public override PredicateResult Redo()
        {

            return IterateArgs();

        }

        private PredicateResult IterateArgs()
        {
            if (_internalArg == null)
            {
                return Fail();
            }
            _internalArg.Value++;
            if (_internalArg.Value > str.Arity)
            {
                return Fail();
            }
            if (_internalArg.Value < str.Arity)
            {
                Engine.AddChoicePoint(this);
            }
            if (_arg.UnifyWithInteger(_internalArg,Engine.BoundedVariables,false))
            {
                return ExtractArg();
            }
            else
            {
                return Fail();
            }
        }

        private PredicateResult ExtractArg()
        {
            Integer arg = _arg.Dereference() as Integer;
            if (arg == null) throw new TypeMismatchException(ValidTypes.Integer, _arg,this);
            if (arg.Value < 0) throw new DomainException(ValidDomains.not_less_than_zero, _arg,this);
            if (arg.Value > 0 && arg.Value <= str.Arity)
            {
                Term t = str[arg.Value - 1];
                if (!_value.Unify(t, Engine.BoundedVariables, false))
                {
                    return Fail();
                }
                else
                {
                    return Success();
                }
            }
            return Fail();
        }



    }
}
