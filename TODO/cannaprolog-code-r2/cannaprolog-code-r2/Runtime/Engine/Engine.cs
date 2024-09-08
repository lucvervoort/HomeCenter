using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using System.Diagnostics;

namespace Canna.Prolog.Runtime
{
    public class Engine : IEngine
    {
        //private static Engine _current = new Engine();

        //public static Engine Current
        //{
        //    get { return Engine._current; }
        //    set { Engine._current = value; }
        //}

        public static Engine Create()
        {
            return new Engine();
        }

        private Engine()
        {
            _boundedVarStack.Push(new VarList());
        }


        private Stack<IPredicate> _choicePoints = new Stack<IPredicate>();

        public Stack<IPredicate> ChoicePoints
        {
            get { return _choicePoints; }
        }

        private Stack<VarList> _boundedVarStack = new Stack<VarList>();

        public VarList BoundedVariables
        {
            get { return _boundedVarStack.Peek(); }
        }

        IPredicate _currentGoal;

        public IPredicate CurrentGoal
        {
            get { return _currentGoal; }
        }

        public PredicateResult ExecuteGoal(IPredicate goal)
        {
            int topStack = GetDepth();
            _currentGoal = goal;
            PredicateResult res = CurrentGoal.Call();
            
            return Loop(res,topStack);
        }

        public PredicateResult Redo()
        {
            while (GetDepth() > 0)
            {
                _currentGoal = RemoveChoicePoint();
                PredicateResult res = CurrentGoal.Redo();
                   return Loop(res,0);
            }
            return PredicateResult.Failed;
        }

        internal PredicateResult Loop(PredicateResult res, int topStack)
        {
            redo:
            if (res.IsFailed)
            {//Try to backtrack
                if (GetDepth() > topStack)
                {
                    _currentGoal = RemoveChoicePoint();
                    res = CurrentGoal.Redo();
                    goto redo;
                }
                else
                    return PredicateResult.Failed;
            }
            if (res.Continuation != null)
            {
                _currentGoal = res.Continuation;
                res = CurrentGoal.Call();
                goto redo;
            }
            return res;
        }

        private void AllocateVariables()
        {
            _boundedVarStack.Push(new VarList());
        }

        #region IEngine Members

        public void AddChoicePoint(IPredicate predicate)
        {
            _choicePoints.Push(predicate);
            AllocateVariables();
        }


        public IPredicate RemoveChoicePoint()
        {
            Debug.Assert(_boundedVarStack.Count == _choicePoints.Count + 1);
            _boundedVarStack.Pop().Unbind();
                //_boundedVarStack.Pop().Unbind();
                //_boundedVarStack.Push(top);
            return _choicePoints.Pop();
        }



        public int GetDepth()
        {
            return _choicePoints.Count;
        }


        public void CutToDepth(int depth)
        {
            while (_choicePoints.Count > depth)
                _choicePoints.Pop();
            while (_boundedVarStack.Count > depth + 1)
            {
              VarList var =  _boundedVarStack.Pop();
              _boundedVarStack.Peek().AddRange(var);
            }
        }




        #endregion

        #region IEngine Members


        public IPredicate Peek()
        {
            if (_choicePoints.Count > 0)
                return _choicePoints.Peek();
            else return null;
        }


        public void Set(IPredicate pred)
        {
            _choicePoints.Pop();
            _choicePoints.Push(pred);
        }




        #endregion
}
}
