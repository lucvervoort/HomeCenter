using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Compiler;

namespace Canna.Prolog.Runtime.Builtins.DynamicPredicates
{
    [PrologPredicate(Name = "clause", Arity = 2)]
    public class clause_2 : BindingPredicate
    {
        Term _head, _ihead;
        Term _body, _ibody;
        IEnumerator<Clause> _clauses;
        Structure head;

        public clause_2(IPredicate continuation, IEngine engine, Term clause, Term body)
            :base(continuation,engine)
        {
            _ihead = clause;
            _ibody = body;
        }

        public override PredicateResult Call()
        {
            _head = _ihead.Dereference();
            _body = _ibody.Dereference();
            ErrorCheck();
            PredicateInfo pi = PredicateTable.Current.GetPredicateInfo(head.GetPI());
            if (pi == null)
            {
                return Fail();
            }
            if (!pi.IsPublic)
            {
                throw new PermissionException(Operations.access, PermissionsTypes.private_procedure,head.GetPI().GetPITerm() ,this);
            }
            _clauses = GetClauses(head);

            return InternalRedo();
        }

        private PredicateResult InternalRedo()
        {
            if (!_clauses.MoveNext())
                return Fail();
            Engine.AddChoicePoint(this);
            if (UnifyClause(_clauses.Current))
            {
                return Success();
            }
            return Fail();
        }

        public override PredicateResult Redo()
        {
            return InternalRedo();
        }

        private bool UnifyClause(Clause clause)
        {
            if (!clause.Head.Unify(_head, Engine.BoundedVariables, false))
            {
                return false;
            }
            else
            {
                if ((clause.Body != null))
                {
                    if (!_body.Unify(clause.Body, Engine.BoundedVariables, false))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!_body.Unify(Structure.True, Engine.BoundedVariables, false))
                    {
                        return false;
                    }
                }
 
            }
            return true;
        }

        private IEnumerator<Clause> GetClauses(Structure head)
        {
            //First try dynamic predicates
            PredicateIndicator pi = head.GetPI();
            PredicateInfo pinfo = PredicateTable.Current.GetPredicateInfo(pi);
            if (pinfo != null)
            {
                if (pinfo.IsDynamic)
                {
                    DynamicPredicate dp = PredicateTable.Current.GetDynamicPredicate(pi);
                    foreach (Clause c in dp.Clauses)
                    {
                        yield return c;
                    }
                }
                else
                {
                    //Then it's static
                    Type t = PredicateTable.Current.GetPredicateType(pi);
                    if (t != null)
                    {
                        Type[] clauses = t.GetNestedTypes();
                        foreach (Type clauseType in clauses)
                        {
                            object[] attributes = clauseType.GetCustomAttributes(typeof(PrologClauseAttribute), false);
                            if (attributes.Length > 0)
                            {
                                yield return ((PrologClauseAttribute)attributes[0]).Clause.Copy(new Variables());
                            }
                        }
                    }
                }
            }
        }

        private void ErrorCheck()
        {
            if (!_head.IsBound)
            {
                throw new InstantiationException(this);
            }
            head = _head as Structure;
            if (head == null)
            {
                throw new TypeMismatchException(ValidTypes.Callable, _head, this);
            }
            if (_body.IsBound && !(_body is Structure))
            {
                throw new TypeMismatchException(ValidTypes.Callable, _body, this);
            }

        }
    }
}
