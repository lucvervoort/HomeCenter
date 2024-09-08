using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.TermComposition
{
    [PrologPredicate(Name = "copy_term", Arity = 2)]
    public class copy_term_2 : BindingPredicate
    {
         Term _in;
        Term _out;

        public copy_term_2(IPredicate continuation, IEngine engine, Term inTerm, Term outTerm)
            : base(continuation,engine)
        {
            _in = inTerm;
            _out = outTerm;
        }

        public override PredicateResult Call()
        {
            _out = _out.Dereference();
            _in = _in.Dereference();
            if (!_out.Unify(_in.Copy(new Variables()),Engine.BoundedVariables,false))
            {
                return Fail();
            }
            return Success();
        }

    }
}
