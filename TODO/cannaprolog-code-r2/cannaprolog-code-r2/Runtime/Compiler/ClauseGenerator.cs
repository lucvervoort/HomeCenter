/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Compiler
{
    class ClauseGenerator
    {
        PredicateGenerator _pg;

        internal PredicateGenerator PredicateGenerator
        {
            get { return _pg; }
            set { _pg = value; }
        }
        int _ord;
        Clause _clause;

        public Clause Clause
        {
            get { return _clause; }
            set { _clause = value; }
        }
        CodeTypeDeclaration _clauseClass;
        ClauseVariables _variables = new ClauseVariables();
        //TODO: queste andrebbero nella ClauseGenerator, che dovrebbe fungere
        //da contesto per UnifyGenerator e ContinuationGenerator
        public ClauseVariables Variables
        {
            get { return _variables; }
            set { _variables = value; }
        }

        public ClauseGenerator(PredicateGenerator pg, int ord)
        {
            _pg = pg;
            _ord = ord;
            _clause = pg.Clauses[ord - 1];
        }

        public CodeTypeDeclaration Generate()
        {

            string name = _clause.Head.GetPI().GetPredicateName() + "_" + _ord.ToString();

            _clauseClass = CompilerHelper.GetClauseClass(name, _clause.Head.Name, _clause.Head.Arity);

            CodeTypeReference basetype = new CodeTypeReference(_pg.ClassName);
            _clauseClass.BaseTypes.Add(basetype);

            _clauseClass.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Clause " + _clause.ToString(new WriteOptions(true, false, true))));
            //_clauseClass.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Clause " + _ord.ToString()));
            _clauseClass.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));

            AddClausesTypeContructor();
            AddCallMethod();
            //AddRedoMethod();
            AddClauseAttribute();
            return _clauseClass;
        }

        private void AddClauseAttribute()
        {
            CodeAttributeDeclaration attr = new CodeAttributeDeclaration(new CodeTypeReference(typeof(PrologClauseAttribute)));
            attr.Arguments.Add(new CodeAttributeArgument(
                "ClauseStatement", new CodePrimitiveExpression(_clause.ToString(new WriteOptions(true,false,true)))));

            _clauseClass.CustomAttributes.Add(attr);

        }

        private void AddClausesTypeContructor()
        {
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            constructor.Parameters.Add(
        new CodeParameterDeclarationExpression(typeof(IPredicate), "continuation")
);
            constructor.Parameters.Add(
new CodeParameterDeclarationExpression(typeof(IEngine), "engine")
);
            constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("continuation"));
            constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("engine"));

            for (int narg = 1; narg <= _clause.Head.Arity; narg++)
            {
                constructor.Parameters.Add(
                        new CodeParameterDeclarationExpression("Term", PredicateGenerator.sConstructorArgPrefix + narg.ToString())
                    );
                constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(PredicateGenerator.sConstructorArgPrefix + narg.ToString()));

            }
            _clauseClass.Members.Add(constructor);
        }

        private void AddRedoMethod()
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "redo";
            method.ReturnType = new CodeTypeReference("PredicateResult");
            method.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            _clauseClass.Members.Add(method);
            if (_clause.Body != null)
            {
                CodeVariableReferenceExpression arg = new CodeVariableReferenceExpression("Continuation");
                CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(), "RedoContinuation");
                CodeMethodReturnStatement ret = new CodeMethodReturnStatement(invoke);
                method.Statements.Add(ret);
            }
            else
            {
                CodeFieldReferenceExpression ex = new CodeFieldReferenceExpression(
                    new CodeTypeReferenceExpression(typeof(PredicateResult)), "Failed");
                CodeMethodReturnStatement ret = new CodeMethodReturnStatement(ex);
                method.Statements.Add(ret);
            }

        }

        private void AddCallMethod()
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Call";
            method.ReturnType = new CodeTypeReference("PredicateResult");
            method.Attributes = MemberAttributes.Public | MemberAttributes.Override;

            _clauseClass.Members.Add(method);

            UnificationGenerator ug = new UnificationGenerator(this);
            method.Statements.AddRange(ug.Generate());

            //CodeFieldReferenceExpression ex = new CodeFieldReferenceExpression(
            //        new CodeTypeReferenceExpression(typeof(PredicateResult)), "Failed");
            //CodeMethodReturnStatement ret = new CodeMethodReturnStatement(ex);
            //method.Statements.Add(ret);
        }

    }

    class ClauseVariables : Dictionary<string, string>
    {
        int i = 1;
        public CodeStatement DeclareVariable(Var var)
        {
            CodeStatement stmt = new CodeCommentStatement("Don't need to declare var " + var.ToString());
            if (ContainsKey(var.Name) || var.Name == "_")
            {

            }
            else
            {
                //was:
                //CodeObjectCreateExpression initex = new CodeObjectCreateExpression(typeof(Var), new CodePrimitiveExpression(var.Name));
                CodeObjectCreateExpression initex = new CodeObjectCreateExpression(typeof(Var));

                string varname = "var" + var.Name;

                CodeVariableDeclarationStatement decl = new CodeVariableDeclarationStatement(
                    typeof(Var), varname, initex);
                stmt = (decl);

                Add(var.Name, "var" + var.Name);
            }
            return stmt;
        }



        public CodeExpression GetVar(Var var)
        {
            CodeExpression exp = null;
            if (ContainsKey(var.Name))
            {
                exp = new CodeVariableReferenceExpression(this[var.Name]);
            }
            else
            {//we assume it is "_"
                exp = new CodeObjectCreateExpression(typeof(Var), new CodePrimitiveExpression("var" + (i++).ToString()));
            }
            return exp;
        }


    }
}
