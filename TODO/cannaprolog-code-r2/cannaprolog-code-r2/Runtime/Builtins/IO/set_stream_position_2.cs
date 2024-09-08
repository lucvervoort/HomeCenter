using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    [PrologPredicate(Name = "set_stream_position", Arity = 2)]
    public class set_stream_position_2 : StreamBasePredicate
    {
        Term _position, _iposition;

        public set_stream_position_2(IPredicate continuation, IEngine engine, Term stream, Term position)
            :base(continuation,engine,stream)
        {
            _iposition = position;
        }

        public override PredicateResult Call()
        {
            //TODO: position term cam change
            _position = _iposition.Dereference();
            Integer pos = _position as Integer;
            if (pos == null)
            {
                throw new DomainException(ValidDomains.stream_position, _position, this);
            }
            StreamTerm stream = GetStream();
            stream.Position = pos.Value;
            return Success();
        }
    }
}
