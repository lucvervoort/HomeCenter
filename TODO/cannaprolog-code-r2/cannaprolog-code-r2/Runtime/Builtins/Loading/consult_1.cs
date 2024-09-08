/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Compiler;

namespace Canna.Prolog.Runtime.Builtins.Loading
{
    [PrologPredicate(Name="$consult", Arity=1)]
    public class dollarconsult_1 : BasePredicate
    {
        Term _arg1;

        public dollarconsult_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            _arg1 = _arg1.Dereference();
            if (!_arg1.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _arg1,this);
            }
            Structure s = _arg1 as Structure;
            string filename = s.Name;
            //if (!filename.EndsWith(".pl"))
            //{
            //    filename = filename + ".pl";
            //}
            try
            {

                PrologCompiler.Consult(filename);
            }
            catch (System.IO.FileNotFoundException )
            {
                throw new ExistenceException(ObjectType.source_sink, _arg1, this);
            }
            return Success();
        }
    }
}
