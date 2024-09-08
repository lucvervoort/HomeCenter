/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins
{
    [PrologPredicate(Name = "print", Arity = 1)]
    public class print_1 : BasePredicate
    {
        Term _arg1;

        public print_1(IPredicate continuation, IEngine engine, Term arg1):base(continuation,engine)
        {
            _arg1 = arg1.Dereference();
        }

        #region IPredicate Members

        public override PredicateResult Call()
        {
            Console.Write(_arg1.Dereference().ToString());
            return Success();
        }

       
 
        #endregion


}

}
