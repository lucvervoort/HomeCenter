using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.DynamicPredicates
{
    [PrologPredicate(Name = "retract", Arity = 1)]
    public class retract_1 : BindingPredicate
    {
        Term _clauseTerm,_iclauseTerm;
        protected DynamicPredicate _dp = null;
        protected Clause _clause;

        public retract_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _iclauseTerm = arg1;
        }

        public override PredicateResult Call()
        {
            _clauseTerm = _iclauseTerm.Dereference();
            if (!_clauseTerm.IsBound)
            {
                throw new InstantiationException(this);
            }
            Structure str = _clauseTerm as Structure;
            if (str == null)
            {
                throw new TypeMismatchException(ValidTypes.Callable, _clauseTerm, this);
            }
            _clause = Clause.FromFunctor(str);
            _dp = null;
            PredicateIndicator pi = _clause.Head.GetPI();
            PredicateInfo pinfo = PredicateTable.Current.GetPredicateInfo(pi);
            if (pinfo != null && !pinfo.IsDynamic)
            {
                throw new PermissionException(Operations.modify, PermissionsTypes.static_procedure, pi.GetPITerm(), this);
            }
            if (PredicateTable.Current.Contains(pi))
            {
                _dp = PredicateTable.Current.GetDynamicPredicate(pi);
            }
            else
            {
                return Fail();
            }
            return RetractClause();
        }

        protected virtual PredicateResult RetractClause()
        {
            if (_clause == null) return Fail();
            
            Engine.AddChoicePoint(this);
            if (_dp.RetractClause(_clause, Engine.BoundedVariables))
            {
                return Success();
            }
            _clause = null;
            return Fail();
       }



        public override PredicateResult Redo()
        {
            return RetractClause();
        }

    }
    [PrologPredicate(Name = "retractall", Arity = 1)]
    public class retractall_1 : retract_1
    {


        public retractall_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation, engine, arg1)
        {
        }


        protected override PredicateResult RetractClause()
        {
            VarList var = new VarList();
            while (_dp.RetractClause(_clause, var))
            {
                var.Unbind();
            }
            var.Unbind();
            _dp = null;
            return Success();
        }

    }

}
