using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins
{

    public abstract class BaseTermComparisonPredicate : BasePredicate
    {
        private Term _arg1,_iarg1, _arg2, _iarg2;

        public BaseTermComparisonPredicate(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
            : base(continuation,engine)
        {
            _iarg1 = arg1;
            _iarg2 = arg2;
        }

        #region IPredicate Members

        public override PredicateResult Call()
        {
            _arg1 = _iarg1.Dereference();
            _arg2 = _iarg2.Dereference();
            TermComparer tc = new TermComparer();
            return this.CallContinuation(GiveResult(tc.Compare(_arg1, _arg2)));

        }

        protected abstract PredicateResult GiveResult(int comparison);




        #endregion


    }

    [PrologPredicate(Name = "@=<", Arity = 2)]
    public class LessThanOrEqualTerm : BaseTermComparisonPredicate
    {

        public LessThanOrEqualTerm(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
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

    [PrologPredicate(Name = "==", Arity = 2)]
    public class IdenticalTerm : BaseTermComparisonPredicate
    {

        public IdenticalTerm(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
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

    [PrologPredicate(Name = @"\==", Arity = 2)]
    public class NotIdenticalTerm : BaseTermComparisonPredicate
    {

        public NotIdenticalTerm(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
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

    [PrologPredicate(Name = "@<", Arity = 2)]
    public class LessThanTerm : BaseTermComparisonPredicate
    {

        public LessThanTerm(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
            : base(continuation, engine, arg1, arg2)
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

    [PrologPredicate(Name = "@>", Arity = 2)]
    public class GreaterThanTerm : BaseTermComparisonPredicate
    {

        public GreaterThanTerm(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
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

    [PrologPredicate(Name = "@>=", Arity = 2)]
    public class GreaterThanOrEqualTerm : BaseTermComparisonPredicate
    {

        public GreaterThanOrEqualTerm(IPredicate continuation, IEngine engine, Term arg1, Term arg2)
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
}
