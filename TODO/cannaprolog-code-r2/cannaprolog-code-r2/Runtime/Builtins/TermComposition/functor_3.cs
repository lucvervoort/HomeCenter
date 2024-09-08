using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.TermComposition
{
    [PrologPredicate(Name = "functor", Arity = 3)]
    public class functor_3 : BindingPredicate
    {
        Term _term;
        Term _name;
        Term _arity;

         public functor_3(IPredicate continuation, IEngine engine, Term term, Term name, Term arity)
             : base(continuation,engine)
         {
             _term = term;
             _name = name;
             _arity = arity;
         }

         public override PredicateResult Call()
         {
             _term = _term.Dereference();
             _name = _name.Dereference();
             _arity = _arity.Dereference();
             if (_term.IsBound)
             {
                 return DecomposeTerm();
             }
             else return ComposeTerm();
         
           

         }

        private PredicateResult ComposeTerm()
         {
             if (!_name.IsAtom)
             {
                 throw new TypeMismatchException(ValidTypes.Atom, _name,new Structure(this.ToString()));
             }
             Integer arity = _arity as Integer;

             if (arity == null)
             {
                 throw new TypeMismatchException(ValidTypes.Integer, _arity,new Structure(this.ToString()));
             }
             if (arity.Value <= 0)
             {
                 throw new DomainException(ValidDomains.not_less_than_zero, arity,this);
             }
             Structure result = new Structure(((Structure)_name).Name);
             for (int i = 0; i < arity.Value; ++i)
             {
                 result.AddArg(new Var());
             }
             if (!_term.UnifyWithStructure(result,Engine.BoundedVariables,false))
             {
                 return Fail();
             }
             return Success();
         }

        private PredicateResult DecomposeTerm()
         {
             if (_term.IsCompound||_term.IsAtom)
             {
                 Structure s = _term as Structure;
                 if (!_name.UnifyWithStructure(new Structure(s.Name), Engine.BoundedVariables,false)
                     ||
                 !_arity.UnifyWithInteger(new Integer(s.Arity), Engine.BoundedVariables,false))
                 {
                     return Fail();
                 }

             }
             else
             {
                 if (!_name.Unify(_term, Engine.BoundedVariables,false) || !_arity.UnifyWithInteger(new Integer(0),Engine.BoundedVariables,false))
                 {
                     return Fail();
                 }
             }
             return Success();
         }
    }
}
