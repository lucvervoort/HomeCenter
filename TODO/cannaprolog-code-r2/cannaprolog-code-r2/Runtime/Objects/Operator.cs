/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Builtins;

namespace Canna.Prolog.Runtime.Objects
{
    [Flags]
    public enum Specifier
    {
        xfx=0x0001,
        xfy=0x0002,
        yfx=0x0004,
        xf=0x0010,
        yf=0x0020,
        fx=0x0040,
        fy=0x0080,
        delimiter=0x0100,
        term=0x1000,
        lterm=0x3000

    }

    /// <summary>
    /// Op represent an operator characteristic
    /// </summary>
    public class Op
    {

        //private static Dictionary<string, Op> m_binaryOps = new Dictionary<string, Op>();
        //private static Dictionary<string, Op> m_unaryOps = new Dictionary<string, Op>();

        private static Dictionary<string, Op> _operators = new Dictionary<string, Op>();

        public string Name;
        public Specifier Type;
        public int PrefixPriority;
        public int InfixPriority;
        public int PostfixPriority;

        //Static Constructor
        static Op()
        {
            //define some operators
            Makeop(200, Specifier.xfy, "^");
            Makeop(200, Specifier.xfx, "**"); //Power
            Makeop(400, Specifier.yfx, "*");
            Makeop(400, Specifier.yfx, "/");
            Makeop(400, Specifier.yfx, "//"); //integer division
            Makeop(400, Specifier.yfx, "rem"); //remainder
            Makeop(400, Specifier.yfx, "mod"); //modulus
            Makeop(400, Specifier.yfx, "<<");
            Makeop(400, Specifier.yfx, ">>");
            Makeop(500, Specifier.yfx, "+");
            Makeop(500, Specifier.yfx, "-");
            Makeop(500, Specifier.yfx, "/\\"); //bitwise and
            Makeop(500, Specifier.yfx, "\\/"); //bitwise or
            Makeop(500, Specifier.fx, "\\"); //bitwise or
            Makeop(500, Specifier.fx, "-"); //Infix versione
            Makeop(700, Specifier.xfx, "=");
            Makeop(700, Specifier.xfx, @"\=");
            Makeop(700, Specifier.xfx, "<");
            Makeop(700, Specifier.xfx, ">");
            Makeop(700, Specifier.xfx, "<=");
            Makeop(700, Specifier.xfx, ">=");
            Makeop(700, Specifier.xfx, @"=\=");
            Makeop(700, Specifier.xfx, "=:=");

            Makeop(700, Specifier.xfx, "@=<");
            Makeop(700, Specifier.xfx, "==");
            Makeop(700, Specifier.xfx, @"\==");
            Makeop(700, Specifier.xfx, "@<");
            Makeop(700, Specifier.xfx, "@>");
            Makeop(700, Specifier.xfx, "@>=");

            Makeop(700, Specifier.xfx, "=..");

            //TODO: qual'è la vera priorità di 'is'?
            Makeop(800, Specifier.xfx, "is");
            Makeop(900, Specifier.fy, "\\+");
            Makeop(1000, Specifier.xfy, ",");
            Makeop(1050, Specifier.xfy, "->");
            Makeop(1050, Specifier.xfy, "*->");
            //Comment to simplify prolog list parsing
            Makeop(1100, Specifier.xfy, "|");
            Makeop(1100, Specifier.xfy, ";");
            Makeop(1200, Specifier.xfx, ":-");
            Makeop(1200, Specifier.fx, ":-"); //for directives
        }

        public Op(int prec, Specifier type, string name)
        {
            Name = name;
            Type = type;

            SetPriority(prec, type);
  
        }

        private void SetPriority(int pri, Specifier type)
        {
            if (IsInfix(type))
            {
                InfixPriority = pri;
            }
            else if (IsPostfix(type))
            {
                PostfixPriority = pri;
            }
            else
            {
                PrefixPriority = pri;
            }
            this.Type |= type;
        }

        public static bool isOp(string name)
        {
            return _operators.ContainsKey(name);
        }

        public static Op GetOp(string name)
        {
            return _operators[name];
        }

        public static bool isBinary(string name)
        {
            if (_operators.ContainsKey(name))
            {
                Specifier spec = _operators[name].Type;
                return IsInfix(spec);
            }
            return false;
        }
        public static bool isUnary(string name)
        {
            if (_operators.ContainsKey(name))
            {
                Specifier spec = _operators[name].Type;
                return IsPostfix(spec)||IsPrefix(spec);
            }
            return false;

        }
        public static int prec(string name)
        {
            if (_operators.ContainsKey(name))
            {
                Op op = _operators[name];
                //hack:
                int ret = op.PrefixPriority;
                if (ret == 0)
                    ret = op.PostfixPriority;
                if (ret == 0)
                    ret = op.InfixPriority;
                return ret;
            }
           

            return -1;
        }

        public static bool isLeftAssoc(string name)
        {
            if (_operators.ContainsKey(name))
            {
                Op op = _operators[name];
                return op.Type == Specifier.yfx;  
            }
            return false;
        }

        public static bool isRightAssoc(string name)
        {
            if (_operators.ContainsKey(name))
            {
                Op op = _operators[name];
                return op.Type == Specifier.xfy;
            }
            return false;
        }

        public static bool isPostfix(string name)
        {
            if (_operators.ContainsKey(name))
            {
                return IsPostfix(_operators[name].Type);
            }
            return false;
        }

        public static bool IsValiOpSpecifier(string opspec)
        {
            try
            {
                Specifier spec = (Specifier)Enum.Parse(typeof(Specifier), opspec);
                return true;
            }
            catch
            {
                return false;
            }
            /*
            if (opspec.Length < 2 || opspec.Length > 3)
                return false;
            if (opspec.StartsWith("x") || opspec.StartsWith("y"))
            {
                if (opspec[1] != 'f')
                    return false;
                if (opspec.Length == 2) return true;
                if (opspec[2] == 'x') return true;
                if (opspec[2] == 'y' && opspec[0]=='x') return true;
                return false;
            }
            else if (opspec.StartsWith("f"))
            {
                if (opspec.Length != 2) return false;
                if (opspec[1] == 'x' || opspec[1] == 'y') return true;
                return false;
            }
            return false;
            */
        }

        public static bool IsPrefix(Specifier spec)
        {
            return 0 != (spec & (Specifier.fx | Specifier.fy));
        }

        public static bool IsPostfix(Specifier spec)
        {
            return 0 != (spec & (Specifier.xf |  Specifier.yf));
        }

        public static bool IsInfix(Specifier spec)
        {
            return 0 != (spec & (Specifier.xfx | Specifier.yfx |Specifier.xfy));
        }

        public static bool IsNotAssoc(Specifier spec)
        {
            return 0 != (spec & Specifier.xfx);
        }

        public static bool IsLeftAssoc(Specifier spec)
        {
            return 0 != (spec & Specifier.yfx);
        }

        public static bool IsRightAssoc(Specifier spec)
        {
            return 0 != (spec & Specifier.xfy);
        }

        public static void Makeop(int prec, Specifier type, string name)
        {
            
            if (!_operators.ContainsKey(name))
            {
                _operators.Add(name, new Op(prec, type, name));
            }
            else
            {
                if (prec == 0)
                {
                    _operators.Remove(name);
                }
                else
                {
                    _operators[name].SetPriority(prec, type);
                }
            }

        }

        internal static IEnumerator<Op> GetAllOperators()
        {
            foreach (Op op in _operators.Values)
            {
                if (op.InfixPriority > 0)
                {
                    yield return new Op(op.InfixPriority,op.Type & (Specifier.xfx|Specifier.xfy|Specifier.yfx),op.Name);
                }
                if (op.PostfixPriority > 0)
                {
                    yield return new Op(op.PostfixPriority, op.Type & (Specifier.xf | Specifier.yf), op.Name);
                }
                if (op.PrefixPriority > 0)
                {
                    yield return new Op(op.PrefixPriority, op.Type & (Specifier.fx | Specifier.fy), op.Name);
                }

            }
            
        }
    }
}
