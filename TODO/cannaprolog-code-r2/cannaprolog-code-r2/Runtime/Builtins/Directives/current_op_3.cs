using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins.Exceptions;

namespace Canna.Prolog.Runtime.Builtins.Directives
{
    [PrologPredicate(Name = "current_op", Arity = 3)]
    public class current_op_3 : BindingPredicate
    {
        Term _priority, _ipriority;
        Term _op_specifier, _iop_specifier;
        Term _op, _iop;
        Integer priority;
        Structure opspec;
        IEnumerator<Op> _operators;

        public current_op_3(IPredicate continuation, IEngine engine, Term priority, Term op_specifier, Term op)
            : base(continuation,engine)
        {
            _ipriority = priority;
            _iop_specifier = op_specifier;
            _iop = op;
        }

        public override PredicateResult Call()
        {
            _priority = _ipriority.Dereference();
            _op_specifier = _iop_specifier.Dereference();
            _op = _iop.Dereference();
            _operators = Op.GetAllOperators();
            return EnumerateOps();
        }

        private PredicateResult EnumerateOps()
        {
            if (!_operators.MoveNext())
                return Fail();
            Engine.AddChoicePoint(this);
            Op op = _operators.Current;
            int prec = op.PrefixPriority + op.InfixPriority + op.PostfixPriority;
            if (_priority.UnifyWithInteger(new Integer(prec), Engine.BoundedVariables, false))
                if (_op_specifier.UnifyWithStructure(new Structure(op.Type.ToString()), Engine.BoundedVariables, false))
                    if (_op.UnifyWithStructure(new Structure(op.Name), Engine.BoundedVariables, false))
                    {
                       return Success();
                    }
                return Fail();
        }

        public void ErrorCheck()
        {
            if (_priority.IsBound)
            {
                priority = _priority as Integer;
                if ((priority == null) || (priority.Value < 0) || (priority.Value > 1200))
                {
                    throw new DomainException(ValidDomains.operator_priority, _priority,this);
                }
            }
            if (_op_specifier.IsBound)
            {
                opspec = _op_specifier as Structure;
                if ((opspec == null) || (!opspec.IsAtom) || (!Op.IsValiOpSpecifier(opspec.Name)))
                {
                    throw new DomainException(ValidDomains.operator_specifier, _op_specifier, this);
                }
            }
            if (_op.IsBound && !_op.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _op,this);
            }
        }

        public override PredicateResult Redo()
        {
            return EnumerateOps();

        }
    }
}
