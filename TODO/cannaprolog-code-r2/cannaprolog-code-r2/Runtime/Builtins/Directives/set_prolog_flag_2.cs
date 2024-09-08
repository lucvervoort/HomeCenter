using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Directives
{
    [PrologPredicate(Name = "set_prolog_flag", Arity = 2)]
    public class set_prolog_flag_2 : BasePredicate
    {
        Term _flag, _iflag, _value, _ivalue;

        public set_prolog_flag_2(IPredicate continuation, IEngine engine, Term flag, Term value)
            :base(continuation,engine)
        {
            _iflag = flag;
            _ivalue = value;
        }

        public override PredicateResult Call()
        {
            _flag = _iflag.Dereference();
            _value = _ivalue.Dereference();

            if (!_flag.IsBound || !_value.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (!_flag.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _flag, this);
            }
            if (!_value.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _value, this);
            }
            string name = ((Structure)_flag).Name;
            Structure val = _value as Structure;

            try
            {
                PrologFlagCollection.Current.Set(name, val);
            }
            catch (FlagNotChangeableException)
            {
                throw new PermissionException(Operations.modify, PermissionsTypes.flag, _flag, this);
            }
            return Success();
        }
    }
}
