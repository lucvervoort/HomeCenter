using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Common;

namespace Workbench
{
    class member2 : IPredicate 
    {
        protected Term arg1, arg2;
        protected IPredicate continuation;
        protected VarList boundedVars = new VarList();

        private IEnumerator<member2> clauses = null;
        private PredicateResult _lastResult = PredicateResult.SuccessWithAlternatives;

        public member2(Term t1, Term t2, IPredicate cont)
        {
            arg1 = t1.dereference();
            arg2 = t2.dereference();
            this.continuation = cont;
        }
        public member2() { }

        #region IPredicate Members

        public virtual PredicateResult execute()
        {
            if (clauses == null)
            {
               clauses = getClauses();
               if (!clauses.MoveNext()) return PredicateResult.Failed;
            }
            if (_lastResult != PredicateResult.SuccessWithAlternatives)
            {
                if (clauses.Current.boundedVars != null)
                {
                    clauses.Current.boundedVars.Unbind();
                }
                if (!clauses.MoveNext()) return PredicateResult.Failed;
            }

          

            _lastResult = clauses.Current.execute();
            while (_lastResult == PredicateResult.Failed)
            {
                bool bChoice = clauses.MoveNext();
                if (!bChoice) return PredicateResult.Failed;
                _lastResult = clauses.Current.execute();
            }
            return PredicateResult.SuccessWithAlternatives;
            
        }

    

        private IEnumerator<member2> getClauses()
        {
            yield return new member2_2(arg1.dereference(), arg2.dereference(), null);
            yield return new member2_1(arg1.dereference(),arg2.dereference(),null);
        }

        #endregion

        //Clauses
        class member2_1 : member2
        {
            public member2_1(Term arg1, Term arg2, IPredicate cont)
                : base(arg1, arg2, cont)
            { }

            public override PredicateResult execute()
            {
                Variables vars = new Variables();
                Functor f1 = new Functor(".");
                f1.AddArg(vars.getVar("X"));
                f1.AddArg(vars.getVar("_"));

                Var v2 = vars.getVar("X");

                if(arg1.unifyFunctor(f1,boundedVars))
                    if (arg2.unifyVar(v2, boundedVars))
                    {
                        return PredicateResult.Success;
                    }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }

           
        }

        class member2_2 : member2
        {
            private member2 cont1;
            private bool redo = false;

            public member2_2(Term arg1, Term arg2, IPredicate cont)
                : base(arg1, arg2, cont)
            { }


            public override PredicateResult execute()
            {
                if (redo) return cont1.execute();

                Variables vars = new Variables();
                Functor f1 = new Functor(".");
                f1.AddArg(vars.getVar("_"));
                f1.AddArg(vars.getVar("T"));

                Var v2 = vars.getVar("X");

                if (arg1.unifyFunctor(f1, boundedVars))
                    if (arg2.unifyVar(v2, boundedVars))
                    {
                        //call to member(T,X).
                        if (cont1 == null)
                        {
                            cont1 = new member2(vars.getVar("T"), vars.getVar("X"), null);
                            redo = true;
                        }
                        return cont1.execute();
                       
                    }

                boundedVars.Unbind();
                return PredicateResult.Failed;
            }


        }

    }
}
