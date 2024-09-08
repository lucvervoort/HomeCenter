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
    [PrologPredicate(Name = "fail", Arity = 0)]
    public class fail_0 : BasePredicate , IPredicate
    {

        public fail_0(IPredicate continuation, IEngine engine) : base(continuation, engine){}

        #region IPredicate Members

        public override PredicateResult Call()
        {
            return Fail();
        }



        #endregion
}
}
