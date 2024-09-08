using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Directives
{
    [PrologPredicate(Name = "discontiguous", Arity = 1)]
    public class discontiguous_1 : BasePredicate
    {
        Term _pi;

        public discontiguous_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _pi = arg1;
        }

        public override PredicateResult Call()
        {
            _pi = _pi.Dereference();
            if (!_pi.IsBound)
            {
                throw new InstantiationException(this);
            }
            PredicateIndicator pi = PredicateIndicator.FromTerm(_pi, this);
            PredicateTable.Current.SetDiscontiguous(pi);
            return Success();

        }

    }
}
