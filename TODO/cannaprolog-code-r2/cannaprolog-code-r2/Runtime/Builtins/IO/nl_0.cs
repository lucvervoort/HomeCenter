/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    [PrologPredicate(Name = "nl", Arity = 0)]
    public class nl_0 : BasePredicate
    {
        

        public nl_0( IPredicate continuation, IEngine engine)
            : base(continuation,engine)
        {
        }

        #region IPredicate Members

        public override PredicateResult Call()
        {
            StreamTerm.CurrentOutput.Newline();
            return Success();
        }



        #endregion


}

    [PrologPredicate(Name = "nl", Arity = 1)]
    public class nl_1 : StreamBasePredicate
    {


        public nl_1(IPredicate continuation, IEngine engine, Term stream)
            : base(continuation,engine,stream)
        {
        }

        #region IPredicate Members

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            StreamTerm stream = GetStream();
            stream.Newline();
            return Success();
        }



        #endregion


    }

}
