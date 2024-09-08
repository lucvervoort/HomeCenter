using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Control
{
    [PrologPredicate(Name = @"repeat", Arity = 0)]
    public class repeat_0 : BasePredicate
    {
        public repeat_0(IPredicate continuation, IEngine engine)
            : base(continuation,engine)
        {
        }

        public override PredicateResult Call()
        {
            return Redo();
        }

        public override PredicateResult Redo()
        {
            Engine.AddChoicePoint(this);
            return Success();
        }


    }
}
