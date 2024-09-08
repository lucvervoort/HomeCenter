using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Directives
{
    [PrologPredicate(Name = "current_prolog_flag", Arity = 2)]
    public class current_prolog_flag_2 : BindingPredicate
    {
        Term _flag, _iflag, _value, _ivalue;

        IEnumerator<PrologFlag> _flags;

        public current_prolog_flag_2(IPredicate continuation, IEngine engine, Term flag, Term value)
            : base(continuation,engine)
        {
            _iflag = flag;
            _ivalue = value;
        }

        public override PredicateResult Call()
        {
            _flag = _iflag.Dereference();
            _value = _ivalue.Dereference();

            if (_flag.IsBound && !_flag.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _flag, this);
            }

            _flags = PrologFlagCollection.Current.GetFlags();
            return InternalRedo();
        }

        private bool UnifyWithFlag(PrologFlag flag)
        {
            Structure name = new Structure(flag.Name);

            if (_flag.UnifyWithStructure(name, Engine.BoundedVariables, false))
                if (_value.Unify(flag.Value, Engine.BoundedVariables, false))
                {
                    return true;
                }
            return false;
        }

        private PredicateResult InternalRedo()
        {
            if (!_flags.MoveNext())
                return Fail();
            Engine.AddChoicePoint(this);
            PrologFlag flag = _flags.Current;
            if(UnifyWithFlag(flag))
            {
                return Success();
            }
            return Fail();
        }

        //TODO: predicates with internal redo can share redo() methods
        public override PredicateResult Redo()
        {

            return (InternalRedo());
        }


    }
}
