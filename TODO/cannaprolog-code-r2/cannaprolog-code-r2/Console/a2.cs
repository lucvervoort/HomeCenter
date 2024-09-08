using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Common;

namespace Workbench
{
    class a2 : BasePredicate
    {
        protected Term arg1, arg2;

         public a2(Term t1, Term t2,  IPredicate cont):base(cont)
        {
            arg1 = t1.dereference();
            arg2 = t2.dereference();
           
        }

        protected override IEnumerator<IPredicate> getClauses()
        {
            yield return new a2_1(arg1, arg2, continuation);
            yield return new a2_2(arg1, arg2, continuation);
        }

        #region clauses
        class a2_1 : a2
        {
            public a2_1(Term arg1, Term arg2, IPredicate cont)
                : base(arg1, arg2, cont)
            { }


            public override PredicateResult call()
            {
                Variables vars = new Variables();


                Var v1 = vars.getVar("X");
                Var v2 = vars.getVar("Y");

                if (arg1.unifyVar(v1, boundedVars))
                    if (arg2.unifyVar(v2, boundedVars))
                        {
                            this.continuation = new b1(v1, new c1(v2, continuation));
                            return continuation.call();
                        }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

            public override PredicateResult redo()
            {
                return continuation.redo();
            }
        }
        class a2_2 : a2
        {
            public a2_2(Term arg1, Term arg2, IPredicate cont)
                : base(arg1, arg2, cont)
            { }


            public override PredicateResult call()
            {
                Variables vars = new Variables();


                Var v1 = vars.getVar("X");
                Var v2 = vars.getVar("Y");

                if (arg1.unifyVar(v1, boundedVars))
                    if (arg2.unifyVar(v2, boundedVars))
                    {
                        this.continuation = new d1(v1, new c1(v2, continuation));
                        return continuation.call();
                    }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

            public override PredicateResult redo()
            {
                return continuation.redo();
            }
        }
        #endregion
    }
}
