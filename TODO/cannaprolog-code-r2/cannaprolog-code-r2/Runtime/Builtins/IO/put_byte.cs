using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins.Exceptions;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    [PrologPredicate(Name = "put_byte", Arity = 2)]
    public class put_byte_2 : StreamBasePredicate
    {
        Term _byte, _ibyte;

        public put_byte_2(IPredicate continuation, IEngine engine, Term stream, Term b)
            : base(continuation, engine, stream)
        {
            _ibyte = b;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _byte = _ibyte.Dereference();
            if (!_byte.IsBound)
            {
                throw new InstantiationException(this);
            }

            Integer b = _byte as Integer;
            if (b == null || b.Value<0 || b.Value>255)
            {
                throw new TypeMismatchException(ValidTypes.Byte, _byte, this);
            }
            StreamTerm stream = GetStream();
            stream.PutByte(b);
            return Success();
        }
    }

    [PrologPredicate(Name = "put_byte", Arity = 1)]
    public class put_byte_1 : BasePredicate
    {
        Term _byte, _ibyte;

        public put_byte_1(IPredicate continuation, IEngine engine, Term b)
            : base(continuation,engine)
        {
            _ibyte = b;
        }

        public override PredicateResult Call()
        {
            _byte = _ibyte.Dereference();
            if (!_byte.IsBound)
            {
                throw new InstantiationException(this);
            }

            Integer b = _byte as Integer;
            if (b == null || b.Value < 0 || b.Value > 255)
            {
                throw new TypeMismatchException(ValidTypes.Byte, _byte, this);
            }
            StreamTerm.CurrentOutput.PutByte(b);
            return Success();
        }
    }

    [PrologPredicate(Name = "peek_byte", Arity = 2)]
    public class peek_byte_2 : StreamBasePredicate
    {
        Term _byte, _ibyte;

        public peek_byte_2(IPredicate continuation, IEngine engine, Term stream, Term b)
            : base(continuation, engine, stream)
        {
            _ibyte = b;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _byte = _ibyte.Dereference();

            StreamTerm stream = GetStream();
            Integer b = stream.PeekByte();
            if (!_byte.UnifyWithInteger(b, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    [PrologPredicate(Name = "peek_byte", Arity = 1)]
    public class peek_byte_1 : BindingPredicate
    {
        Term _byte, _ibyte;

        public peek_byte_1(IPredicate continuation, IEngine engine, Term b)
            : base(continuation,engine)
        {
            _ibyte = b;
        }

        public override PredicateResult Call()
        {
            _byte = _ibyte.Dereference();

            Integer b = StreamTerm.CurrentInput.PeekByte();
            if (!_byte.UnifyWithInteger(b, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    [PrologPredicate(Name = "get_byte", Arity = 2)]
    public class get_byte_2 : StreamBasePredicate
    {
        Term _byte, _ibyte;

        public get_byte_2(IPredicate continuation, IEngine engine, Term stream, Term b)
            : base(continuation, engine, stream)
        {
            _ibyte = b;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _byte = _ibyte.Dereference();

            StreamTerm stream = GetStream();
            Integer b = stream.GetByte();
            if (!_byte.UnifyWithInteger(b, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    [PrologPredicate(Name = "get_byte", Arity = 1)]
    public class get_byte_1 : BindingPredicate
    {
        Term _byte, _ibyte;

        public get_byte_1(IPredicate continuation, IEngine engine, Term b)
            : base(continuation,engine)
        {
            _ibyte = b;
        }

        public override PredicateResult Call()
        {
            _byte = _ibyte.Dereference();

            Integer b = StreamTerm.CurrentInput.GetByte();
            if (!_byte.UnifyWithInteger(b, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }
}
