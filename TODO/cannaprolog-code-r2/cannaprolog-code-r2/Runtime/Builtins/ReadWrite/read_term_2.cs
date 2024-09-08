/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Lexical;
using System.IO;
using Canna.Prolog.Runtime.Builtins.IO;

namespace Canna.Prolog.Runtime.Builtins.ReadWrite
{
    [PrologPredicate(Name = "read_term", Arity = 3)]
    public class read_term_3 : StreamBasePredicate
    {
        protected Term _term, _options,_ioptions;
        private Variables _varlist;
        private PrologList _variables;
        
        public read_term_3(IPredicate continuation, IEngine engine, Term s_or_a, Term term,Term options)
            : base(continuation, engine, s_or_a)
        {
            _term = term;
            _ioptions = options;
            
        }

        public override PredicateResult Call()
        {
            _term = _term.Dereference();
            _options = _ioptions.Dereference();
            _stream = _istream.Dereference();

            if (!_options.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _options, this);
            }
            StreamTerm stream = GetStream();
            return InternalReadTerm(stream);
        }

        protected PredicateResult InternalReadTerm(StreamTerm stream)
        {
            StreamReader sr = new StreamReader(stream.Stream);
            Parser parser = new Parser(new Tokenizer(sr));
            //Term t = parser.ReadTerm(1200);
            Term t = parser.ReadTerm();
            _varlist = new Variables();
            // _varlist.keepnames = true;
            Term t2 = t.Copy(_varlist);
            _variables = _varlist.ToPrologList();

            if (!_term.Unify(t2, Engine.BoundedVariables, false))
            {
                return Fail();
            }


            PrologList options = _options as PrologList;
            if (!ApplyOption(options))
            {
                return Fail();
            }

            return Success();
        }

        private bool ApplyOption(PrologList options)
        {
            if (options.isEmpty()) return true;

            Structure option = options.Head as Structure;
            if (option == null)
            {

                throw new DomainException(ValidDomains.read_option, options.Head, this);
            }
            switch (option.Name)
            {
                case "variables":
                    if (option.Arity == 1)
                    {
                        if (!option[0].Unify(_variables, Engine.BoundedVariables, false))
                        {
                            return false;
                        }
                    }
                    break;
                case "variable_names":
                    if (option.Arity == 1)
                    {
                        PrologList varnames = GetVarNames();
                        if (!option[0].Unify(varnames, Engine.BoundedVariables, false))
                        {
                            return false;
                        }
                    }
                    break;
                case "singletons":
                    if (option.Arity == 1)
                    {
                        PrologList singletons = GetSingletons();
                        if (!option[0].Unify(singletons, Engine.BoundedVariables, false))
                        {
                            return false;
                        }
                    }
                    break;
            }

            PrologList tail = options.Tail as PrologList;
            if (tail == null) throw new TypeMismatchException(ValidTypes.List, options.Tail, this);
            return ApplyOption(tail);
        }

        private PrologList GetVarNames()
        {
            PrologList plist = new PrologList();

            foreach (KeyValuePair<string, Var> pair in _varlist)
            {
                Structure eq = new Structure("=", new Structure(pair.Key), pair.Value);
                plist = plist.Append(new PrologList(eq));

            }
            return plist;
        }

        private PrologList GetSingletons()
        {
            PrologList plist = new PrologList();

            foreach (KeyValuePair<string, Var> entry in _varlist)
            {
                if (!entry.Value.Anonymous)
                {
                    if (entry.Value.NRef == 1)
                    {
                        Structure eq = new Structure("=", new Structure(entry.Key), entry.Value);
                        plist = plist.Append(new PrologList(eq));
                    }
                }

            }
            return plist;
        }

    }


    [PrologPredicate(Name = "read_term", Arity = 2)]
    public class read_term_2 : read_term_3
    {

        public read_term_2(IPredicate continuation, IEngine engine, Term term, Term opts)
            : base(continuation,engine,null,term,opts)
        {
        }

        public override PredicateResult Call()
        {
            _term = _term.Dereference();
            _options = _ioptions.Dereference();
            StreamTerm stream = StreamTerm.CurrentInput;

            if (!_options.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _options, this);
            }

            return InternalReadTerm(stream);
        }

   
    }

    [PrologPredicate(Name = "read", Arity = 2)]
    public class read_2 : read_term_3
    {

        public read_2(IPredicate continuation, IEngine engine, Term stream, Term term)
            : base(continuation, engine, stream, term, new PrologList())
        {
        }

        

    }

    [PrologPredicate(Name = "read", Arity = 1)]
    public class read_1 : read_term_2
    {

        public read_1(IPredicate continuation, IEngine engine, Term term)
            : base(continuation, engine, term, new PrologList())
        {
        }



    }
}
