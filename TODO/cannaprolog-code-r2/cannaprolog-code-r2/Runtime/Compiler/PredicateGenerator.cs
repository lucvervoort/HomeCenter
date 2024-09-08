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
    class PredicateGenerator
    {
        string _predicate;

        ModuleGenerator _mg;

        internal ModuleGenerator ModuleGenerator
        {
            get { return _mg; }
            set { _mg = value; }
        }

        internal string PredicateName
        {
            get { return _predicate; }
            set { _predicate = value; }
        }

        ClausesList _clauses;

        internal ClausesList Clauses
        {
            get { return _clauses; }
            set { _clauses = value; }
        }

        CodeTypeDeclaration _predClass;

        internal string ClassName
        {
            get { return _predClass.Name; }
        }
        int _arity;

        internal static string sMemberArgPrefix = "_arg";
        internal static string sConstructorArgPrefix = "arg";
        internal static string sDerefArgSuffix = "_deref";


        public PredicateGenerator(string predicate, ClausesList clauses, ModuleGenerator mg)
        {
            _predicate = predicate;
            _clauses = clauses;
            _arity = _clauses[0].Head.Arity;
            _mg = mg;
        }

        public CodeTypeDeclaration Generate()
        {
            bool ispublic = true;
            if (_mg.Program.IsModule)
            {
                ispublic = _mg.Program.PublicPredicates.Contains(PredicateName);
            }
            _predClass = CompilerHelper.GetPredicateClass(PredicateName, Clauses[0].Head.Name, Clauses[0].Head.Arity,ispublic);
           
            //add base classes
            _predClass.BaseTypes.Add(typeof(MultiClausePredicate));
            //add members
            for (int arg = 1; arg <= _arity; arg++)
            {
                CodeMemberField field = new CodeMemberField("Term", sMemberArgPrefix + arg.ToString());
                _predClass.Members.Add(field);
            }
            //adds the constructor
            AddPredicateConstructor();
            //add region
            _predClass.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Predicate "+_predicate.Replace("_","/")));
            _predClass.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));

            //add the clauses inner classes
            int i = 1;
            foreach (Clause clause in _clauses)
            {
                ClauseGenerator cg = new ClauseGenerator(this, i++);
                _predClass.Members.Add(cg.Generate());
            }
            //add the getClauses override
            AddGetClausesOverride();
            return _predClass;
        }




        private void AddPredicateConstructor()
        {
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            constructor.Parameters.Add(
        new CodeParameterDeclarationExpression(typeof(IPredicate), "continuation")
);
            constructor.Parameters.Add(
        new CodeParameterDeclarationExpression(typeof(IEngine), "engine")
);

            for (int narg = 1; narg <= _arity; narg++)
            {
                constructor.Parameters.Add(
                        new CodeParameterDeclarationExpression("Term", sConstructorArgPrefix + narg.ToString())
                    );
            }
            constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("continuation"));
            constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("engine"));

            for (int narg = 1; narg <= _arity; narg++)
            {
                CodeVariableReferenceExpression field = new CodeVariableReferenceExpression(sMemberArgPrefix + narg.ToString());
                CodeVariableReferenceExpression arg = new CodeVariableReferenceExpression(sConstructorArgPrefix + narg.ToString());
                CodeMethodInvokeExpression invoke = 
                    new CodeMethodInvokeExpression( arg, "Dereference");
                //changed on 19/06: on construction no dereference
                //CodeAssignStatement assignment = new CodeAssignStatement(field, invoke);
                CodeAssignStatement assignment = new CodeAssignStatement(field, arg);
                constructor.Statements.Add(assignment);
            }
            _predClass.Members.Add(constructor);        }

        private void AddGetClausesOverride( )
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "getClauses";
            method.ReturnType = new CodeTypeReference("IEnumerator");
            method.ReturnType.TypeArguments.Add("IPredicate");
            method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            _predClass.Members.Add(method);

            for (int i = 1; i <= _clauses.Count; ++i)
            {
                StringBuilder literal = new StringBuilder();

                literal.Append("yield return new ");
                literal.Append(_predClass.Name);
                literal.Append("_");
                literal.Append(i);
                literal.Append("(Continuation, Engine");
                for (int j = 1; j <= _arity; ++j)
                {
                    literal.Append(", ");
                    literal.Append(sMemberArgPrefix + j.ToString());
                }
                literal.Append(");");
                CodeSnippetStatement stmt = new CodeSnippetStatement(literal.ToString());
                method.Statements.Add(stmt);
            }
        }



    }
}
