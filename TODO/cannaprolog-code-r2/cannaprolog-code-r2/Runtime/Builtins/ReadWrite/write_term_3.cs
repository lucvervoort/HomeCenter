/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using System.IO;
using Canna.Prolog.Runtime.Builtins.IO;

namespace Canna.Prolog.Runtime.Builtins.ReadWrite
{
    /// <summary>
    /// Summary for write_term_3
    /// </summary>
    [PrologPredicate(Name = "write_term", Arity = 3)]
    public class write_term_3 : StreamBasePredicate
    {
        protected Term _term, _iterm, _options,_ioptions;

        public write_term_3(IPredicate continuation, IEngine engine, Term s_or_a, Term term,Term options)
            : base(continuation, engine, s_or_a)
        {
            _iterm = term;
            _ioptions = options;
        }

        public override PredicateResult Call()
        {
            _term = _iterm.Dereference();
            _options = _ioptions.Dereference();
            _stream = _istream.Dereference();
            StreamTerm stream = GetStream();
            if (!_options.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _options, this);
            }
            PrologList _optionsList = _options as PrologList;
            WriteOptions opts = ParseOptions(_optionsList);
            _term.Write(stream, opts);

            return Success();
        }

        private WriteOptions ParseOptions(PrologList list)
        {
            WriteOptions opts = new WriteOptions();
            foreach (Term t in list)
            {
                if(!t.IsBound)
                {
                    throw new InstantiationException(this);
                }
                Structure str = t as Structure;
                if (str == null||str.Arity != 1)
                {
                    throw new DomainException(ValidDomains.write_option, t, this);
                }
                
                switch (str.Name)
                {
                    case "quoted":
                        opts.quoted = Utils.Conversion.TermToBool(str[0]);
                        break;
                    case "ignore_ops":
                        opts.ignore_ops = Utils.Conversion.TermToBool(str[0]);
                        break;
                    case "numbervars":
                        opts.numbervars = Utils.Conversion.TermToBool(str[0]);
                        break;
                    default:
                        throw new DomainException(ValidDomains.write_option, t, this);
                        
                }
            }
            return opts;
        }
    }
}
