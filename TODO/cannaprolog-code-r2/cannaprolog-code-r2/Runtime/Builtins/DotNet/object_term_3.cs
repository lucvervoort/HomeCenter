using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.DotNet
{
    [PrologPredicate(Name = "object_term", Arity = 3)]
    public class object_term_3 : BasePredicate
    {
        Term _ob, _iob, _term, _iterm, _opt, _iopt;

        public object_term_3(IPredicate continuation, IEngine engine,
            Term ob, Term term, Term opt)
            : base(continuation, engine)
        {
            _iob = ob;
            _iterm = term;
            _iopt = opt;
        }

        public override PredicateResult Call()
        {
            _ob = _iob.Dereference();
            _term = _iterm.Dereference();
            _opt = _iopt.Dereference();
            ErrorCheck();
            Term arg1=null,arg2=null;
            if (_ob.IsBound)
            {
                if (!_ob.IsObject)
                {
                    throw new TypeMismatchException(ValidTypes.Object, _ob, this);
                }
                arg1 = _term;
                arg2 = ObjectToTerm(((ObjectTerm)_ob).ObjectValue);
            }
            else{
                TermConverter tc = new TermConverter(_term);
                ObjectTerm ob = tc.Convert();
                arg1 = _ob;
                arg2 = ob;
            }
            if (!arg1.Unify(arg2, Engine.BoundedVariables, false))
            {
                return Fail();
            }

            return base.Call();
        }

        private Term ObjectToTerm(object ob)
        {
            ObjectConverter oc = new ObjectConverter(ob);
            return oc.Convert();
        }



        private void ErrorCheck()
        {
            if (!_ob.IsBound && !_term.IsBound)
            {
                throw new InstantiationException(this);
            }
        }
    }

    [PrologPredicate(Name = "object_term", Arity = 2)]
    public class object_term_2 : object_term_3
    {
        public object_term_2(IPredicate continuation, IEngine engine,
            Term ob, Term term)
            : base(continuation, engine, ob, term, new PrologList())
        {
        }
    }
}
