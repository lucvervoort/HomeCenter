/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Control
{
    //[PrologPredicate(Name = "->", Arity = 2)]
    public class IfThen : BasePredicate
    {
        IPredicate _if, _then;
        int _depth = -1;

        public IfThen(IPredicate continuation, IEngine engine, IPredicate ifpred)
            : base(continuation,engine)
        {
            _if = ifpred;
        }

        public override PredicateResult Call()
        {
             cut_0 cut = new cut_0(Continuation, Engine);
             _if.Continuation = cut;
             this.Continuation = _if;
             return Success();
        }
    }

    public class IfThenElse : MultiClausePredicate
    {
        IPredicate _if, _else,_then;

        Term _arg1, _arg2;

        public IfThenElse(IPredicate continuation, IEngine engine, IPredicate ifgoal, IPredicate thengoal, IPredicate elsegoal)
            : base(continuation, engine)
        {
            _if = ifgoal;
            _else = elsegoal;
            _then = thengoal;
        }

        public IfThenElse(IPredicate continuation, IEngine engine, Structure term1, Structure term2)
            : base(continuation, engine)
        {
            _arg1 = term1;
            _arg2 = term2;
        }

        public override PredicateResult Call()
        {
            return base.Call();
        }




        protected override IEnumerator<IPredicate> getClauses()
        {
            cut_0 cut = new cut_0(_then, Engine);
            _if.Continuation = cut;
            yield return new BasicClause(_if, Engine);
            yield return new BasicClause(_else, Engine);
        }
    }

}
