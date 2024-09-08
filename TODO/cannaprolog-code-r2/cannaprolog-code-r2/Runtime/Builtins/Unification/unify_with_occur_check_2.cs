/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
namespace Canna.Prolog.Runtime.Builtins.Unification
{
    [PrologPredicate(Name = "unify_with_occur_check", Arity = 2)]
    public class unify_with_occur_check_2 : BindingPredicate
    {
         private Term _arg1, _arg2;

        public unify_with_occur_check_2(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
            : base(continuation,engine)
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        public override PredicateResult Call()
        {
            if (!_arg1.Dereference().Unify(_arg2.Dereference(), Engine.BoundedVariables, true))
            {
                return Fail();
            }
            return Success();

        }

    }
}
