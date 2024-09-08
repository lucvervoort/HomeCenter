using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Recorded
{
    [PrologPredicate(Name = "erase", Arity = 1)]
    public class erase_1 : BasePredicate
    {
        Term _ref, _iref;

        public erase_1(IPredicate continuation, IEngine engine, Term reference)
            : base(continuation, engine)
        {
            _iref = reference;
        }

        public override PredicateResult Call()
        {
            _ref = _iref.Dereference();
            if (!(_ref is RecordReferenceTerm))
            {
                throw new TypeMismatchException(new Structure("RecordReference"), _ref, this);
            }

            RecordReferenceTerm rrt = _ref as RecordReferenceTerm;
            RecordedDB.Current.Erase(rrt);

            return Success();
        }
    }
}
