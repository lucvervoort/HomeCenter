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
   internal class UnificationGenerator
    {
        ClauseGenerator _clauseGen;

        internal ClauseGenerator ClauseGenerator
        {
            get { return _clauseGen; }
            set { _clauseGen = value; }
        }

        public Clause Clause
        {
            get { return _clauseGen.Clause; ; }
        }
       CodeStatementCollection _lastUnificationPoint = null;

       public CodeStatementCollection LastUnificationPoint
       {
           get { return _lastUnificationPoint; }
           set { _lastUnificationPoint = value; }
       }

       public ClauseVariables Variables
       {
           get { return _clauseGen.Variables; }
       }

       public UnificationGenerator(ClauseGenerator clauseGenerator)
       {
           _clauseGen = clauseGenerator;
       }

       public CodeStatementCollection Generate()
       {
           Structure head = Clause.Head;

           _lastUnificationPoint = new CodeStatementCollection();
           CodeStatementCollection stmts = _lastUnificationPoint;

           //CodeStatement trace = CompilerHelper.GenerateTrace(
           //    new CodePrimitiveExpression("CALLING CLAUSE: " + this.ClauseGenerator.Clause.ToString())
           //    );
           //stmts.Add(trace);

           stmts.Add(GenerateTrace(TraceEventType.Call));

           int ord = 1;
           foreach (Term t in head.Args)
           {
               ord = GenerateArgUnification(stmts, ord, t);
           }
           if (Clause.Body!=null)
           {
               GenerateContinuation();
           }
           else
           {
               //Adds trace
               _lastUnificationPoint.Add(GenerateTrace(TraceEventType.Exit));

           }
           CodeMethodReturnStatement ret = new CodeMethodReturnStatement();
           ret.Expression = new CodeMethodInvokeExpression(
               new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Success"));
           _lastUnificationPoint.Add(ret);

           return stmts;
       }

       private void GenerateContinuation()
       {
           _lastUnificationPoint.AddRange(GenerateVarsDeclaration());
           CodeAssignStatement assignment = new CodeAssignStatement();
           assignment.Left = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
               "Continuation");
           assignment.Right = new ContinuationGenerator(this).Generate();
           _lastUnificationPoint.Add(assignment);

           //Adds trace
           _lastUnificationPoint.Add(GenerateTrace(TraceEventType.Exit));


       }

       private int GenerateArgUnification(CodeStatementCollection stmts, int ord, Term t)
       {
           string argName = PredicateGenerator.sMemberArgPrefix + ord.ToString();
           //CodeVariableDeclarationStatement decl = new CodeVariableDeclarationStatement(typeof(Term), argName);
           //stmts.Add(decl);

           stmts.Add(GenerateArgsDereference(argName));
           //CodeExpression argToUnify = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), argName+PredicateGenerator.sDerefArgSuffix);
           CodeExpression argToUnify = new CodeVariableReferenceExpression(argName + PredicateGenerator.sDerefArgSuffix);
           argToUnify.UserData["name"] = argName;
           TermUnificationGenerator gen = new TermUnificationGenerator(this, argToUnify, t);
           stmts.AddRange(gen.Generate());
           ord++;
           return ord;
       }


       private CodeStatement GenerateArgsDereference(string argName)
       {
           CodeVariableReferenceExpression arg = new CodeVariableReferenceExpression(argName);

           CodeVariableDeclarationStatement derefDecl = new CodeVariableDeclarationStatement(
               typeof(Term),
               argName + PredicateGenerator.sDerefArgSuffix,
               new CodeMethodInvokeExpression(arg, "Dereference"));

               //CodeAssignStatement  assign = new CodeAssignStatement();
               //assign.Left = arg;
               //assign.Right = new CodeMethodInvokeExpression(arg, "Dereference");
           return derefDecl;
       }

       private CodeStatementCollection GenerateVarsDeclaration()
       {
           CodeStatementCollection stmts = new CodeStatementCollection();
           VarList variables=new VarList();
           variables.AddRange(Clause.Body.GetFreeVariables());
           foreach (Var var in variables)
           {
               stmts.Add (Variables.DeclareVariable(var));
               
           }
           return stmts;
       }

       private CodeStatement GenerateTrace( TraceEventType evt)
       {
           CodeConditionStatement ifthen = new CodeConditionStatement();
           //TODO:ADD cndition
           CodeFieldReferenceExpression sw = new CodeFieldReferenceExpression();
           sw.TargetObject = new CodeTypeReferenceExpression(typeof(BasePredicate));
           sw.FieldName = "_predicateSwitch";
           CodePropertyReferenceExpression enabled = new CodePropertyReferenceExpression(sw, "Enabled");
           ifthen.Condition = enabled;
           CodeMethodInvokeExpression mi = new CodeMethodInvokeExpression();
           mi.Method = new CodeMethodReferenceExpression();
           mi.Method.TargetObject = new CodeThisReferenceExpression();
           mi.Method.MethodName = "TraceEvent";
           mi.Parameters.Add(new CodeFieldReferenceExpression( new CodeTypeReferenceExpression(typeof(TraceEventType)), evt.ToString()));
           int ord = 1;
           foreach (Term t in Clause.Head.Args)
           {
               string argName = PredicateGenerator.sMemberArgPrefix + ord.ToString();
               CodeVariableReferenceExpression arg = new CodeVariableReferenceExpression(argName);
               mi.Parameters.Add(new CodeMethodInvokeExpression(arg, "Dereference"));
               ord++;
           }

           ifthen.TrueStatements.Add(new CodeExpressionStatement(mi));
           return ifthen;
       }
     
    }

    class TermUnificationGenerator : ITermVisitor
    {
        protected UnificationGenerator _ug;
        protected CodeExpression  _argToUnify;
        protected Term _term;
        protected CodeStatementCollection _stmts = new CodeStatementCollection();
        public TermUnificationGenerator(UnificationGenerator ug, CodeExpression argToUnify, Term term)
        {
            _ug = ug;
            _argToUnify = argToUnify;
            _term = term;
        }

        public virtual CodeStatementCollection Generate()
        {
            _term.Accept(this);
            return _stmts;
        }

        public CodeExpression GetBoundedVariables()
        {
            CodePropertyReferenceExpression engine = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Engine");
            return new CodePropertyReferenceExpression(engine, "BoundedVariables");
        }
       

        #region ITermVisitor Members

        private CodeStatementCollection GenerateUnificationCondition(CodeExpression condition)
        {
            CodeStatementCollection stmts = new CodeStatementCollection();
            CodeConditionStatement ifcond = new CodeConditionStatement();
            ifcond.Condition = new CodeBinaryOperatorExpression(
                            condition,
                        CodeBinaryOperatorType.ValueEquality,
                        new CodePrimitiveExpression(false)
                );

            ifcond.TrueStatements.Add(CompilerHelper.GenerateReturn(Result.Failed));
            stmts.Add(ifcond);
            return stmts;
        }

        public void VisitVar(Var var)
        {
            CodeCommentStatement comment = new CodeCommentStatement("Test if " + _argToUnify.UserData["name"] + " unify with variable " + var.ToString());
            _stmts.Add(comment);
            _stmts.Add(_ug.Variables.DeclareVariable(var));
            CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(
                _argToUnify, "UnifyWithVar",
                _ug.Variables.GetVar(var),
                GetBoundedVariables(),
                new CodePrimitiveExpression(false));//no occur check
            _stmts.AddRange(GenerateUnificationCondition(invoke));
        }

 

        public void VisitInteger(Integer integer)
        {
            CodeCommentStatement comment = new CodeCommentStatement("Test if " + _argToUnify.UserData["name"] + " unify with integer " + integer.ToString());
            _stmts.Add(comment);
            CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(
                _argToUnify, "UnifyWithInteger",
                new CodeObjectCreateExpression(typeof(Integer),new CodePrimitiveExpression(integer.Value)),
                GetBoundedVariables(),
                new CodePrimitiveExpression(false)
                );
            _stmts.AddRange(GenerateUnificationCondition(invoke));

        }

        public void VisitFloat(Floating floating)
        {

            CodeCommentStatement comment = new CodeCommentStatement("Test if " + _argToUnify.UserData["name"] + " unify with floating " + floating.ToString());
            _stmts.Add(comment);
            CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(
                _argToUnify, "UnifyWithFloating",
                new CodeObjectCreateExpression(typeof(Floating), new CodePrimitiveExpression(floating.Value)),
                GetBoundedVariables(),
                new CodePrimitiveExpression(false));
            _stmts.AddRange(GenerateUnificationCondition(invoke));

          
        }

        public void VisitStruct(Structure structure)
        {
            
           // structure.getFreeVariables(
            VarList variables = structure.GetFreeVariables();
            foreach (Var v in variables)
            {
                _stmts.Add(_ug.Variables.DeclareVariable(v));
            }
            CodeVariableDeclarationStatement declTemplate = new CodeVariableDeclarationStatement();

            declTemplate.Type = new CodeTypeReference(typeof(Structure));
            declTemplate.Name = _argToUnify.UserData["name"].ToString() + "Template";
            declTemplate.InitExpression = GenerateFunctorCreation(structure);
            _stmts.Add(declTemplate);
            CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(
        _argToUnify, "UnifyWithStructure",
        new CodeVariableReferenceExpression(declTemplate.Name),
        GetBoundedVariables(),
        new CodePrimitiveExpression(false)
        );
            _stmts.AddRange(GenerateUnificationCondition(invoke));

           
        }

        public void VisitList(PrologList list)
        {
            VarList variables = list.GetFreeVariables();
            foreach (Var v in variables)
            {
                _stmts.Add(_ug.Variables.DeclareVariable(v));
            }
            CodeVariableDeclarationStatement declTemplate = new CodeVariableDeclarationStatement();

            declTemplate.Type = new CodeTypeReference(typeof(PrologList));
            declTemplate.Name = _argToUnify.UserData["name"].ToString() + "Template";
            declTemplate.InitExpression = GenerateFunctorCreation(list);
            _stmts.Add(declTemplate);
            CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(
        _argToUnify, "UnifyWithStructure",
        new CodeVariableReferenceExpression(declTemplate.Name),
        GetBoundedVariables(),
        new CodePrimitiveExpression(false)
        );
            _stmts.AddRange(GenerateUnificationCondition(invoke));
        }

        private CodeStatement CheckFunctorAndVar(Structure structure )
        {
            CodeConditionStatement ifVar = new CodeConditionStatement();
            CodeTypeOfExpression typeOf = new CodeTypeOfExpression(typeof(Var));

            CodeMethodInvokeExpression mi1 = new CodeMethodInvokeExpression();
            mi1.Method.TargetObject = typeOf;
            mi1.Method.MethodName = "IsInstanceOfType";
            mi1.Parameters.Add(_argToUnify);

            ifVar.Condition = mi1;
            ifVar.FalseStatements.Add(CompilerHelper.GenerateReturn(Result.Failed));
            return ifVar;
        }

        private CodeVariableDeclarationStatement DeclareFunctorCast()
        {
            CodeVariableDeclarationStatement decl = new CodeVariableDeclarationStatement();
            decl.Type = new CodeTypeReference(typeof(Structure));
            decl.Name = _argToUnify.UserData["name"].ToString() + "Structure";

            decl.InitExpression = new CodeCastExpression(
                typeof(Structure),
                _argToUnify);
            return decl;
        }

        private CodeExpression GenerateFunctorCreation(Structure f)
        {
            TermCreationGenerator gen = new TermCreationGenerator(_ug.Variables, f);
            return gen.Generate();
        }

        private static CodeBinaryOperatorExpression CheckNameAndArity(Structure structure, CodeVariableReferenceExpression _argFunctor)
        {
            CodeBinaryOperatorExpression name = new CodeBinaryOperatorExpression();
            name.Operator = CodeBinaryOperatorType.ValueEquality;
            name.Left = new CodePropertyReferenceExpression(_argFunctor, "Name");
            name.Right = new CodePrimitiveExpression(structure.Name);

            CodeBinaryOperatorExpression arity = new CodeBinaryOperatorExpression();
            arity.Operator = CodeBinaryOperatorType.ValueEquality;
            arity.Left = new CodePropertyReferenceExpression(_argFunctor, "Arity");
            arity.Right = new CodePrimitiveExpression(structure.Arity);


            CodeBinaryOperatorExpression nameandarity = new CodeBinaryOperatorExpression();
            nameandarity.Operator = CodeBinaryOperatorType.BooleanAnd;
            nameandarity.Left = arity;
            nameandarity.Right = name;
            return nameandarity;
        }

       
        #endregion

  
}

}
