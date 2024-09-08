/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Arithmetic
{
    [PrologPredicate(Name="is",Arity=2)]
    public class is_2 : BindingPredicate
    {
        private Term _arg1, _arg2;

        public is_2(IPredicate continuation, IEngine engine, Term arg1, Term arg2):base(continuation,engine)
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }
        #region IPredicate Members



        public override PredicateResult Call()
        {
            Term arg1 = _arg1.Dereference();
            Term arg2 = _arg2.Dereference();
            if (!arg2.IsGround)
            {
                throw new InstantiationException(this);
            }
            ExpressionEvaluator expeval = new ExpressionEvaluator(arg2);
            Term res = expeval.Eval();
            if (!arg1.Unify(res, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();

        }


        #endregion
}
}
