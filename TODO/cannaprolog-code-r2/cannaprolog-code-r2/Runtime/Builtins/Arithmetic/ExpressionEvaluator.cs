/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Arithmetic
{
    internal class ExpressionEvaluator : ITermVisitor
    {
        Term _theExp;
        Term result;

        public ExpressionEvaluator(Term expression)
        {
            _theExp = expression;
        }

        public Term Eval()
        {
            _theExp.Accept(this);
            return result;
        }

        #region ITermVisitor Members

        public void VisitVar(Var var)
        {
            result = var.Dereference();
        }

        public void VisitInteger(Integer integer)
        {
            result = integer;
        }

        public void VisitFloat(Floating floating)
        {
            result = floating;
        }

        public void VisitStruct(Structure structure)
        {
            Term subres1,subres2=null;
            //if (!structure.isOp())
            //{
            //    throw new PrologException("Operator unknown: " + structure.Name);
            //}
            ExpressionEvaluator exp1 = new ExpressionEvaluator(structure[0]);
            subres1 = exp1.Eval();
            if (structure.Args.Count>1)
            {
                ExpressionEvaluator exp2 = new ExpressionEvaluator(structure[1]);
                subres2 = exp2.Eval();
            }
            switch (structure.Name)
            {
                case "+":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Add);
                    break;
                case "*":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Multiply);
                    break;
                case "-":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Subtract);
                    break;
                case "/":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Divide);
                    break;
                case "//":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.IntDiv);
                    break;
                case "rem":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Remainder);
                    break;
                case "mod":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Modulus);
                    break;
                case "abs":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Abs);
                    break;
                case "sign":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Sign);
                    break;
                case "float_integer_part":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.IntegerPart);
                    break;
                case "float_fractional_part":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.FractionalPart);
                    break;
                case "float":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Float);
                    break;
                case "floor":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Floor);
                    break;
                case "round":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Round);
                    break;
                case "ceiling":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Ceiling);
                    break;
                case "truncate":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.IntegerPart);
                    break;
                case "**":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Power);
                    break;
                case "sin":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Sin);
                    break;
                case "cos":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Cos);
                    break;
                case "atan":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Atan);
                    break;
                case "exp":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Exp);
                    break;
                case "log":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Log);
                    break;
                case "sqrt":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.Sqrt);
                    break;
                case ">>":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.RShift);
                    break;
                case "<<":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.LShift);
                    break;
                case "/\\":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.BitWiseAnd);
                    break;
                case "\\/":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.BitWiseOr);
                    break;
                case "\\":
                    result = TermCalculator.Calculate(subres1, subres2, TermCalculator.Operation.BitWiseComplement);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }


        public void VisitList(PrologList list)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
}
}
