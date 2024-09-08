using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Common;

namespace Workbench
{
    class append3 : BasePredicate, IPredicate
    {
        protected Term arg1, arg2, arg3;

 
        public append3(Term t1, Term t2, Term t3, IPredicate cont)
        {
            arg1 = t1.dereference();
            arg2 = t2.dereference();
            arg3 = t3.dereference();
            //this.continuation = cont;
        }

        public append3() { }



        protected override IEnumerator<IPredicate> getClauses()
        {
            yield return new append3_1(arg1,arg2,arg3,null);
            yield return new append3_2(arg1, arg2, arg3, null);

        }

        # region Clauses
        class append3_1 : append3
        {
            public append3_1(Term arg1, Term arg2, Term arg3,IPredicate cont)
                : base(arg1, arg2, arg3, cont)
            { }

            public override PredicateResult call()
            {
                Variables vars = new Variables();
                Functor f1 = new Functor(".");
                

                Var v2 = vars.getVar("L");
                Var v3 = vars.getVar("L");

                if (arg1.unifyFunctor(f1, boundedVars))
                    if (arg2.unifyVar(v2, boundedVars))
                        if(arg3.unifyVar(v3,boundedVars))
                        {
                            return PredicateResult.Success;
                            //throw new CutException();
                        }   

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

            public override PredicateResult redo()
            {
                return PredicateResult.Failed;
            }
        }

        class append3_2 : append3
        {

            private append3 cont1;

            public append3_2(Term arg1, Term arg2, Term arg3, IPredicate cont)
                : base(arg1, arg2, arg3, cont)
            { }

            public override PredicateResult call()
            {

                Variables vars = new Variables();
                Functor f1 = new Functor(".");
                f1.AddArg(vars.getVar("X"));
                f1.AddArg(vars.getVar("L1"));

                Var v2 = vars.getVar("L2");
                Functor f3 = new Functor(".");
                f3.AddArg(vars.getVar("X"));
                f3.AddArg(vars.getVar("L3"));

                if (arg1.unifyFunctor(f1, boundedVars))
                    if (arg2.unifyVar(v2, boundedVars))
                        if (arg3.unifyFunctor(f3, boundedVars))
                        {
                            //call to append(L1,L2,L3).
                            if (cont1 == null)
                            {
                                cont1 = new append3(vars.getVar("L1"), vars.getVar("L2"),vars.getVar("L3"), null);
                            }
                            return cont1.call();
                        }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

            public override PredicateResult redo()
            {
                return cont1.redo();
            }
        }

        # endregion

    }
}
