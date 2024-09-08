using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    [PrologPredicate(Name = "flush_output", Arity = 1)]
    public class flush_output_1 : StreamBasePredicate
    {
        public flush_output_1(IPredicate continuation, IEngine engine, Term stream)
            :base(continuation,engine,stream)
        {

        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            StreamTerm stream = GetStream();
            stream.Flush();
            return Success();
        }

    }
}
