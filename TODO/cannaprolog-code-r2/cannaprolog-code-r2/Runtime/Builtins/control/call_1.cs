/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Compiler;

namespace Canna.Prolog.Runtime.Builtins.Control
{
    [PrologPredicate(Name = "call", Arity = 1)]
    public class call_1 : BasePredicate
    {
        Term _term;
        IPredicate _pred;

        public call_1(IPredicate continuation, IEngine engine, Term t):base(continuation,engine)
        {
            _term = t;
        }



        #region IPredicate Members

        public override PredicateResult Call()
        {
            Term term = _term.Dereference();
            if (!term.IsBound)
            {
                throw new InstantiationException(this);
            }
            Structure _f = term.Dereference() as Structure;
            if (_f == null)
            {
                throw new TypeMismatchException(ValidTypes.Callable, _term.Dereference(),this);
            }

            try
            {
                _pred = PredicateTable.Current.GetPredicate(_f.GetPI(), Continuation, Engine,_f.Args.ToArray());
            }
            catch (UnknownPredicateException upe)
            {
                string unknownFlag = PrologFlagCollection.Current.GetAtom("unknown");
                if (unknownFlag == null || unknownFlag == "error")
                {
                    //TODO: throw ExistenceEror!!!
                    throw;
                }
                if (unknownFlag == "warning")
                {
                    Console.WriteLine("Warning: " + upe.Message);
                }

                return Fail();
            }

            this.Continuation = _pred;
            return Success();
            //return _pred.call();
        }

        public override PredicateResult Redo()
        {
            if (_pred != null)
            {
                return _pred.Redo();
            }
            else
            {
                return Fail();
            }
        }



        #endregion
}
}
