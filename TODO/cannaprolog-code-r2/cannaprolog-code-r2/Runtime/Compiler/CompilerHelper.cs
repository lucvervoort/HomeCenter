/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Canna.Prolog.Runtime.Objects;
using System.Reflection;
using System.Diagnostics;
using Microsoft.CSharp;

namespace Canna.Prolog.Runtime.Compiler
{
    class CompilerHelper
    {
        private static Dictionary<char, string> _symbols;

        static CompilerHelper()
        {
            _symbols = new Dictionary<char, string>();
            _symbols.Add('|', "pipe");
            _symbols.Add('\\', "backslash");
            _symbols.Add('!', "exclam");
            _symbols.Add('"', "dquote");
            _symbols.Add('\'', "quote");
            _symbols.Add('$', "dollar");
            _symbols.Add('%', "percent");
            _symbols.Add('&', "ampersand");
            _symbols.Add('/', "slash");
            _symbols.Add('=',"equal");
            _symbols.Add('?', "question");
            _symbols.Add('-', "dash");
            _symbols.Add('_', "underscore");
            _symbols.Add(',', "comma");
            _symbols.Add(';', "semicolon");
            _symbols.Add('.', "dot");
            _symbols.Add(':',"colon");
            _symbols.Add('<',"lessthan");
            _symbols.Add('>',"greaterthan");
            _symbols.Add('*', "star");
            _symbols.Add('+', "plus");


        }

        public static CodeMethodReturnStatement GenerateReturn(Result result)
        {
            CodeFieldReferenceExpression ex = new CodeFieldReferenceExpression(
                new CodeTypeReferenceExpression(typeof(PredicateResult)), result.ToString());
            return new CodeMethodReturnStatement(ex);
        }


        public static CodeTypeReference GetPredicateType(Structure f, PrologProgram program)
        {
            //is this inside the Program we're compiling?
            if (program.ContainsKey(f))
            {
                return new CodeTypeReference(f.GetPI().GetPredicateName());
            }


            Type t = PredicateTable.Current.GetPredicateType(f.GetPI());
            if (t == null)
            {
                throw new UnknownPredicateException(f.GetPI());
            }
            return new CodeTypeReference(t);
        }

        public static CodeStatement GenerateTrace(CodeExpression exp)
        {
            CodeMethodReferenceExpression write = new CodeMethodReferenceExpression();
            write.TargetObject = new CodeTypeReferenceExpression(typeof(Trace));
            write.MethodName = "WriteLineIf";
            
            
            
            CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression();
            invoke.Method = write;
            invoke.Parameters.Add(new CodeFieldReferenceExpression(
                new CodeFieldReferenceExpression(
                new CodeTypeReferenceExpression(typeof(MultiClausePredicate)), "_predicateSwitch"), "Enabled"));
            invoke.Parameters.Add(exp);
            return new CodeExpressionStatement( invoke);
        }

        public static bool IsValidIdentifier(string name)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            return provider.IsValidIdentifier(name);
        }

        public static string GetValidIdentifier(string name)
        {
            StringBuilder sb = new StringBuilder();
            if (Char.IsLetter(name[0]) || name[0] == '_')
            {
                sb.Append(name[0]);
            }
            else
            {
                sb.Append(ReplaceChar(name[0]));
            }
            for (int i = 1; i < name.Length; ++i)
            {
                char c = name[i];
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(ReplaceChar(c));
                }
            }
            return sb.ToString();
        }

        

        private static string ReplaceChar(char c)
        {
            if (_symbols.ContainsKey(c))
                return _symbols[c];
            return "_";
        }

        public static CodeTypeDeclaration GetPredicateClass(string predname,string name, int arity, bool ispublic)
        {
            string className = GetClassName(predname);
            CodeTypeDeclaration _predClass = new CodeTypeDeclaration(className);
            {
                CodeAttributeDeclaration attr = new CodeAttributeDeclaration(new CodeTypeReference(typeof(PrologPredicateAttribute)));
                attr.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(name)));
                attr.Arguments.Add(new CodeAttributeArgument("Arity", new CodePrimitiveExpression(arity)));
                attr.Arguments.Add(new CodeAttributeArgument("IsPublic", new CodePrimitiveExpression(ispublic)));
                _predClass.CustomAttributes.Add(attr);
                if (!ispublic)
                    _predClass.TypeAttributes = TypeAttributes.NotPublic;
            }
            return _predClass;
        }
        public static CodeTypeDeclaration GetClauseClass(string predname, string name, int arity)
        {
            string className = GetClassName(predname);
            CodeTypeDeclaration _predClass = new CodeTypeDeclaration(className);
            {
                CodeAttributeDeclaration attr = new CodeAttributeDeclaration(new CodeTypeReference(typeof(PrologPredicateAttribute)));
                attr.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(name)));
                attr.Arguments.Add(new CodeAttributeArgument("Arity", new CodePrimitiveExpression(arity)));
                attr.Arguments.Add(new CodeAttributeArgument("IsPublic", new CodePrimitiveExpression(false)));
                _predClass.CustomAttributes.Add(attr);
                _predClass.TypeAttributes = TypeAttributes.NestedPrivate;
            }
            return _predClass;
        }
        private static string GetClassName(string predname)
        {
            if (CompilerHelper.IsValidIdentifier(predname))
            {
                return predname;
            }
            else
            {
                return GetValidIdentifier(predname);
            }
        }

        public static CodeExpression GenerateNull()
        {
            return new CodePrimitiveExpression(null);
        }
    }
}
