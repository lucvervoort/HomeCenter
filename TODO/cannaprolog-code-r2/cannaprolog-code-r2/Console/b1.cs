using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Common;

namespace Workbench
{
    class b1 : BasePredicate
    {
        protected Term arg1;

        public b1(Term t1, IPredicate cont)
            : base(cont)
        {
            arg1 = t1.dereference();

        }

        protected override IEnumerator<IPredicate> getClauses()
        {
            yield return new b1_1(arg1, continuation);
            yield return new b1_2(arg1, continuation);

        }

        #region clauses
        class b1_1 : b1
        {
            public b1_1(Term arg1, IPredicate cont)
                : base(arg1, cont)
            { }


            public override PredicateResult call()
            {
                Variables vars = new Variables();



                if (arg1.unifyInteger(new Integer(1), boundedVars))
                {
                    return PredicateResult.Success;
                }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

            public override PredicateResult redo()
            {
                return PredicateResult.Failed;
            }
        }

        class b1_2 : b1
        {
            public b1_2(Term arg1, IPredicate cont)
                : base(arg1, cont)
            { }


            public override PredicateResult call()
            {
                Variables vars = new Variables();



                if (arg1.unifyInteger(new Integer(2), boundedVars))
                {
                    return PredicateResult.Success;
                }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

            public override PredicateResult redo()
            {
                return PredicateResult.Failed;
            }
        }

        #endregion
    }

    class c1 : BasePredicate
    {
        protected Term arg1;

        public c1(Term t1, IPredicate cont)
            : base(cont)
        {
            arg1 = t1.dereference();

        }

        protected override IEnumerator<IPredicate> getClauses()
        {
            yield return new c1_1(arg1, continuation);
            yield return new c1_2(arg1, continuation);

        }

        #region clauses
        class c1_1 : c1
        {
            public c1_1(Term arg1, IPredicate cont)
                : base(arg1, cont)
            { }


            public override PredicateResult call()
            {
                Variables vars = new Variables();



                if (arg1.unifyInteger(new Integer(3), boundedVars))
                {
                    return PredicateResult.Success;
                }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

            public override PredicateResult redo()
            {
                return PredicateResult.Failed;
            }
        }
        class c1_2 : c1
        {
            public c1_2(Term arg1, IPredicate cont)
                : base(arg1, cont)
            { }


            public override PredicateResult call()
            {
                Variables vars = new Variables();



                if (arg1.unifyInteger(new Integer(4), boundedVars))
                {
                    return PredicateResult.Success;
                }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

            public override PredicateResult redo()
            {
                return PredicateResult.Failed;
            }
        }

        #endregion
    }

    class d1 : BasePredicate
    {
        protected Term arg1;

        public d1(Term t1, IPredicate cont)
            : base(cont)
        {
            arg1 = t1.dereference();

        }

        protected override IEnumerator<IPredicate> getClauses()
        {
            yield return new d1_1(arg1, continuation);

        }

        #region clauses
        class d1_1 : d1
        {
            public d1_1(Term arg1, IPredicate cont)
                : base(arg1, cont)
            { }


            public override PredicateResult call()
            {
                Variables vars = new Variables();



                if (arg1.unifyInteger(new Integer(5), boundedVars))
                {
                    return PredicateResult.Success;
                }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

            public override PredicateResult redo()
            {
                return PredicateResult.Failed;
            }
        }

        #endregion
    }
}