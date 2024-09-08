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
using Canna.Prolog.Runtime.Builtins;
using System.Reflection;
using Canna.Prolog.Runtime.Builtins.Control;

namespace Canna.Prolog.Runtime.Compiler
{
    internal class ContinuationGenerator
    {
        UnificationGenerator _ug;

        internal UnificationGenerator UnificationGenerator
        {
            get { return _ug; }
            set { _ug = value; }
        }

        public ContinuationGenerator(UnificationGenerator ug)
        {
            _ug = ug;
        }

        public CodeExpression Generate()
        {
            CodeExpression continuation = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"Continuation");
            return GenerateGenericContinuation(_ug.Clause.Body, continuation);
        }

        public ClauseVariables Variables
        {
            get { return _ug.Variables; }
        }


        //private CodeExpression GenerateContinuation(SubclausesList body, int n)
        //{
        //    CodeExpression exp;

        //    if (n < body.Count)
        //    {
        //        CodeObjectCreateExpression create;
        //        //create.CreateType = GetPredicateType(body[n]);
        //        Structure f = body[n] as Structure;

        //        if((f != null)&&(f.Name == ";"))
        //        {
        //            create = GenerateDisjunction(f, GenerateContinuation(body, n + 1));
        //        }
        //        else
        //        {
        //            create = GenerateSubclause(body[n], GenerateContinuation(body, n + 1));
        //        }
        //        exp = create;
        //    }
        //    else
        //    {
        //        exp = (new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"continuation"));
        //    }
        //    return exp;
        //}



        private CodeObjectCreateExpression GenerateDisjunction(Structure disjunction, CodeExpression cont)
        {
            Structure left = disjunction[0] as Structure;
            if (left!= null && left. Name== "->")
            {
                return GenerateIfThenElse(left[0],left[1],disjunction[1], cont);
            }
            CodeExpression clause1 = GenerateGenericContinuation(disjunction[0], cont);
            CodeExpression clause2 = GenerateGenericContinuation(disjunction[1], cont);
            return GenerateDisjunction(clause1, clause2, cont) ;
        }

        private CodeObjectCreateExpression GenerateIfThenElse(Term ifgoal, Term thengoal, Term elsegoal, CodeExpression cont)
        {
            CodeObjectCreateExpression create = new CodeObjectCreateExpression();
            create.CreateType = new CodeTypeReference(typeof(IfThenElse));
            create.Parameters.Add(cont);
            create.Parameters.Add(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Engine"));
            create.Parameters.Add(GenerateGenericContinuation(ifgoal, cont));
            create.Parameters.Add(GenerateGenericContinuation(thengoal, cont));
            create.Parameters.Add(GenerateGenericContinuation(elsegoal, cont));

            return create;
        }


        private CodeObjectCreateExpression GenerateDisjunction(CodeExpression clause1, CodeExpression clause2, CodeExpression cont)
        {
            CodeObjectCreateExpression create = new CodeObjectCreateExpression();
            create.CreateType = new CodeTypeReference(typeof(Disjunction));
            create.Parameters.Add(cont);
            create.Parameters.Add(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Engine"));
            create.Parameters.Add(clause1);
            create.Parameters.Add(clause2);
            return create;
        }


        private CodeObjectCreateExpression GenerateIfThen(Term termIf, Term termThen, CodeExpression cont)
        {
            CodeObjectCreateExpression clauseif = new CodeObjectCreateExpression();
            clauseif.CreateType = new CodeTypeReference(typeof(IfThen));
            clauseif.Parameters.Add(GenerateGenericContinuation(termThen, cont));
            clauseif.Parameters.Add(new CodePropertyReferenceExpression( new CodeThisReferenceExpression(), "Engine"));
            clauseif.Parameters.Add(GenerateGenericContinuation(termIf, CompilerHelper.GenerateNull()));
            return clauseif;
        }



        private CodeObjectCreateExpression GenerateSubclause(Term term, CodeExpression cont)
        {
            Structure f = term as Structure;
            CodeObjectCreateExpression create = new CodeObjectCreateExpression();
            create.Parameters.Add(cont);
            create.Parameters.Add(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Engine"));
            if (f != null)
            {
                if (f.Name == "!")
                {
                    create = GenerateCut(cont);
                }
                else
                {
                    try
                    {
                        create.CreateType = CompilerHelper.GetPredicateType(f, this.UnificationGenerator.ClauseGenerator.PredicateGenerator.ModuleGenerator.Program);
                        foreach (Term t in f.Args)
                        {
                            TermCreationGenerator cpg = new TermCreationGenerator(this.Variables, t);
                            create.Parameters.Add(cpg.Generate());
                        }

                    }
                    catch (UnknownPredicateException)
                    {
                        create = UseCall(cont, term);
                    }
                }
            }
            else
            {
                create = UseCall(cont,term);
            }
            return create;
        }

        private CodeObjectCreateExpression UseCall(CodeExpression cont,Term term)
        {
            CodeObjectCreateExpression create = new CodeObjectCreateExpression();
            create.CreateType = new CodeTypeReference(typeof(call_1));
            TermCreationGenerator cpg = new TermCreationGenerator(this.Variables, term);
            create.Parameters.Add(cont);
            create.Parameters.Add(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Engine"));
            create.Parameters.Add(cpg.Generate());
            return create;
        }

        private CodeObjectCreateExpression GenerateConjunction(Structure conj, CodeExpression cont)
        {
            return GenerateGenericContinuation(conj[0], GenerateGenericContinuation(conj[1], cont));
        }

        private CodeObjectCreateExpression GenerateGenericContinuation(Term t, CodeExpression cont)
        {
            CodeObjectCreateExpression create=null;
            Structure f = t as Structure;
            if (f != null)
            {
                if ((f.Name == ";"))
                {
                    create = GenerateDisjunction(f, cont);
                }
                else if (f.Name == ",")
                {
                    create = GenerateConjunction(f, cont);
                }
                else if (f.Name == "->")
                {
                    create = GenerateIfThen(f[0], f[1], cont);
                }
                else
                {
                    create = GenerateSubclause(t, cont);
                }
            }

            else
            {
                create = GenerateSubclause(t, cont);
            }
            return create;
        }

        private CodeObjectCreateExpression GenerateCut(CodeExpression cont)
        {
            CodeObjectCreateExpression create = new CodeObjectCreateExpression();
            create.Parameters.Add(cont);
            create.Parameters.Add(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Engine"));
            create.CreateType = new CodeTypeReference(typeof(cut_0));
            return create;
        }
    }


}
