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
    [PrologPredicate(Name = @"true", Arity = 0)]
    public class true_0 : BasePredicate
    {

        public true_0(IPredicate continuation, IEngine engine) : base(continuation,engine) { }

        #region IPredicate Members

        public override PredicateResult Call()
        {
            return  Success();
        }



        #endregion
    }
}
