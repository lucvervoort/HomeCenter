using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.DynamicPredicates
{
    [PrologPredicate(Name = "current_predicate", Arity = 1)]
    public class current_predicate_1 : BindingPredicate
    {
        Term _pi, _ipi;
        Structure str,name,pistr;
        Integer arity;
        IEnumerator<PredicateIndicator> _predIndicators;
        public current_predicate_1(IPredicate continuation, IEngine engine, Term pi)
            :base(continuation,engine)
        {
            _ipi = pi;
        }

        public override PredicateResult Call()
        {
            _pi = _ipi.Dereference();
            name = new Structure("");
            arity = new Integer(0);
            pistr = new Structure("/", name, arity);
            if (_pi.IsBound)
            {
                str = _pi as Structure;
                if (str == null || !str.IsPredicateIndicator)
                {
                    throw new TypeMismatchException(ValidTypes.PredicateIndicator, _pi, this);
                }
                if (str[0].IsBound && !str[0].IsAtom)
                {
                    throw new TypeMismatchException(ValidTypes.PredicateIndicator, _pi, this);
                }
                if (str[1].IsBound && !str[1].IsInteger)
                {
                    throw new TypeMismatchException(ValidTypes.PredicateIndicator, _pi, this);
                }
     
            }
            _predIndicators = PredicateTable.Current.GetPredicateIndicators();
            return InternalRedo();

        }

        private PredicateResult InternalRedo()
        {
            if (!_predIndicators.MoveNext())
                return Fail();
            Engine.AddChoicePoint(this);
            PredicateIndicator pi = _predIndicators.Current;
            name.Name = pi.Name;
            arity.Value = pi.Arity;
            if (_pi.UnifyWithStructure(pistr,Engine.BoundedVariables,false))
            {
                return Success();
            }
            return Fail();
        }

        //TODO: predicates with internal redo can share redo() methods
        public override PredicateResult Redo()
        {
            return InternalRedo();
        }

        
    }
}
