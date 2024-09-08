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


    internal class TermCalculator : ITermVisitor
    {
        Term _result;
        Term _other;
        Operation _op;

        public static Term Calculate(Term left, Term right, Operation op)
        {
            return new TermCalculator().CalculateInternal(left, right, op);
        }

        private Term CalculateInternal(Term left, Term right, Operation op)
        {
            _other = right;
            _op = op;
            left.Accept(this);
            return _result;
        }

        #region ITermVisitor Members

        public void VisitVar(Var var)
        {
            throw new TypeMismatchException(ValidTypes.Number, var,null);
        }

        public void VisitInteger(Integer integer)
        {
            _result = new IntCalculator(integer,_op).Execute(_other);
        }

        public void VisitFloat(Floating floating)
        {
            _result = new FloatCalculator(floating, _op).Execute(_other);
        }

        public void VisitStruct(Structure structure)
        {
            throw new TypeMismatchException(ValidTypes.Number, structure,null);
        }

        public void VisitList(PrologList list)
        {
            throw new TypeMismatchException(ValidTypes.Number, list,null);
        }

        #endregion

        class IntCalculator : ITermVisitor
        {

            Term _result;
            Integer _integer;
            Operation _op;

            public IntCalculator(Integer integer, Operation op)
            {
                _op = op;
                _integer = integer;
            }

            public Term Execute(Term other)
            {
                if (other != null)
                {
                    other.Accept(this);
                }
                else
                {
                    DoUnary();
                }
                return _result;
            }

            private void DoUnary()
            {
                switch (_op)
                {
                    case Operation.Subtract:
                        _result = new Integer(-_integer.Value);
                        break;
                    case Operation.Add:
                        _result = _integer;
                        break;
                    case Operation.Abs:
                        _result = new Integer(Math.Abs(_integer.Value));
                        break;
                    case Operation.Sign:
                        _result = new Integer(Math.Sign(_integer.Value));
                        break;
                    case Operation.IntegerPart:
                        _result = new Integer(_integer.Value);
                        break;
                    case Operation.FractionalPart:
                        _result = new Integer(0);
                        break;
                    case Operation.Float:
                        _result = new Floating((double)_integer.Value);
                        break;
                    case Operation.Floor:
                    case Operation.Ceiling:
                    case Operation.Round:
                        _result = _integer;
                        break;
                    case Operation.Sin:
                        _result = new Floating(Math.Sin(_integer.Value));
                        break;
                    case Operation.Cos:
                        _result = new Floating(Math.Cos(_integer.Value));
                        break;
                    case Operation.Atan:
                        _result = new Floating(Math.Atan(_integer.Value));
                        break;
                    case Operation.Exp:
                        _result = new Floating(Math.Exp(_integer.Value));
                        break;
                    case Operation.Log:
                        _result = new Floating(Math.Log(_integer.Value, Math.E));
                        break;
                    case Operation.Sqrt:
                        _result = new Floating(Math.Sqrt(_integer.Value));
                        break;
                    case Operation.BitWiseComplement:
                        _result = new Integer(~_integer.Value);
                        break;
                }
            }

            #region ITermVisitor Members

            public void VisitVar(Var var)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void VisitInteger(Integer integer)
            {
                switch(_op)
                {
                    case Operation.Add:
                     _result = new Integer(_integer.Value + integer.Value);
                     break;
                    case Operation.Subtract:
                        _result = new Integer(_integer.Value - integer.Value);
                        break;
                    case Operation.Multiply:
                        _result = new Integer(_integer.Value * integer.Value);
                        break;
                    case Operation.Divide:
                        _result = new Integer(_integer.Value / integer.Value);
                        break;
                    case Operation.Power:
                        _result = new Floating(Math.Pow(_integer.Value , integer.Value));
                        break;
                    case Operation.Modulus:
                        _result = new Integer(_integer.Value % integer.Value);
                        break;
                    case Operation.IntDiv:
                        _result = new Integer(_integer.Value / integer.Value);
                        break;
                    case Operation.Remainder:
                        _result = new Integer(_integer.Value % integer.Value);
                        break;
                    case Operation.RShift:
                        _result = new Integer(_integer.Value >> integer.Value);
                        break;
                    case Operation.LShift:
                        _result = new Integer(_integer.Value << integer.Value);
                        break;
                    case Operation.BitWiseAnd:
                        _result = new Integer(_integer.Value & integer.Value);
                        break;
                    case Operation.BitWiseOr:
                        _result = new Integer(_integer.Value | integer.Value);
                        break;

                }
            }

            public void VisitFloat(Floating floating)
            {
                switch (_op)
                {
                    case Operation.Add:
                        _result = new Floating(_integer.Value + floating.Value);
                        break;
                    case Operation.Subtract:
                        _result = new Floating(_integer.Value - floating.Value);
                        break;
                    case Operation.Multiply:
                        _result = new Floating(_integer.Value * floating.Value);
                        break;
                    case Operation.Divide:
                        _result = new Floating(_integer.Value / floating.Value);
                        break;
                    case Operation.Power:
                        _result = new Floating(Math.Pow(_integer.Value, floating.Value));
                        break;
                    case Operation.Modulus:
                    case Operation.IntDiv:
                    case Operation.Remainder:
                    case Operation.RShift:
                    case Operation.LShift:
                    case Operation.BitWiseAnd:
                    case Operation.BitWiseOr:
                        throw new TypeMismatchException(ValidTypes.Integer, floating,null);

                }
            }

            public void VisitStruct(Structure structure)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void VisitList(PrologList list)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        class FloatCalculator : ITermVisitor
        {

            Term _result;
            Floating _floating;
            Operation _op;

            public FloatCalculator(Floating floating, Operation op)
            {
                _op = op;
                _floating = floating;
            }

            public Term Execute(Term other)
            {
                if (other != null)
                {
                    other.Accept(this);
                }
                else
                {
                    DoUnary();
                }
                return _result;
            }


            private void DoUnary()
            {
                switch (_op)
                {
                    case Operation.Subtract:
                        _result = new Floating(-_floating.Value);
                        break;
                    case Operation.Add:
                        _result = _floating;
                        break;
                    case Operation.Abs:
                        _result = new Floating(Math.Abs(_floating.Value));
                        break;
                    case Operation.Sign:
                        _result = new Floating(Math.Sign(_floating.Value));
                        break;
                    case Operation.IntegerPart:
                        _result = new Integer((int)_floating.Value);
                        break;
                    case Operation.FractionalPart:
                        _result = new Floating(_floating.Value-(double)((int)_floating.Value));
                        break;
                    case Operation.Float:
                        _result = _floating;
                        break;
                    case Operation.Floor:
                        _result = new Integer((int)Math.Floor(_floating.Value));
                        break;
                    case Operation.Round:
                        _result = new Integer((int)Math.Round(_floating.Value));
                        break;
                    case Operation.Ceiling:
                        _result = new Integer((int)Math.Ceiling(_floating.Value));
                        break;
                    case Operation.Sin:
                        _result = new Floating(Math.Sin(_floating.Value));
                        break;
                    case Operation.Cos:
                        _result = new Floating(Math.Cos(_floating.Value));
                        break;
                    case Operation.Atan:
                        _result = new Floating(Math.Atan(_floating.Value));
                        break;
                    case Operation.Exp:
                        _result = new Floating(Math.Exp(_floating.Value));
                        break;
                    case Operation.Log:
                        _result = new Floating(Math.Log(_floating.Value,Math.E));
                        break;
                    case Operation.Sqrt:
                        _result = new Floating(Math.Sqrt(_floating.Value));
                        break;
                    case Operation.BitWiseComplement:
                        throw new TypeMismatchException(ValidTypes.Integer, _floating,null);

                }
            }

            #region ITermVisitor Members

            public void VisitVar(Var var)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void VisitInteger(Integer integer)
            {
                switch (_op)
                {
                    case Operation.Add:
                        _result = new Floating(_floating.Value + integer.Value);
                        break;
                    case Operation.Subtract:
                        _result = new Floating(_floating.Value - integer.Value);
                        break;
                    case Operation.Multiply:
                        _result = new Floating(_floating.Value * integer.Value);
                        break;
                    case Operation.Divide:
                        _result = new Floating(_floating.Value / integer.Value);
                        break;
                    case Operation.Power:
                        _result = new Floating(Math.Pow(_floating.Value, integer.Value));
                        break;
                    case Operation.Modulus:
                    case Operation.IntDiv:
                    case Operation.Remainder:
                    case Operation.RShift:
                    case Operation.LShift:
                    case Operation.BitWiseAnd:
                    case Operation.BitWiseOr:
                        throw new TypeMismatchException(ValidTypes.Integer, _floating,null);
                }
            }

            public void VisitFloat(Floating floating)
            {
                switch (_op)
                {
                    case Operation.Add:
                        _result = new Floating(_floating.Value + floating.Value);
                        break;
                    case Operation.Subtract:
                        _result = new Floating(_floating.Value - floating.Value);
                        break;
                    case Operation.Multiply:
                        _result = new Floating(_floating.Value * floating.Value);
                        break;
                    case Operation.Divide:
                        _result = new Floating(_floating.Value / floating.Value);
                        break;
                    case Operation.Power:
                        _result = new Floating(Math.Pow(_floating.Value, floating.Value));
                        break;
                    case Operation.Modulus:
                    case Operation.IntDiv:
                    case Operation.Remainder:
                    case Operation.RShift:
                    case Operation.LShift:
                    case Operation.BitWiseAnd:
                    case Operation.BitWiseOr:
                        throw new TypeMismatchException(ValidTypes.Integer, _floating,null);
                }
            }

            public void VisitStruct(Structure structure)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void VisitList(PrologList list)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }


        public enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Power,
            Modulus,
            IntDiv,
            Remainder,
            Abs,
            Sign,
            IntegerPart,
            FractionalPart,
            Float,
            Floor,
            Round,
            Ceiling,
            Sin,
            Cos,
            Atan,
            Exp,
            Log,
            Sqrt,
            RShift,
            LShift,
            BitWiseAnd,
            BitWiseOr,
            BitWiseComplement
        }
    }
}
