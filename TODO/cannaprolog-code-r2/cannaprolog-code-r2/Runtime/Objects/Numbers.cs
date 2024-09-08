using System;
using System.Collections.Generic;
using System.Text;

namespace Canna.Prolog.Runtime.Objects
{

    public class Number : Const
    {
        public override bool IsNumber
        {
            get
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Represent an integer number
    /// </summary>
    public class Integer : Number
    {
        Int32 m_value;
        public Integer(int val)
        {
            m_value = val;
        }

        public Int32 Value
        {
            get
            {
                return m_value;
            }

            internal set
            {
                m_value = value;
            }
        }


        public override bool Unify(Term that, VarList boundedvars, bool occurCheck)
        {
            return that.UnifyWithInteger(this, boundedvars, occurCheck);
        }

        public override bool UnifyWithStructure(Structure f, VarList boundedvars, bool occurCheck)
        {
            return false;
        }//

        public override bool UnifyWithVar(Var v, VarList boundedvars, bool occurCheck)
        {
            return v.UnifyWithInteger(this, boundedvars, occurCheck);
        }

        public override bool UnifyWithInteger(Integer i, VarList boundedvars, bool occurCheck)
        {
            return i.Value == Value;
        }
        public override bool UnifyWithFloating(Floating f, VarList boundedvars, bool occurCheck)
        {
            return false;
        }

        public override void Accept(ITermVisitor visitor)
        {
            visitor.VisitInteger(this);
        }

        public override bool IsInteger
        {
            get
            {
                return true;
            }
        }

        public override void Write(StreamTerm output, WriteOptions options)
        {
            output.Write(m_value);
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }

        public override object ObjectValue
        {
            get
            {
                return m_value;
            }
        }

    }
    /// <summary>
    /// represent a floating point number
    /// </summary>
    public class Floating : Number
    {
        Double m_value;

        public Floating(double val)
        {
            m_value = val;
        }

        public Double Value
        {
            get
            {
                return m_value;
            }

            internal set
            {
                m_value = value;
            }
        }



        public override bool Unify(Term that, VarList boundedvars, bool occurCheck)
        {
            return that.UnifyWithFloating(this, boundedvars, occurCheck);
        }

        public override bool UnifyWithStructure(Structure f, VarList boundedvars, bool occurCheck)
        {
            return false;
        }//

        public override bool UnifyWithVar(Var v, VarList boundedvars, bool occurCheck)
        {
            return v.UnifyWithFloating(this, boundedvars, occurCheck);
        }

        public override bool UnifyWithInteger(Integer i, VarList boundedvars, bool occurCheck)
        {
            return false;
        }
        public override bool UnifyWithFloating(Floating f, VarList boundedvars, bool occurCheck)
        {

            return f.Value == Value;
        }

        public override void Accept(ITermVisitor visitor)
        {
            visitor.VisitFloat(this);
        }

        public override void Write(StreamTerm output, WriteOptions options)
        {
            output.Write(m_value);
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }

        public override object ObjectValue
        {
            get
            {
                return m_value;
            }
        }

    }
}
