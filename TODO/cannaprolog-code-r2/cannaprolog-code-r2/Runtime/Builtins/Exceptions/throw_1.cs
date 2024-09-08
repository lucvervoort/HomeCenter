using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Exceptions
{
    [PrologPredicate(Name = "throw", Arity = 1)]
    public class throw_1 : BasePredicate
    {
        Term _term;
        public throw_1(IPredicate continuation,IEngine engine, Term t)
            : base(continuation,engine)
        {
            _term = t;
        }

        public override PredicateResult Call()
        {
            throw new PrologRuntimeException(Continuation, _term.Dereference());
        }
    }
}
