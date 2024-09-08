using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    public class StreamBasePredicate : BindingPredicate
    {
        protected Term _stream, _istream;

        public StreamBasePredicate(IPredicate continuation, IEngine engine, Term stream)
            :base(continuation,engine)
        {
            _istream = stream;
        }

        protected StreamTerm GetStream()
        {
            if (!_stream.IsBound)
            {
                throw new InstantiationException(this);
            }
            StreamTerm stream = StreamTerm.GetStreamFromTerm(_stream);
            if (stream == null)
            {
                throw new DomainException(ValidDomains.stream_or_alias, _stream, this);
            }
            if (!stream.IsOpen)
            {
                throw new ExistenceException(ObjectType.stream, _stream, this);
            }
            return stream;
        }

    }
}
