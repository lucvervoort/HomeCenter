using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Directives
{
    [PrologPredicate(Name = "makeop", Arity = 3)]
    public class makeop_3 : BasePredicate
    {
        Term _precedence;
        Term _opSpecifier;
        Term _operator;

        public makeop_3(IPredicate cont, IEngine engine, Term prec, Term type, Term name)
            : base(cont, engine)
        {
            _precedence = prec;
            _opSpecifier = type;
            _operator = name;
        }

        public override PredicateResult Call()
        {
            Term precedence = this._precedence.Dereference();
            Term type = this._opSpecifier.Dereference();
            Term name = this._operator.Dereference();

            if ((!precedence.IsBound)||(!type.IsBound))
            {
                throw new InstantiationException(this);
            }
            if (name.IsList && !name.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (precedence.IsBound && !precedence.IsInteger)
            {
                throw new TypeMismatchException(ValidTypes.Integer, precedence,this);
            }
            if (type.IsBound && !type.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, precedence,this);
            }
            if (!name.IsList && !name.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.List, precedence, this);
            }

            Integer prec = precedence as Integer;
            if (prec == null)
            {
                throw new TypeMismatchException(ValidTypes.Integer, precedence, this);
            }

            Structure stype = type as Structure;
            if (stype == null)
            {
                throw new TypeMismatchException(ValidTypes.Atom, type, this);
            }

            Structure sname = name as Structure;
            if (sname == null)
            {
                throw new TypeMismatchException(ValidTypes.Atom, name, this);
            }
            if ((prec.Value < 0) || (prec.Value > 1200))
            {
                throw new DomainException(ValidDomains.operator_priority, prec, this);
            }
            if (!Op.IsValiOpSpecifier(stype.Name))
            {
                throw new DomainException(ValidDomains.operator_specifier, stype, this);
            }
            if((stype.Name.Length == 2 && Op.isBinary(sname.Name)))
            {
                throw new PermissionException(Operations.create, PermissionsTypes.Operator, stype, this);
            }
            if ((stype.Name.Length == 3 && Op.isUnary(sname.Name)))
            {
                throw new PermissionException(Operations.create, PermissionsTypes.Operator, stype, this);
            }

            if (name.IsBound)
            {
                
                Op.Makeop(prec.Value,(Specifier) Enum.Parse(typeof(Specifier),stype.Name), sname.Name);
            }

            return Success();

        }
    }
}
