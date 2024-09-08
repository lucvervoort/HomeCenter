/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Lexical;

namespace Canna.Prolog.Runtime.Objects
{
    public class PrologPredicateAttribute : Attribute
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private int _Arity;

        public int Arity
        {
            get { return _Arity; }
            set { _Arity = value; }
        }

        private bool bPublic=false;

        public bool IsPublic
        {
            get { return bPublic; }
            set { bPublic = value; }
        }
    }

    public class PrologClauseAttribute : Attribute
    {
        private Clause _clause;

        public Clause Clause
        {
            get { return _clause; }
            set { _clause = value; }
        }
        private string _clauseStatement;

        public string ClauseStatement
        {
            get { return _clauseStatement; }
            set { _clauseStatement = value;
            Parse();
            }
        }

        private void Parse()
        {
            Tokenizer tokenizer = new Tokenizer(ClauseStatement);
            Parser parser = new Parser(tokenizer);
            _clause = parser.ReadClause();
        }





    }
}
