using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    [PrologPredicate(Name = "current_input", Arity = 1)]
    public class current_input_1 : BindingPredicate
    {
        Term _stream, _istream;

        public current_input_1(IPredicate continuation, IEngine engine, Term stream)
            :base(continuation,engine)
        {
            _istream = stream;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            if (!_stream.Unify(StreamTerm.CurrentInput, Engine.BoundedVariables, false))
            {
                return Fail();
            }

            return Success();
        }
    }

    [PrologPredicate(Name = "current_output", Arity = 1)]
    public class current_output_1 : BindingPredicate
    {
        Term _stream, _istream;

        public current_output_1(IPredicate continuation, IEngine engine, Term stream)
            : base(continuation,engine)
        {
            _istream = stream;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            if (!_stream.Unify(StreamTerm.CurrentOutput, Engine.BoundedVariables, false))
            {
                return Fail();
            }

            return Success();
        }
    }

    [PrologPredicate(Name = "set_output", Arity = 1)]
    public class set_output_1 : StreamBasePredicate
    {
        public set_output_1(IPredicate continuation, IEngine engine, Term stream)
            : base(continuation, engine, stream)
        {
        }

        public override PredicateResult Call()
        {
            StreamTerm stream = GetStream();
            StreamTerm.CurrentOutput = stream;
            return Success();
        }
    }

    [PrologPredicate(Name = "set_intput", Arity = 1)]
    public class set_input_1 : StreamBasePredicate
    {
        public set_input_1(IPredicate continuation, IEngine engine, Term stream)
            : base(continuation, engine, stream)
        {
        }

        public override PredicateResult Call()
        {
            StreamTerm stream = GetStream();
            StreamTerm.CurrentInput = stream;
            return Success();
        }
    }

}
