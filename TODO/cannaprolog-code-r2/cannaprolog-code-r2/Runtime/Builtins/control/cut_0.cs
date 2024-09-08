/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using System.Diagnostics;

namespace Canna.Prolog.Runtime.Builtins.Control
{
    [PrologPredicate(Name="!",Arity=0)]
    public class cut_0 : BasePredicate
    {
        int depth = -1;

        internal int Depth
        {
            get { return depth; }
            set { depth = value; }
        }

          public cut_0( IPredicate continuation, IEngine engine)
              :base(continuation,engine)
        {
            depth = engine.GetDepth();
            if (engine.Peek() == engine.CurrentGoal)
                depth--;
        }

        #region IPredicate Members

        public override PredicateResult Call()
        {
            Debug.Assert(depth >= 0);
            Engine.CutToDepth(depth);
            return CallContinuation();
        }



        #endregion
}
}
