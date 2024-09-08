/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins;

namespace Canna.Prolog.Runtime.Compiler
{
    internal class ModuleGenerator
    {
        PrologProgram _program;

        internal PrologProgram Program
        {
            get { return _program; }
            set { _program = value; }
        }

       public ModuleGenerator(PrologProgram program)
        {
            _program = program;
        }

        public CodeCompileUnit Generate()
        {
            // Create a new CodeCompileUnit to contain 
            // the program graph.
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            // Declare a new namespace
            string namesp = "Canna.Prolog.Global";
            if (_program.Module != null && _program.Module.Length > 0)
                namesp = _program.Module;
            CodeNamespace users = new CodeNamespace(namesp);
            // Add the new namespace to the compile unit.
            compileUnit.Namespaces.Add(users);

            // Add the new namespace import for the System namespace.
            users.Imports.Add(new CodeNamespaceImport("Canna.Prolog.Runtime.Objects"));
            users.Imports.Add(new CodeNamespaceImport("Canna.Prolog.Runtime.Builtins"));

            users.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            users.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));

            foreach (string pred in _program.Keys)
            {
                try
                {
                    //if(PredicateTable.Current.Contains(
                    AddPredicateToNamespace(users, pred);
                }
                catch (Exception ex)
                {
                    //TODO: add to errors and warnings
                    Console.WriteLine(ex.ToString());
                }
            }

            if (_program.InitializationGoals.Count > 0)
            {
                CodeTypeDeclaration program = GetProgramClass();
                users.Types.Add(program);
            }

            return compileUnit;
        }

        private void AddPredicateToNamespace(CodeNamespace users, string pred)
        {
            PredicateInfo info = PredicateTable.Current.GetPredicateInfo(pred);
            if (info != null)
            {
                if (!info.IsPublic)//i.e. builtins
                {
                    throw new PermissionException(Operations.modify, PermissionsTypes.static_procedure, info.PredicateIndicator.GetPITerm(), null);
                }
                else
                {
                    if (info.IsDynamic)
                    {
                        foreach (Clause cl in _program[pred])
                        {
                            info.DynamicPredicate.AppendClause(cl);
                        }
                    }
                    else //not dynamic
                    {
                        if (info.IsMultiFile)
                        {
                            throw new NotImplementedException("Multifile preds are not yet implemented");
                        }
                        else
                        {
                            //Remove predicate from the table in order to redefine it.
                            PredicateTable.Current.RemovePredicate(pred);
                        }
                    }
                }

            }
            {
                PredicateGenerator pg = new PredicateGenerator(pred, _program[pred], this);
                users.Types.Add(pg.Generate());
            }
        }

        private CodeTypeDeclaration GetProgramClass()
        {
            CodeTypeDeclaration program = new CodeTypeDeclaration();
            program.Name = "Program";
            program.Members.Add(GetEntryCode());

            return program;
        }

        private CodeEntryPointMethod GetEntryCode()
        {
            CodeEntryPointMethod start = new CodeEntryPointMethod();
            foreach (Structure goal in _program.InitializationGoals)
            {
                start.Statements.Add(GenerateInitGoal(goal));
            }
            return start;
        }

        private CodeExpression GenerateInitGoal(Structure goal)
        {


            CodeObjectCreateExpression create = new CodeObjectCreateExpression();
            create.CreateType = CompilerHelper.GetPredicateType(goal, _program);
            ClauseVariables var = new ClauseVariables();
            create.Parameters.Add(CompilerHelper.GenerateNull());

            foreach (Term t in goal.Args)
            {
                TermCreationGenerator cpg = new TermCreationGenerator(var, t);
                create.Parameters.Add(cpg.Generate());
            }

            CodeMethodInvokeExpression call = new CodeMethodInvokeExpression();
            call.Method = new CodeMethodReferenceExpression(create, "Call");
            return call;
        }
        
    }
}
