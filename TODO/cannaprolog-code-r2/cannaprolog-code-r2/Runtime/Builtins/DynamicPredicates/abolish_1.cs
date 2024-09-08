using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.DynamicPredicates
{
    [PrologPredicate(Name = "abolish", Arity = 1)]
    public class abolish_1 : BasePredicate
    {
        Term _pi;

        public abolish_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _pi = arg1;
        }

        public override PredicateResult Call()
        {
            _pi = _pi.Dereference();
            if (!_pi.IsBound)
            {
                throw new InstantiationException(this);
            }
            //TODO: usare validatore PI
            Structure str = _pi as Structure;
            if ((str == null)||(!str.IsPredicateIndicator))
            {
                throw new TypeMismatchException(ValidTypes.PredicateIndicator, _pi,this);
            }
            if((!str[0].IsBound)||(!str[1].IsBound))
            {
                throw new InstantiationException(this);
            }
            if (!str[0].IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, str[0], this);
            }
            Integer arity = str[1] as Integer;
            if (arity == null)
            {
                throw new TypeMismatchException(ValidTypes.Integer, str[1], this);
            }
            if (arity.Value < 0)
            {
                throw new DomainException(ValidDomains.not_less_than_zero, arity, this);
            }
            string name = ((Structure)str[0]).Name;
            PredicateIndicator pi = new PredicateIndicator(name, arity.Value);
            PredicateInfo pinfo = PredicateTable.Current.GetPredicateInfo(pi);
            if (pinfo!=null && !pinfo.IsDynamic)
            {
                throw new PermissionException(Operations.modify, PermissionsTypes.static_procedure, pi.GetPITerm(), this);
            }
            if (PredicateTable.Current.Contains(pi))
            {
                PredicateTable.Current.AbolishPredicate(pi);
            }

            return Success();
        
        }


    }

 

}
