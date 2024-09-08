using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.TermComposition
{
    [PrologPredicate(Name="=..", Arity=2)]
    public class univ_2 : BindingPredicate
    {
        Term _term,_iterm;
        Term _list,_ilist;

        public univ_2(IPredicate continuation, IEngine engine, Term term, Term list)
            : base(continuation,engine)
        {
            _iterm = term;
            _ilist = list;
        }

        public override PredicateResult Call()
        {
            _term = _iterm.Dereference();
            _list = _ilist.Dereference();

            if (_term.IsBound)
            {
                return DecomposeTerm();
            }
            else return ComposeTerm();



        }

        private PredicateResult ComposeTerm()
        {
            PrologList list = _list as PrologList;
            if (list == null)
            {
                throw new TypeMismatchException(ValidTypes.List, _list, this);
            }
//TODO: check that _list is not partial list
            if (((PrologList)list.Tail).isEmpty())
            {
                if (!_term.Unify(list.Head, Engine.BoundedVariables, false))
                {
                    return Fail();
                }
            }
            else
            {
                Structure head = list.Head as Structure;
                if ((head == null)||(!head.IsAtom))
                {
                    throw new TypeMismatchException(ValidTypes.Atom, list.Head, this);
                }
                Structure newstr = new Structure(head.Name);
                PrologList arg = list.Tail as PrologList;
                while (!arg.isEmpty())
                {
                    if (arg == null)
                    {
                        throw new TypeMismatchException(ValidTypes.List, list, this);
                    }
                    newstr.AddArg(arg.Head);
                    arg = arg.Tail as PrologList;
                }
                if (!_term.UnifyWithStructure(newstr, Engine.BoundedVariables, false))
                {
                    return Fail();
                }

            }

           
            return Success();
        }

        private PredicateResult DecomposeTerm()
        {
            if (_term.IsCompound )
            {
                Structure s = _term as Structure;
                PrologList list = new PrologList(new Structure(s.Name));
                foreach (Term t in s.Args)
                {
                    list = list.Append(new PrologList(t));
                }
                if (!list.Unify(_list, Engine.BoundedVariables, false))
                {
                    return Fail();
                }

            }
            else if(_term.IsNumber || _term.IsAtom)
            {
                if (!new PrologList(_term).Unify(_list, Engine.BoundedVariables, false))
                {
                    return Fail();
                }
            }
            else
            {
                throw new InstantiationException(this);
            }
            return Success();
        }
    }
}
