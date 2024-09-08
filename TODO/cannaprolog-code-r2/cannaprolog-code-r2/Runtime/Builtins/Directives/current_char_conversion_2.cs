using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Lexical;

namespace Canna.Prolog.Runtime.Builtins.Directives
{
    [PrologPredicate(Name = "current_char_conversion", Arity = 2)]
    public class current_char_conversion_2 : BindingPredicate
    {
                Term _inchar, _iinchar;
        Term _outchar, _ioutchar;
        IEnumerator<KeyValuePair<char, char>> _pairs;

        public current_char_conversion_2(IPredicate continuation, IEngine engine, Term inchar, Term outchar)
            :base(continuation,engine)
        {
            _iinchar = inchar;
            _ioutchar = outchar;
        }

        public override PredicateResult Call()
        {
            _inchar = _iinchar.Dereference();
            _outchar = _ioutchar.Dereference();
            if (_inchar.IsBound)
            {
                string inchar = Utils.Conversion.AtomToString(_inchar);
                if (inchar == null || inchar.Length != 1)
                    throw new RepresentationException(RepresentationFlags.Character, this);
            }
            if (_outchar.IsBound)
            {
                string inchar = Utils.Conversion.AtomToString(_outchar);
                if (inchar == null || inchar.Length != 1)
                    throw new RepresentationException(RepresentationFlags.Character, this);
            }

            //CharConversionTable.Current.AddPair(inchar[0], outchar[0]);
            _pairs = CharConversionTable.Current.GetPairs();
            return InternalRedo();
        }

        private PredicateResult InternalRedo()
        {
            if (!_pairs.MoveNext())
                return Fail();
            Engine.AddChoicePoint(this);

            KeyValuePair<char, char> current = _pairs.Current;
            if (UnifyWithPair(current.Key, current.Value))
            {
                return Success();
            }
            return Fail();
        }

        private bool UnifyWithPair(char inchar, char outchar)
        {
            Structure strInChar = new Structure(inchar.ToString());
            Structure strOutChar = new Structure(outchar.ToString());
            if (_inchar.UnifyWithStructure(strInChar, Engine.BoundedVariables, false))
                if (_outchar.UnifyWithStructure(strOutChar, Engine.BoundedVariables, false))
                {
                    return true;
                }
            return false;
        }

        //TODO: predicates with internal redo can share redo() methods
        public override PredicateResult Redo()
        {
            return InternalRedo();
        }

    }
}
