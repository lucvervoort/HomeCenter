using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Recorded
{
    [PrologPredicate(Name = "recorded", Arity = 3)]
    public class recorded_3 : BasePredicate
    {
        private Term _key, _ikey, _term, _iterm, _ref, _iref;

        private IEnumerator<Term> _terms = null;

        public recorded_3(IPredicate continuation, IEngine engine,
            Term key, Term term, Term reference)
            : base(continuation, engine)
        {
            _ikey = key;
            _iterm = term;
            _iref = reference;
        }

        public override PredicateResult Call()
        {
            _key = _ikey.Dereference();
            _term = _iterm.Dereference();
            _ref = _iref.Dereference();

            if (!_key.IsBound)
            {
                throw new InstantiationException(this);
            }
            Var reference = _ref as Var;
            if (reference == null || reference.IsBound)
            {
                throw new TypeMismatchException(ValidTypes.Variable, _ref, this);
            }

            TermList _tl = RecordedDB.Current.GetByKey(_key);
            if (_tl == null)
                return Fail();
            _terms = _tl.GetEnumerator();
            //Integer i = RecordedDB.Current.Recorded(_key, _term, Engine.BoundedVariables);
            //i.UnifyWithVar(reference, Engine.BoundedVariables, false);

            return NextRecorded();

        }

        private PredicateResult NextRecorded()
        {
            if(!_terms.MoveNext())
            {
                return Fail();
            }
            Engine.AddChoicePoint(this);
            Term t = _terms.Current;
            if (t.Unify(_term, Engine.BoundedVariables, false))
            {
                RecordReferenceTerm rrt = new RecordReferenceTerm(new RecordReference(_key, t));
                rrt.UnifyWithVar(_ref as Var,Engine.BoundedVariables,false);
                return Success();
            }
            else
                return Fail();
        }

        public override PredicateResult Redo()
        {
            return NextRecorded();
        }

    }
}
