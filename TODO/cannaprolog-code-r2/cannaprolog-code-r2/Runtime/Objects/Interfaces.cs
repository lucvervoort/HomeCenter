/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;


namespace Canna.Prolog.Runtime.Objects
{
 
    public interface IPredicate
    {
        PredicateResult Call();
        PredicateResult Redo();
        IPredicate Continuation { get; set;}
    }

    public interface ITermVisitor
    {
        void VisitVar(Var var);
        void VisitInteger(Integer integer);
        void VisitFloat(Floating floating);
        void VisitStruct(Structure structure);
        void VisitList(PrologList list);
    }

    public interface ITermComparer
    {
        int Compare(Term t1, Term t2);
    }

    public interface INumberComparer
    {
        int Compare(Number n1, Number n2);
    }

    public interface IEngine
    {
        void AddChoicePoint(IPredicate predicate);
        int GetDepth();
        void CutToDepth(int depth);
        PredicateResult ExecuteGoal(IPredicate predicate);
        PredicateResult Redo();
        VarList BoundedVariables { get;}
        IPredicate Peek();
        IPredicate CurrentGoal { get;}

    }
}
