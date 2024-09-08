using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Recorded
{
    [PrologPredicate(Name = "recorda", Arity = 3)]
    public class recorda_3 : BasePredicate
    {
        private Term _key, _ikey, _term, _iterm, _ref, _iref;

        public recorda_3(IPredicate continuation, IEngine engine,
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
            if (reference == null || reference.IsBound )
            {
                throw new TypeMismatchException(ValidTypes.Variable,_ref,this);
            }
            RecordReferenceTerm rrt = RecordedDB.Current.Record(_key, _term,false);
            rrt.UnifyWithVar(reference, Engine.BoundedVariables, false);

            return Success();

        }
    }

    [PrologPredicate(Name = "recordz", Arity = 3)]
    public class recordz_3 : BasePredicate
    {
        private Term _key, _ikey, _term, _iterm, _ref, _iref;

        public recordz_3(IPredicate continuation, IEngine engine,
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
            RecordReferenceTerm rrt = RecordedDB.Current.Record(_key, _term, true);
            rrt.UnifyWithVar(reference, Engine.BoundedVariables, false);

            return Success();

        }
    }

}
