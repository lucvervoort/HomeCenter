using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins.Control;

namespace Canna.Prolog.Runtime.Builtins.Exceptions
{
    
    [PrologPredicate(Name = "catch", Arity = 3)]
    public class catch_3 : BindingPredicate
    {
        IPredicate _goal;

        Term _catcher;
        IPredicate _recover;
        int _depth = -1;

        public catch_3(IPredicate continuation, IEngine engine, Term goal, Term catcher, Term recover)
            :base(continuation,engine)
        {
            _goal = new call_1(null, Engine, goal);
            _catcher = catcher;
            _recover = new call_1(continuation, Engine, recover);
        }

        public override PredicateResult Call()
        {
            try
            {
                _depth = Engine.GetDepth();
                _goal = _goal.Call().Continuation;
                PredicateResult res = Engine.ExecuteGoal(_goal);

                if (Engine.GetDepth() > _depth)
               {
                   Engine.AddChoicePoint(this);
               }
               return CallContinuation(res);
            }
            catch (PrologRuntimeException pre)
            {
                return HandleException(pre);
            }
        }

        public override PredicateResult Redo()
        {
            try
            {
                IPredicate goal = ((Engine)Engine).RemoveChoicePoint();
                PredicateResult res = goal.Redo();
                res = ((Engine)Engine).Loop(res, _depth);

                if (Engine.GetDepth() > _depth)
                {
                    Engine.AddChoicePoint(this);
                }
                return CallContinuation(res);
            }
            catch (PrologRuntimeException pre)
            {
                return HandleException(pre);
            }
        }

        private PredicateResult HandleException(PrologRuntimeException pre)
        {
            if (!_catcher.Unify(pre.Term,Engine.BoundedVariables,false))
            {
                throw pre;
            }
            Engine.CutToDepth(_depth);
            this.Continuation = _recover.Call().Continuation;
            return Success();
        }


    }
     
}
