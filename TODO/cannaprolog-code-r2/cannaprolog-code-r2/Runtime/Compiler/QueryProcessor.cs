/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins;
using Canna.Prolog.Runtime.Lexical;

namespace Canna.Prolog.Runtime.Compiler
{
    /*
    public class QueryProcessor : IPredicate
    {
        private IPredicate _goal;
        private Variables _goalVars = new Variables();

        public Variables GoalVariables
        {
            get { return _goalVars; }
            set { _goalVars = value; }
        }

        public QueryProcessor(string query)
        {
            _goal = GetQuery(GetQueryGoals(query), _goalVars,0);
        }

        private static IPredicate GetQuery(Structure subgoals, Variables vars, int ord)
        {
            if (subgoals.Count <= ord) return null;
            return new call_1((Structure)subgoals[ord].Copy(vars), GetQuery(subgoals, vars, ord + 1));
        }

        private static Structure GetQueryGoals(string query)
        {
            PrologTokenizer tokenizer = new PrologTokenizer(query);
            PrologParser parser = new PrologParser(tokenizer);
            Clause c = parser.getQuery();
            return c.Body;
        }


        #region IPredicate Members

        public PredicateResult call()
        {
            return _goal.call();
        }

        public PredicateResult redo()
        {
            return _goal.redo();
        }

        public void Unbind()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
}
     * */
}
