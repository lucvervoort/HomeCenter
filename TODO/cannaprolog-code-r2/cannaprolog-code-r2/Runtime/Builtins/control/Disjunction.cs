/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins.Control;

namespace Canna.Prolog.Runtime.Builtins.Control
{
    //[PrologPredicate(Name = @";", Arity = 2)]
    public class Disjunction : MultiClausePredicate
    {
        IPredicate continuation1, continuation2;

        Term _arg1, _arg2;

        public Disjunction(IPredicate continuation, IEngine engine, IPredicate cont1, IPredicate cont2):base(continuation,engine)
        {
            continuation1 = cont1;
            continuation2 = cont2;
        }

        public Disjunction(IPredicate continuation, IEngine engine, Structure term1, Structure term2)
            : base(continuation,engine)
        {
            _arg1 = term1;
            _arg2 = term2;
        }

        public override PredicateResult Call()
        {
            if (_arg1 != null && _arg2 != null)
            {
                InitContinuations();
            }
            return base.Call();
        }



        private void InitContinuations()
        {
            _arg1 = _arg1.Dereference();
            _arg2 = _arg2.Dereference();
            if (!(_arg1 is Structure))
            {
                throw new TypeMismatchException(ValidTypes.Callable, _arg1,this);
            }
            if (!(_arg2 is Structure))
            {
                throw new TypeMismatchException(ValidTypes.Callable, _arg2,this);
            }

            continuation1 = new call_1(Continuation, Engine,(Structure)_arg1);
            continuation2 = new call_1(Continuation, Engine,(Structure)_arg2);
        }

        protected override IEnumerator<IPredicate> getClauses()
        {
            yield return new BasicClause(continuation1,Engine);
            yield return new BasicClause(continuation2, Engine);
        }
}
}
