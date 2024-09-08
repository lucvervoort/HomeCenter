using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Builtins;
using Canna.Prolog.Runtime.Builtins.Control;

namespace Canna.Prolog.Runtime.Objects
{
    public class DynamicPredicate : MultiClausePredicate
    {
        Term[] _arguments;

        ClausesList _clauses = new ClausesList();

        public ClausesList Clauses
        {
            get { return _clauses; }
            private set { _clauses = value; }
        }

        internal DynamicPredicate()
        {

        }

        private DynamicPredicate(IPredicate continuation, IEngine engine, ClausesList clauses, params Term[] args)
            : base(continuation,engine)
        {
            _arguments = args;
            _clauses = clauses;
        }

        public DynamicPredicate GetNew(IPredicate cont, IEngine engine, params Term[] args)
        {
            DynamicPredicate ndp = new DynamicPredicate(cont, engine, Clauses, args);
            return ndp;
        }

        //public string PredicateIndicator
        //{
        //    get { return _indicator; }
        //    set { _indicator = value; }
        //}

        protected override IEnumerator<IPredicate> getClauses()
        {
            foreach(Clause clause in Clauses)
            {
                Variables var = new Variables();
                //TODO: just to compile
                DynamicClause dc = new DynamicClause(clause.Copy(var), null, Engine, _arguments);
                yield return dc;
            }
        }

        public void AppendClause(Clause clause)
        {
            _clauses.Add(clause);
        }

        public void InsertClause(Clause clause)
        {
            _clauses.Insert(0, clause);
        }

        public IEnumerator<Clause> GetUnifyingHeads(Structure head, VarList var)
        {
            foreach (Clause c in Clauses)
            {
                if (c.Head.UnifyWithStructure(head, var, false))
                {
                    yield return c;
                }
                var.Unbind();
            }
        }

        public bool Remove(Clause c)
        {
            return Clauses.Remove(c);
        }

        public bool RetractClause(Clause clause, VarList _var)
        {
            bool found = false;
            VarList var = new VarList();
            foreach (Clause c in Clauses)
            {
                var.Unbind();
                var.Clear();
                if (!c.Head.Unify(clause.Head,var,false))
                {
                    continue;
                }
                else
                {
                    if( (c.Body != null)&&(clause.Body != null))
                    {
                        if (!c.Body.Unify(clause.Body,var,false))
                        {
                            continue;
                        }
                    }
                    found = true;
                    var.Unbind();
                    c.Head.Unify(clause.Head, _var, false);
                    if (c.Body != null)
                    {
                        c.Body.Unify(clause.Body, _var, false);
                    }
                    Clauses.Remove(c);

                    break;
                    
                }
            }
            return found;
        }


        #region dynamic clauses

        class DynamicClause : BindingPredicate
        {

            private Clause _clause;
            private Term[] _arguments;

            public Clause Clause
            {
                get { return _clause; }
                set { _clause = value; }
            }

            public DynamicClause(Clause clause,IPredicate cont, IEngine engine, params Term[] arguments)
                :base(cont,engine)
            {
                _clause = clause;
                _arguments = arguments;
            }

            #region IPredicate Members

            public override PredicateResult Call()
            {
                int i;
                for ( i = 0; i < _arguments.Length; ++i)
                {
                    _arguments[i] = _arguments[i].Dereference();
                }
                 i = 0;
                foreach (Term arg in Clause.Head.Args)
                {
                    if (!arg.Unify(_arguments[i++], Engine.BoundedVariables, false))
                    {
                        return Fail();
                    }
                }
                if (Clause.Body != null)
                {

                    this.Continuation = new call_1(Continuation, Engine, Clause.Body);
                    return this.Continuation.Call();
                }
                else
                {
                    return PredicateResult.Success;
                }
            }




            #endregion
        }

        #endregion
    }
}
