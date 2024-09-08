/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Diagnostics;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Compiler
{
    internal class TermCreationGenerator : ITermVisitor
    {
        //protected ContinuationGenerator _cg;
        private Term _term;
        private CodeExpression exp;
        private ClauseVariables _vars;
        //protected int _ord;
        public TermCreationGenerator(ClauseVariables vars, Term term)
        {
            _vars = vars;
            _term = term;
            exp = new CodePrimitiveExpression(null);
        }

        public virtual CodeExpression Generate()
        {
            this._term.Accept(this);
            return exp;
        }

        #region ITermVisitor Members

        public void VisitVar(Var var)
        {
            exp = _vars.GetVar(var);
       
        }

        public void VisitInteger(Integer integer)
        {
            CodeObjectCreateExpression creation = new CodeObjectCreateExpression();
            creation.CreateType = new CodeTypeReference(typeof(Integer));
            creation.Parameters.Add(new CodePrimitiveExpression(integer.Value));
            exp = creation;
        }

        public void VisitFloat(Floating floating)
        {
            CodeObjectCreateExpression creation = new CodeObjectCreateExpression();
            creation.CreateType = new CodeTypeReference(typeof(Floating));
            creation.Parameters.Add(new CodePrimitiveExpression(floating.Value));
            exp = creation;
        }

        public void VisitStruct(Structure structure)
        {
            CodeObjectCreateExpression creation = new CodeObjectCreateExpression();
            creation.CreateType = new CodeTypeReference(typeof(Structure));
            creation.Parameters.Add(new CodePrimitiveExpression(structure.Name));
            foreach (Term t in structure.Args)
            {
                TermCreationGenerator gen = new TermCreationGenerator(_vars, t);
                creation.Parameters.Add(gen.Generate());
            }
            exp = creation;
        }


        public void VisitList(PrologList list)
        {
            CodeObjectCreateExpression creation = new CodeObjectCreateExpression();
            creation.CreateType = new CodeTypeReference(typeof(PrologList));
            foreach (Term t in list.Args)
            {
                TermCreationGenerator gen = new TermCreationGenerator(_vars, t);
                creation.Parameters.Add(gen.Generate());
            }
            exp = creation;

        }

        #endregion
}

}
