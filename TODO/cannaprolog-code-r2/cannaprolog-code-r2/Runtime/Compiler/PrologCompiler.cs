/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.IO;
using System.Diagnostics;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Canna.Prolog.Runtime.Lexical;
using Canna.Prolog.Runtime.Objects;
using System.Reflection;
using Canna.Prolog.Runtime.Builtins;
using Canna.Prolog.Runtime.Builtins.Control;

namespace Canna.Prolog.Runtime.Compiler
{
    public class PrologCompiler
    {
        PrologProgram prog;
        CodeCompileUnit ccu;
        string _currentPath;

        private PrologCompiler()
        {
        }

        public static void Consult(string filename)
        {
            PrologCompiler pc = new PrologCompiler();
            pc.InternalConsult(filename);
        }

        public static string PrologToCSharp(string prologFile, string assemblyName)
        {
            PrologCompiler pc = new PrologCompiler();
            return pc.GenerateCSharpCodeFromText(prologFile, assemblyName);
        }

        private void InternalConsult(string prologFile)
        {
            string assemblyName = GetAssemblyName(prologFile);
            if (AssemblyCache.Current.ContainsKey(assemblyName))
            {
                PredicateTable.Current.RemoveAssembly(AssemblyCache.Current[assemblyName]);
                AssemblyCache.Current.Remove(assemblyName);
            }
            prog = new PrologProgram(assemblyName);
            _currentPath = Path.GetDirectoryName(Path.GetFullPath(prologFile)) ;
            string normalizedFileName = NormalizeFileName(Path.GetFileName(prologFile));

            ParseFile(normalizedFileName,true);
            GenerateCodeGraph();
            //GenerateCSharpCode(assemblyName);
            //prog = new PrologProgram(assemblyName);
            Assembly ass = Compile(assemblyName);
            
            if (ass != null)
            {
                AssemblyCache.Current.Add(assemblyName, ass, normalizedFileName);
                PredicateTable.Current.AddAssembly(AssemblyCache.Current[assemblyName]);
                Initialize(ass);
            }
        }

        private string GenerateCSharpCodeFromText(string prologFile, string assemblyName)
        {
            StreamReader sr = new StreamReader(prologFile);
            StringBuilder output = new StringBuilder();
            StringWriter sw = new StringWriter(output);
            prog = new PrologProgram(assemblyName);
            _currentPath = Path.GetDirectoryName(Path.GetFullPath(prologFile));

            Parse(sr, false);
            GenerateCodeGraph();
            CSharpCodeProvider provider = new CSharpCodeProvider();
            provider.GenerateCodeFromCompileUnit(ccu, sw, new CodeGeneratorOptions());

            return output.ToString();
        }

        private void Initialize(Assembly ass)
        {
            if (ass.EntryPoint != null)
            {
                ass.EntryPoint.Invoke(null, null);
            }
            
        }

        private string NormalizeFileName(string file)
        {
            string extension = Path.GetExtension(file);

            if (extension.Length == 0)
            {
                file = Path.ChangeExtension(file, ".pl");
            }

           
            if (!Path.IsPathRooted(file) && _currentPath.Length > 0)
            {
                file = Path.Combine(_currentPath, file);
            }

            return file;
        }

        private void ParseFile(String file, bool execDirective)
        {
            file = NormalizeFileName(file);
            StreamReader sr = new StreamReader(file);
            try
            {
                Parse(sr,execDirective);
            }
            finally
            {
                sr.Close();
            }
        }

        //public IPredicate CompileQuery(string query)
        //{
        //    query = "goal :- " + query;
        //    PrologTokenizer tokenizer = new PrologTokenizer(query);
        //    PrologParser parser = new PrologParser(tokenizer);
        //    Clause qc = parser.getClause();
        //    prog = new PrologProgram("query");
        //    prog.Add(qc);
        //    GenerateCodeGraph();
        //    GenerateCSharpCode("query");
        //    Assembly ass = Compile("query");
        //    Type t = ass.GetType("query.goal_0");
        //    IPredicate ipred = Activator.CreateInstance(t, new object[] { null }) as IPredicate;

        //    return ipred;
        //}

        private string GetAssemblyName(string filename)
        {
            string assemblyname = Path.GetFileNameWithoutExtension(filename);
            return assemblyname;
        }

        private bool Parse(TextReader txt, bool execDirective)
        {
            Tokenizer tokenizer = new Tokenizer(txt);
            //PrologParser parser = new PrologParser(tokenizer);
            Lexical.Parser parser = new Lexical.Parser(tokenizer);
            
            Clause c;
            try
            {
                c = parser.ReadClause();
                while (c != null)
                {
                    if (c.Head != null)
                    {
                        prog.Add(c);
                    }
                    else
                    {
                        ExecuteDirective(c.Body, execDirective);
                    }

                    c = parser.ReadClause();
                }
            }
            catch (PrologException e)
            {
                throw e;
            }

            return true;
        }

        private void ExecuteDirective(Structure structure, bool execute)
        {
            if (structure.Name == "include")
            {
                Include(structure[0]);
                return;
            }
            if (structure.Name == "module")
            {
                MakeModule(structure);
                return;
            }
            if (!execute) return;
            if (structure.Name == "initialization")
            {
                AddInitGoal(structure[0]);
                return;
            }
            try
            {
                Engine eng = Engine.Create();
                eng.ExecuteGoal(new call_1(null, eng, structure));
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void MakeModule(Structure structure)
        {
            Structure strModName = structure[0] as Structure;
            if (strModName == null || !strModName.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, structure[0], null);
            }
            prog.Module = strModName.Name;
            PrologList list = structure[1] as PrologList;
            if (list == null)
            {
                throw new TypeMismatchException(ValidTypes.List, structure[1], null);
            }
            foreach (Term t in list)
            {
                PredicateIndicator pi = PredicateIndicator.FromTerm(t, null);
                prog.PublicPredicates.Add(pi.GetPredicateName());
            }
        }

        private void AddInitGoal(Term term)
        {
            Structure str = term as Structure;
            if (str == null)
            {
                throw new TypeMismatchException(ValidTypes.Callable, term, null);
            }
            this.prog.InitializationGoals.Add(str);

        }

        private void Include(Term term)
        {
            Structure s = term as Structure;
            if (s == null)
            {
                throw new TypeMismatchException(ValidTypes.Atom, term, null);
            }
            ParseFile(s.Name,true);
        }

        private void GenerateCodeGraph()
        {
            ModuleGenerator mg = new ModuleGenerator(prog);
            ccu = mg.Generate();
        }
        public void GenerateCSharpCode(string filename)
        {
            GenerateCSharpCode(ccu, filename);
        }
        private string GenerateCSharpCode(CodeCompileUnit compileunit, string filename)
        {
            // Generate the code with the C# code provider.
            CSharpCodeProvider provider = new CSharpCodeProvider();

            // Obtain an ICodeGenerator from the CodeDomProvider class.
            //ICodeGenerator gen = provider.CreateGenerator();

            // Build the output file name.
            String sourceFile;
            if (provider.FileExtension[0] == '.')
            {
                sourceFile = filename + provider.FileExtension;
            }
            else
            {
                sourceFile = filename + "." + provider.FileExtension;
            }

            // Create a TextWriter to a StreamWriter to the output file.
            IndentedTextWriter tw = new IndentedTextWriter(
                    new StreamWriter(sourceFile, false), "    ");

            // Generate source code using the code generator.
            provider.GenerateCodeFromCompileUnit(compileunit, tw,
                    new CodeGeneratorOptions());

            // Close the output file.
            tw.Close();

            return sourceFile;

        }

        private Assembly Compile(string assemblyName)
        {
            // Generate the code with the C# code provider.
            Assembly ret = null;
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters par = GetCompileParameters();
            
            //TODO: import only needed assemblies
            foreach (PrologTextInfo pti in AssemblyCache.Current.Values)
            {
                Assembly ass = pti.Assembly;
                par.ReferencedAssemblies.Add(ass.ManifestModule.ScopeName);
            }
            par.OutputAssembly = assemblyName+".dll";
            CompilerResults cr = provider.CompileAssemblyFromDom(par, ccu);
            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}",
                    assemblyName, (cr.PathToAssembly == null ? "memory" : cr.PathToAssembly));
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("{0} compiled into {1} successfully.",
                    assemblyName, (cr.PathToAssembly==null?"memory":cr.PathToAssembly));
                ret = cr.CompiledAssembly;
            }
            provider.Dispose();
            return ret;
        }

        private CompilerParameters GetCompileParameters()
        {
            CompilerParameters par = new CompilerParameters();
            par.GenerateInMemory = true;
            par.IncludeDebugInformation = false;
            par.CompilerOptions = "/o+";
            par.ReferencedAssemblies.Add("Canna.Prolog.Runtime.dll");
            par.ReferencedAssemblies.Add("System.dll");
            par.GenerateExecutable = prog.InitializationGoals.Count > 0;
            return par;
        }

    }
}
