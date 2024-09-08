using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Lexical;

namespace Canna.Prolog.Runtime.Builtins.Directives
{
    [PrologPredicate(Name = "char_conversion", Arity = 2)]
    public class char_conversion_2 : BasePredicate
    {
        Term _inchar, _iinchar;
        Term _outchar, _ioutchar;

        public char_conversion_2(IPredicate continuation, IEngine engine, Term inchar, Term outchar)
            :base(continuation,engine)
        {
            _iinchar = inchar;
            _ioutchar = outchar;
        }

        public override PredicateResult Call()
        {
            _inchar = _iinchar.Dereference();
            _outchar = _ioutchar.Dereference();
            if (!_inchar.IsBound || !_outchar.IsBound)
            {
                throw new InstantiationException(this);
            }
            string inchar = Utils.Conversion.AtomToString(_inchar);
            if (inchar == null || inchar.Length != 1)
            {
                throw new RepresentationException(RepresentationFlags.Character, this);
            }
            string outchar = Utils.Conversion.AtomToString(_outchar);
            if (outchar == null || outchar.Length != 1)
            {
                throw new RepresentationException(RepresentationFlags.Character, this);
            }
            CharConversionTable.Current.AddPair(inchar[0], outchar[0]);

            return Success();
        }
    }
}
