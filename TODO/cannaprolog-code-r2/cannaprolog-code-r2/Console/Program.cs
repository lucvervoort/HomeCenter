/* *******************************************************************
 * Copyright (c) 2005 - 2008-2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Canna.Prolog.Runtime.Compiler;
using Canna.Prolog.Runtime.Lexical;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins;
using System.Reflection;
using Canna.Prolog.Runtime;
using Canna.Prolog.Runtime.Builtins.Control;
using Canna.Prolog.Runtime.Builtins.ReadWrite;

namespace Canna.Prolog.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Canna Prolog";

            DisplayWelcome();
            Init();
            ReadLoop();
        }



        private static void Init()
        {
            //PrologCompiler.Consult(@"..\..\..\Runtime\Scripts\init.pl");
        }


        private static void ReadQuery()
        {
            Engine engine = Engine.Create();
            Var query = new Var("Query");
            PrologList options = new PrologList();
            Var variable_names = new Var("VarNames");
            options = options.Append(new PrologList(new Structure("variable_names", variable_names)));
            IPredicate pred = new read_term_2(null, engine, query, options);
            Console.Write("?- ");

            if (!engine.ExecuteGoal(pred).IsFailed)
            {
                pred = new call_1(null, engine, query);
                PredicateResult result = engine.ExecuteGoal(pred);

            redo:

                   System.Console.WriteLine();
                if (result.IsFailed)
                {
                    Console.WriteLine("No");
                }
                else
                {
                    PrologList var_names = (PrologList)variable_names.Dereference();
                    bool hasvar = false;

                    while (!var_names.isEmpty())
                    {
                        hasvar = true;
                        Structure var = var_names.Head as Structure;
                        Console.Write("{0} = {1}", ((Structure)var[0]).Name, var[1].ToString());
                        var_names = var_names.Tail as PrologList;
                        if (!var_names.isEmpty())
                        {
                            Console.WriteLine(",");
                        }
                    }
                    
                    foreach (Term var in var_names)
                    {
                        Console.WriteLine(var.ToString());
                        hasvar = true;
                    }
                   
                    if (hasvar)
                    {
                        Console.Write(" ? ");

                        ConsoleKeyInfo key = Console.ReadKey(true);
                        char c = key.KeyChar;
                        if ((c != 'y') && (c != '\r'))
                        {
                            result = engine.Redo();
                            goto redo;
                        }

                    }
                    Console.WriteLine(string.Empty);
                    Console.WriteLine("Yes");
                    
                }
            }
        }

        private static void ReadLoop()
        {
            while (true)
            {
                try
                {
                    ReadQuery();
                }
                catch (PrologException ex)
                {
                    Console.WriteLine("ERROR: {0}",ex.ToString());
                }
            }
        }



        private static void DisplayWelcome()
        {
            Console.WriteLine("Welcome To Canna Prolog Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("Copyright 2005-2008 Gabriele Cannata");
            Console.WriteLine(string.Empty);
        }
    }
}
