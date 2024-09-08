using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    [PrologPredicate(Name = "stream_property", Arity = 2)]
    public class stream_property_2 : BindingPredicate
    {
        Term _stream, _istream;
        Term _prop, _iprop;
        StreamTerm _theStream;
        IEnumerator<StreamTerm> _streams;
        IEnumerator<Structure> _properties;

        public stream_property_2(IPredicate continuation, IEngine engine, Term stream, Term prop)
            : base(continuation,engine)
        {
            _istream = stream;
            _iprop = prop;
        }

        public override PredicateResult Call()
        {
            _stream = _istream.Dereference();
            _prop = _iprop.Dereference();
            if (_stream.IsBound)
            {
                _theStream = StreamTerm.GetStreamFromTerm(_stream);
                if (_theStream == null)
                {
                    throw new DomainException(ValidDomains.stream_or_alias, _stream, this);
                }
                _streams = GetJustOne();
            }
            else
            {
                _streams = StreamTerm.GetAllOpenStreams();
            }
            return InternalRedo();
        }


        private PredicateResult InternalRedo()
        {
            start:
            if (_properties == null || !_properties.MoveNext())
            {
                if(!_streams.MoveNext())
                    return Fail();
                _properties = _streams.Current.GetProperties();
                goto start;
            }
            Engine.AddChoicePoint(this);
            if (_prop.UnifyWithStructure(_properties.Current,Engine.BoundedVariables,false))
            {
                if (!_stream.IsBound)
                {
                    _stream.Unify(_streams.Current, Engine.BoundedVariables, false);
                }
                return Success();
            }
            return Fail();
        }

        //TODO: predicates with internal redo can share redo() methods
        public override PredicateResult Redo()
        {
            return InternalRedo();
        }


        private IEnumerator<StreamTerm> GetJustOne()
        {
            yield return _theStream;
        }
    }
}
