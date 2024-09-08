/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Arithmetic
{
    public abstract class BaseComparisonPredicate : BasePredicate
    {
        private Term _arg1, _arg2;
        private Term _iarg1, _iarg2;

        public BaseComparisonPredicate(IPredicate continuation, IEngine engine, Term arg1, Term arg2):base(continuation,engine)
        {
            _iarg1 = arg1;
            _iarg2 = arg2;
        }

                #region IPredicate Members

        public override PredicateResult Call()
        {
            _arg1 = _iarg1.Dereference();
            if (!_arg1.IsGround)
            {
                throw new InstantiationException(this);
            }
            _arg2 = _iarg2.Dereference();
            if (!_arg2.IsGround)
            {
                throw new InstantiationException(this);
            }
            ExpressionEvaluator expeval = new ExpressionEvaluator(_arg1);
            Number n1 = expeval.Eval() as Number;
            if ((n1 == null))
            {
                throw new TypeMismatchException(ValidTypes.Number,_arg1, null);
            }
            expeval = new ExpressionEvaluator(_arg2);
            Number n2 = expeval.Eval() as Number;
            if ((n1 == null))
            {
                throw new TypeMismatchException(ValidTypes.Number, _arg2, null);
            }

            //NumberComparer nc = new NumberComparer();
            //return this.CallContinuation(GiveResult(nc.Compare(n1, n2)));
            TermComparer nc = new TermComparer();
            return this.CallContinuation(GiveResult(nc.Compare(n1, n2)));
        }

        protected abstract PredicateResult GiveResult(int comparison);
      



        #endregion


    }

    [PrologPredicate(Name = "<", Arity = 2)]
    public class LessThan : BaseComparisonPredicate
    {

        public LessThan(IPredicate continuation, IEngine engine, Term arg1, Term arg2):base(continuation, engine, arg1, arg2)
        {
        }


        protected override PredicateResult GiveResult(int comparison)
        {
            if (comparison < 0)
                return PredicateResult.Success;
            else
                return Fail();
        }
}

    [PrologPredicate(Name = ">", Arity = 2)]
    public class GreaterThan : BaseComparisonPredicate
    {

        public GreaterThan(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
            : base(continuation, engine, arg1, arg2)
        {
        }


        protected override PredicateResult GiveResult(int comparison)
        {
            if (comparison > 0)
                return PredicateResult.Success;
            else
                return Fail();
        }
    }

    [PrologPredicate(Name = "<=", Arity = 2)]
    public class LessThanOrEqual : BaseComparisonPredicate
    {

        public LessThanOrEqual(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
            : base(continuation, engine, arg1, arg2)
        {
        }


        protected override PredicateResult GiveResult(int comparison)
        {
            if (comparison <= 0)
                return PredicateResult.Success;
            else
                return Fail();
        }
    }

    [PrologPredicate(Name = ">=", Arity = 2)]
    public class GreaterThanOrEqual : BaseComparisonPredicate
    {

        public GreaterThanOrEqual(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
            : base(continuation, engine, arg1, arg2)
        {
        }


        protected override PredicateResult GiveResult(int comparison)
        {
            if (comparison >= 0)
                return PredicateResult.Success;
            else
                return Fail();
        }
    }

    [PrologPredicate(Name = @"=\=", Arity = 2)]
    public class Different : BaseComparisonPredicate
    {

        public Different(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
            : base(continuation, engine, arg1, arg2)
        {
        }


        protected override PredicateResult GiveResult(int comparison)
        {
            if (comparison != 0)
                return PredicateResult.Success;
            else
                return Fail();
        }
    }

    [PrologPredicate(Name = "=:=", Arity = 2)]
    public class Equal : BaseComparisonPredicate
    {

        public Equal(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
            : base(continuation, engine, arg1, arg2)
        {
        }


        protected override PredicateResult GiveResult(int comparison)
        {
            if (comparison == 0)
                return PredicateResult.Success;
            else
                return Fail();
        }
    }
}
