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
    [PrologPredicate(Name = @"\+", Arity = 1)]
    public class not_1 : BasePredicate
    {
        private Term _arg1;

        public not_1(IPredicate continuation,IEngine engine, Term arg1)
            :base(continuation,engine)
        {
            _arg1 = arg1;
        }

        #region IPredicate Members

        public override PredicateResult Call()
        {
            Structure str = _arg1.Dereference() as Structure;
            if (str == null)
            {
                throw new TypeMismatchException(ValidTypes.Callable, _arg1, this);
            }
            IPredicate clause1 = new call_1(null,Engine, str);
            //TODO WORNG!!!
            int depth = Engine.GetDepth();
            PredicateResult res = Engine.ExecuteGoal(clause1);
            Engine.CutToDepth(depth);
            if (res.IsFailed)
            {
                return CallContinuation();
            }
            else
            {
                return Fail();
            }
        }

        public override PredicateResult Redo()
        {
            return Fail();
        }



        #endregion
    }
}
