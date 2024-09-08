using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    [PrologPredicate(Name = "close", Arity = 2)]
    public class close_2 : StreamBasePredicate
    {
        Term _opt, _iopt;

        public close_2(IPredicate continuation, IEngine engine, Term stream, Term opt)
            :base(continuation,engine,stream)
        {
            _iopt = opt;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _opt = _iopt.Dereference();
            ErrorCheck();
            //TODO: check alias
            StreamTerm stream = GetStream();
           
            //TODO: check options
            stream.Close(true);
            return Success();
        }

        private void ErrorCheck()
        {
            if (!_stream.IsBound || !_opt.IsGround)
            {
                throw new InstantiationException(this);
            }
            if (!_opt.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _opt,this);
            }
            
        }
    }
}
