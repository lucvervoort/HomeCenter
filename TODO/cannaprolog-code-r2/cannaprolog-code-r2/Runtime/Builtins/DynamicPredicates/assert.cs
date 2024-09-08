using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.DynamicPredicates
{
    [PrologPredicate(Name = "assert", Arity = 1)]
    public class assert_1 : BasePredicate
    {
        Term _clause,_iclause;

        public assert_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _iclause = arg1;
        }

        public override PredicateResult Call()
        {
            Structure head, body;
            _clause = _iclause.Dereference();
            if (!_clause.IsBound)
            {
                throw new InstantiationException(this);
            }
            
            Structure str = _clause as Structure;
            if(str == null)
            {
                throw new TypeMismatchException(ValidTypes.Callable, _clause, this);
            }
            if (str.Name == ":-")
            {
                head = str[0] as Structure;
                body = str[1] as Structure;
            }
            else
            {
                head = str;
                body = Structure.True;
            }
            if (head == null)
            {
                throw new TypeMismatchException(ValidTypes.Callable, str[0], this);
            }
            if (body == null)
            {
                throw new TypeMismatchException(ValidTypes.Callable, str[1], this);
            }

            Clause c = Clause.FromFunctor(str);


            PredicateIndicator pi = c.Head.GetPI();
            PredicateInfo pinfo = PredicateTable.Current.GetPredicateInfo(pi);
            if (pinfo !=null && !pinfo.IsDynamic)
            {
                throw new PermissionException(Operations.modify, PermissionsTypes.static_procedure, pi.GetPITerm(), this);
            }

            DynamicPredicate dp = null;
            if (PredicateTable.Current.Contains(pi))
            {
                dp = PredicateTable.Current.GetDynamicPredicate(pi);
            }
            else
            {
                dp = new DynamicPredicate();
                PredicateTable.Current.AssertPredicate(pi, dp);
            }
            AssertClause(dp, c);

            return Success();
        }

        protected virtual void AssertClause(DynamicPredicate dp, Clause c)
        {
            dp.AppendClause(c);
        }
    }

    [PrologPredicate(Name = "assertz", Arity = 1)]
    public class assertz_1 : assert_1
    {
        public assertz_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation, engine, arg1)
        { }
    }

    [PrologPredicate(Name = "asserta", Arity = 1)]
    public class asserta_1 : assert_1
    {
       public asserta_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation, engine, arg1)
       {}


        protected override void AssertClause(DynamicPredicate dp, Clause c)
        {
            dp.InsertClause(c);
        }
    }
    
}
