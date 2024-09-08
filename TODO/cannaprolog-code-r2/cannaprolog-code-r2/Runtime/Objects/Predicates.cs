/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Canna.Prolog.Runtime.Objects
{

    public enum Result
    {
        Failed,
        Success,
        SuccessWithAlternatives
    }

    public struct PredicateResult
    {
        Result _result;
        IPredicate _continuation;

        public IPredicate Continuation
        {
            get { return _continuation; }
            set { _continuation = value; }
        }

        public PredicateResult(Result res)
        {
            _result = res;
            _continuation = null;
        }

        public PredicateResult(IPredicate continuation)
        {
            _continuation = continuation;
            _result = Result.Success;
        }

        public bool IsFailed
        {
            get
            {
                return (_result == Result.Failed);
            }
        }

        public bool IsSuccessWithAlternatives
        {
            get
            {
                return (_result == Result.SuccessWithAlternatives);
            }
        }



        public static PredicateResult Success = new PredicateResult(Result.Success);
        public static PredicateResult SuccessWithAlternatives = new PredicateResult(Result.SuccessWithAlternatives);
        public static PredicateResult Failed = new PredicateResult(Result.Failed);
    }

    public enum TraceEventType
        {
            Call,
            Exit,
            Fail,
            Redo
        }

    public class BasePredicate : IPredicate
    {



        private IPredicate continuation = null;
        private IEngine _engine;

        protected IEngine Engine
        {
            get { return _engine; }
        }
        public IPredicate Continuation
        {
            get { return continuation; }
            set { continuation = value; }
        }


        protected static BooleanSwitch _predicateSwitch = new BooleanSwitch("Base Predicate", "Provides Base Predicates Tracing");

        public BasePredicate() { }
        public BasePredicate(IPredicate cont,IEngine engine)
        {
            this.continuation = cont;
            _engine = engine;
        }



        protected PredicateResult CallContinuation()
        {
            return new PredicateResult(Continuation);
        }
        protected PredicateResult Success()
        {
            Trace.Unindent();
            Trace.WriteLineIf(_predicateSwitch.Enabled, "EXIT: " + this.ToString());

            return new PredicateResult(Continuation);
        }

 
        protected PredicateResult CallContinuation(PredicateResult res)
        {
            if (res.Continuation == null && !res.IsFailed)
            {
                res.Continuation = Continuation;
            }
            return res;
        }

        protected virtual void TraceEvent(TraceEventType evt, params Term[] args)
        {
            StringBuilder sb = new StringBuilder(evt.ToString());
            sb.Append(": ");
            sb.Append(GetType().Name);
            if (args.Length > 0)
            {
                sb.Append("(");
                int i = 1;
                foreach (Term t in args)
                {
                    sb.Append(t.ToString());
                    if (i++ < args.Length)
                        sb.Append(", ");
                }
                sb.Append(")");
            }
            Trace.WriteLineIf(_predicateSwitch.Enabled, sb);
            
        }

        #region IPredicate Members

        public virtual PredicateResult Call()
        {
            return Success();
        }

        public virtual PredicateResult Redo()
        {
            //Unbind();
            //if (Continuation != null)
            //{
            //    return Continuation.redo();
            //}
            //else
                return Fail();
        }





        #endregion

        public override string ToString()
        {
            Type t = this.GetType();
            StringBuilder predName = new StringBuilder(t.Name);

            object[] attr = t.GetCustomAttributes(typeof(PrologPredicateAttribute),true);
            foreach (PrologPredicateAttribute ppa in attr)
            {
                predName.Length = 0;
                predName.Append(ppa.Name);
                predName.Append("/");
                predName.Append(ppa.Arity);
            }
            if (t.IsNested)
            {
                int i = t.Name.ToString().LastIndexOf("_")+1;
                predName.Append("(");
                predName.Append(t.Name.Substring(i, t.Name.Length - i));
                predName.Append(")");
            }
            return predName.ToString();
        }

        protected virtual PredicateResult Fail()
        {
            Trace.Unindent();
            Trace.WriteLineIf(_predicateSwitch.Enabled, "FAIL: " + this.ToString());
            return PredicateResult.Failed;
        }


        protected virtual PredicateResult SuccessWithAlternatives()
        {
            Trace.Unindent();
            Trace.WriteLineIf(_predicateSwitch.Enabled, "EXIT: " + this.ToString());
            return PredicateResult.SuccessWithAlternatives;
        }
}

    public class BindingPredicate : BasePredicate
    {
        //private VarList boundedVars = new VarList();

        //protected VarList BoundedVariables
        //{
        //    get { return boundedVars; }
        //    set { boundedVars = value; }
        //}

        public BindingPredicate() { }

        public BindingPredicate(IPredicate cont,IEngine engine) : base(cont,engine) { }

        //public override void Unbind()
        //{
        //    if (BoundedVariables != null) BoundedVariables.Unbind();
        //    if (Continuation != null) Continuation.Unbind();
        //}

 
    }
}
