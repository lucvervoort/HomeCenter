/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins.Control;

namespace Canna.Prolog.Runtime.Builtins.control
{

    //[PrologPredicate(Name = @",", Arity = 2)]
    class Conjunction : BasePredicate
    {
        Term _arg1;
        Term _arg2;

        IPredicate _pred;

        public Conjunction(IPredicate continuation, IEngine engine, Term term1, Term term2)
            : base(continuation,engine)
        {
            _arg1 = term1;
            _arg2 = term2;
        }

        private void InitContinuation()
        {
            _arg1 = _arg1.Dereference();
            _arg2 = _arg2.Dereference();
            if (!(_arg1 is Structure))
            {
                throw new TypeMismatchException(ValidTypes.Callable, _arg1, this);
            }
            if (!(_arg2 is Structure))
            {
                throw new TypeMismatchException(ValidTypes.Callable, _arg2, this);
            }

            _pred = new call_1(new call_1(Continuation, Engine, (Structure)_arg2), Engine, (Structure)_arg1);
        }

        public override PredicateResult Call()
        {
            if (_pred == null)
            {
                InitContinuation();
            }
            this.Continuation = _pred;
            return Success();
            //return _pred.call();
        }




    }
}
