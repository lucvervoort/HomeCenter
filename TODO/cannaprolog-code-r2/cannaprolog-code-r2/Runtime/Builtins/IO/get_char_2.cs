using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    
    [PrologPredicate(Name = "get_char", Arity = 2)]
    public class get_char_2 : StreamBasePredicate
    {
        Term _char, _ichar;

        public get_char_2(IPredicate continuation, IEngine engine, Term stream, Term ch)
            : base(continuation,engine,stream)
        {
            _ichar = ch;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _char = _ichar.Dereference();

            StreamTerm stream = GetStream();
            Structure ch = stream.GetChar();
            if (!_char.UnifyWithStructure(ch, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    [PrologPredicate(Name = "get_code", Arity = 2)]
    public class get_code_2 : StreamBasePredicate
    {
        Term _code, _icode;

        public get_code_2(IPredicate continuation, IEngine engine, Term stream, Term code)
            : base(continuation, engine, stream)
        {
            _icode = code;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _code = _icode.Dereference();

            StreamTerm stream = GetStream();
            Integer code = stream.GetCode();
            if (!_code.UnifyWithInteger(code, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    [PrologPredicate(Name = "peek_char", Arity = 2)]
    public class peek_char_2 : StreamBasePredicate
    {
        Term _char, _ichar;

        public peek_char_2(IPredicate continuation, IEngine engine, Term stream, Term ch)
            : base(continuation, engine, stream)
        {
            _ichar = ch;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _char = _ichar.Dereference();

            StreamTerm stream = GetStream();
            Structure ch = stream.PeekChar();
            if (!_char.UnifyWithStructure(ch, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    [PrologPredicate(Name = "peek_code", Arity = 2)]
    public class peek_code_2 : StreamBasePredicate
    {
        Term _code, _icode;

        public peek_code_2(IPredicate continuation, IEngine engine, Term stream, Term code)
            : base(continuation, engine, stream)
        {
            _icode = code;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _code = _icode.Dereference();

            StreamTerm stream = GetStream();
            Integer code = stream.PeekCode();
            if (!_code.UnifyWithInteger(code, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    
    [PrologPredicate(Name = "put_char", Arity = 2)]
    public class put_char_2 : StreamBasePredicate
    {
        Term _char, _ichar;

        public put_char_2(IPredicate continuation, IEngine engine, Term stream, Term ch)
            : base(continuation, engine, stream)
        {
            _ichar = ch;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _char = _ichar.Dereference();
            if (!_char.IsBound)
            {
                throw new InstantiationException(this);
            }
            Structure ch = _char as Structure;
            if (!_char.IsAtom || ch.Name.Length!=1)
            {
                throw new TypeMismatchException(ValidTypes.Character, _char,this);
            }
            StreamTerm stream = GetStream();
            stream.PutChar(ch.Name[0]);
            return Success();
        }
    }

    [PrologPredicate(Name = "put_code", Arity = 2)]
    public class put_code_2 : StreamBasePredicate
    {
        Term _code, _icode;

        public put_code_2(IPredicate continuation, IEngine engine, Term stream, Term code)
            : base(continuation, engine, stream)
        {
            _icode = code;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _code = _icode.Dereference();
            if (!_code.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (!_code.IsInteger)
            {
                throw new TypeMismatchException(ValidTypes.Integer, _code, this);
            }
            StreamTerm stream = GetStream();
            int code = ((Integer)_code).Value;
            stream.PutCode(code);
            return Success();
        }
    }

    

    [PrologPredicate(Name = "get_char", Arity = 1)]
    public class get_char_1 : BindingPredicate
    {
        Term _char, _ichar;

        public get_char_1(IPredicate continuation, IEngine engine, Term ch)
            : base(continuation,engine)
        {
            _ichar = ch;
        }

        public override PredicateResult Call()
        {
            _char = _ichar.Dereference();

            
            Structure ch = StreamTerm.CurrentInput.GetChar();
            if (!_char.UnifyWithStructure(ch, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    [PrologPredicate(Name = "get_code", Arity = 1)]
    public class get_code_1 : BindingPredicate
    {
        Term _code, _icode;

        public get_code_1(IPredicate continuation, IEngine engine, Term code)
            : base(continuation,engine)
        {
            _icode = code;
        }

        public override PredicateResult Call()
        {
            _code = _icode.Dereference();

            Integer code = StreamTerm.CurrentInput.GetCode();
            if (!_code.UnifyWithInteger(code, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    [PrologPredicate(Name = "peek_char", Arity = 1)]
    public class peek_char_1 : BindingPredicate
    {
        Term _char, _ichar;

        public peek_char_1(IPredicate continuation, IEngine engine, Term ch)
            : base(continuation,engine)
        {
            _ichar = ch;
        }

        public override PredicateResult Call()
        {
            _char = _ichar.Dereference();

            Structure ch = StreamTerm.CurrentInput.PeekChar();
            if (!_char.UnifyWithStructure(ch, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }

    [PrologPredicate(Name = "peek_code", Arity = 1)]
    public class peek_code_1 : BindingPredicate
    {
        Term _code, _icode;

        public peek_code_1(IPredicate continuation, IEngine engine, Term code)
            : base(continuation,engine)
        {
            _icode = code;
        }

        public override PredicateResult Call()
        {
            _code = _icode.Dereference();

            Integer code = StreamTerm.CurrentInput.PeekCode();
            if (!_code.UnifyWithInteger(code, Engine.BoundedVariables, false))
            {
                return Fail();
            }
            return Success();
        }
    }


    [PrologPredicate(Name = "put_char", Arity = 1)]
    public class put_char_1 : BasePredicate
    {
        Term _char, _ichar;

        public put_char_1(IPredicate continuation, IEngine engine, Term ch)
            : base(continuation,engine)
        {
            _ichar = ch;
        }

        public override PredicateResult Call()
        {
            _char = _ichar.Dereference();
            if (!_char.IsBound)
            {
                throw new InstantiationException(this);
            }
            Structure ch = _char as Structure;
            if (!_char.IsAtom || ch.Name.Length != 1)
            {
                throw new TypeMismatchException(ValidTypes.Character, _char, this);
            }
            StreamTerm.CurrentOutput.PutChar(ch.Name[0]);
            return Success();
        }
    }

    [PrologPredicate(Name = "put_code", Arity = 1)]
    public class put_code_1 : BasePredicate
    {
        Term _code, _icode;

        public put_code_1(IPredicate continuation, IEngine engine, Term code)
            : base(continuation,engine)
        {
            _icode = code;
        }

        public override PredicateResult Call()
        {
            _code = _icode.Dereference();
            if (!_code.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (!_code.IsInteger)
            {
                throw new TypeMismatchException(ValidTypes.Integer, _code, this);
            }
            int code = ((Integer)_code).Value;
            StreamTerm.CurrentOutput.PutCode(code);
            return Success();
        }
    }

}
