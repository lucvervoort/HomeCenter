using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.AtomProcessing
{
    [PrologPredicate(Name = "char_code", Arity = 2)]
    public class char_code_2 : BindingPredicate
    {
        Term _char,_ichar;
        Term _code,_icode;
        Structure _ch;
        Integer _co;
        public char_code_2(IPredicate contination, IEngine engine, Term character, Term code)
            : base(contination,engine)
        {
            _ichar = character;
            _icode = code;
        }

        public override PredicateResult Call()
        {
            _char = _ichar.Dereference();
            _code = _icode.Dereference();
            if (!_char.IsBound && !_code.IsBound)
            {
                throw new InstantiationException(this);
            }

            PredicateResult res=Fail();
            if (_char.IsBound)
            {
                _ch = _char as Structure;
                if ((_ch == null) || (_ch.Name.Length != 1))
                {
                    throw new TypeMismatchException(ValidTypes.Character, _char,this);
                }
                _co = new Integer(_ch.Name[0]);
                if (_co.Unify(_code,Engine.BoundedVariables,false))
                {
                    res = PredicateResult.Success;
                }
            }
            else if (_code.IsBound)
            {
                _co = _code as Integer;
                if ((_co == null) )
                {
                    throw new TypeMismatchException(ValidTypes.Integer, _code,this);
                }
                _ch = new Structure(((char)_co.Value).ToString());
                if (_ch.Unify(_char, Engine.BoundedVariables, false))
                {
                    res = PredicateResult.Success;
                }
            }

            return CallContinuation(res);
        }

       
    }
}
