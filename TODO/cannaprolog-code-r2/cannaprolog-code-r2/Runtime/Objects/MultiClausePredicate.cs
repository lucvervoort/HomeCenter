/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Canna.Prolog.Runtime;

namespace Canna.Prolog.Runtime.Objects
{
 
    public abstract class MultiClausePredicate : BindingPredicate
    {

        private IEnumerator<IPredicate> clauses = null;
        private PredicateResult _lastResult = PredicateResult.SuccessWithAlternatives;
        private IPredicate _currentClause;

        bool bIsCut = false;

        protected bool IsCut
        {
            get { return bIsCut; }
            set { bIsCut = value; }
        }



        public MultiClausePredicate() { }

        public MultiClausePredicate(IPredicate continuation, IEngine engine):base(continuation,engine)
        {
        }

        public override PredicateResult Call()
        {
            Trace.WriteLineIf(_predicateSwitch.Enabled,"CALL: "+this.ToString());
            Trace.Indent();
            clauses = getClauses();

            if (!clauses.MoveNext())
            {
                return Fail();
            }
            _currentClause = null;

            return CallContinuation(InternalRedo());
        }



        private PredicateResult InternalRedo()
        {

            _currentClause = clauses.Current;
            if (clauses.Current == null) return Fail();


                if (clauses.MoveNext())
                {
                    Engine.AddChoicePoint(this);
                }

                _lastResult = _currentClause.Call();

                 return _lastResult;


        }


        public override PredicateResult Redo()
        {
            Trace.Unindent();

            Trace.WriteLineIf(_predicateSwitch.Enabled, "REDO: " + this.ToString());
            Trace.Indent();

            //...then try other clauses 
            return CallContinuation( InternalRedo());
        }



    


        protected abstract  IEnumerator<IPredicate> getClauses();
    }

    public abstract class BaseFact : MultiClausePredicate
    {
        public override PredicateResult Redo()
        {
            return Fail();
        }
    }

    public class BasicClause : BasePredicate
    {
        public BasicClause(IPredicate continuation, IEngine engine):base(continuation,engine)
        {

        }

        public override PredicateResult Call()
        {
            return Success();
        }
    }

}
