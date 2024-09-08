using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins.Control;

namespace Canna.Prolog.Runtime.Builtins.Control
{
    [PrologPredicate(Name = @"once", Arity = 1)]
    public class once_1 : BasePredicate
    {
        Term _goal;
        IPredicate _pred;

        public once_1(IPredicate continuation, IEngine engine, Term goal)
            : base(continuation,engine)
        {
            _goal = goal;
        }

        public override PredicateResult Call()
        {
            _goal = _goal.Dereference();
            _pred = new call_1(new cut_0(Continuation,Engine), Engine, _goal);
            return _pred.Call();
        }


    }
}
