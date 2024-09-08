/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

namespace Canna.Prolog.Runtime.Objects
{
	using System;
	using System.Collections;
    using System.Collections.Generic;
	using System.Text;
    using System.IO;

/// <summary>
/// PrologException
/// </summary>
	
	public class PrologException: ApplicationException
	{

        public PrologException()
        { }

		public PrologException(string message):base(message)
		{
		}
	}







/// <summary>
/// Term is the base class for all prolog objects
/// </summary>
	public class Term 
	{
		/// <summary>
		/// Says if this object is an operator.
		/// </summary>
		/// <returns>true or false</returns>
		public virtual bool IsOp
		{
            get
            {
			    return false;
            }
		}
		/// <summary>
		/// Says if this object is an atom i.e.. dog.
		/// </summary>
		/// <returns>true or false</returns>
		public virtual bool IsAtom
		{
            get {
			return false;
            }
		}

        /// <summary>
        /// 
        /// 
        /// Says if this object is an atom i.e.. dog.
        /// </summary>
        /// <returns>true or false</returns>
        public virtual bool IsCompound
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// Says if this object is a List.
		/// </summary>
		/// <returns>true or false</returns>
		public virtual bool IsList
		{
            get
            {
                return false;
            }
		}

		/// <summary>
		/// Says if this object is bound.
		/// </summary>
		/// <returns>true or false</returns>
		public virtual bool IsBound
		{
            get
            {
                return true;
            }
		}

		/// <summary>
		/// Says if this object is Ground.
		/// </summary>
		/// <returns>true or false</returns>
		public virtual bool IsGround
		{
            get
            {
                return true;
            }
		}

        public virtual bool IsNumber
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsInteger
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// Method to unify terms.
		/// </summary>
		/// <param name="that">The other term to unify with.</param>
		/// <param name="boundedvars">A Structure used to store all the variables
		/// that were bound during the unification process.</param>
		/// <returns>True if the two terms unify, false otherwise.</returns>
		public virtual bool Unify(Term that, VarList boundedvars, bool occurCheck)
		{
			return false;
		}
		/// <summary>
		/// A specialized unification method for unifying with a Structure.
		/// </summary>
        /// <param name="f">The Structure to unify with.</param>
		/// <param name="boundedvars">A Structure used to store all the variables
		/// that were bound during the unification process.</param>
		/// <returns>True if the two terms unify, false otherwise.</returns>
        public virtual bool UnifyWithStructure(Structure f, VarList boundedvars, bool occurCheck)
		{
			return false;
		}
		/// <summary>
		/// A specialized unification method for unifying with a Variable.
		/// </summary>
		/// <param name="v">The Variable to unify with.</param>
		/// <param name="boundedvars">A Structure used to store all the variables
		/// that were bound during the unification process.</param>
		/// <returns>True if the two terms unify, false otherwise.</returns>
        public virtual bool UnifyWithVar(Var v, VarList boundedvars, bool occurCheck)
		{
			return false;
		}
		/// <summary>
		/// A specialized unification method for unifying with an Integer.
		/// </summary>
		/// <param name="i">The Integer to unify with.</param>
		/// <param name="boundedvars">A Structure used to store all the variables
		/// that were bound during the unification process.</param>
		/// <returns>True if the two terms unify, false otherwise.</returns>
        public virtual bool UnifyWithInteger(Integer i, VarList boundedvars, bool occurCheck)
		{
			return false;
		}
		/// <summary>
		/// A specialized unification method for unifying with an Floating.
		/// </summary>
		/// <param name="f">The Floating to unify with.</param>
		/// <param name="boundedvars">A Structure used to store all the variables
		/// that were bound during the unification process.</param>
		/// <returns>True if the two terms unify, false otherwise.</returns>
        public virtual bool UnifyWithFloating(Floating f, VarList boundedvars, bool occurCheck)
		{
			return false;
		}

        /// <summary>
        /// A specialized unification method for unifying with an Object Term.
        /// </summary>
        /// <param name="f">The Object to unify with.</param>
        /// <param name="boundedvars">A Structure used to store all the variables
        /// that were bound during the unification process.</param>
        /// <returns>True if the two terms unify, false otherwise.</returns>
        public virtual bool UnifyWithObject(ObjectTerm ob, VarList boundedvars, bool occurCheck)
        {
           bool b = Type.Equals(ob.ObjectValue, ObjectValue);
           if (!b)
           {
               b = ob.ObjectValue.ToString() == ObjectValue.ToString();
           }
            return b;
        }

		/// <summary>
		/// Dereference this Term.
		/// </summary>
		/// <returns>A new, dereferenced Term</returns>
		public virtual Term Dereference()
		{
			return this;
		}
		/// <summary>
		/// Retrieves all the unbound variables contained in this Term.
		/// </summary>
		/// <param name="list">The Structure used to accomodate all the unbound variables.</param>
		public virtual VarList GetFreeVariables()
		{
            return new VarList();
		}
		/// <summary>
		/// Copies this term.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public virtual Term Copy(Variables v)
		{
			return this; //completely useless
		}

        public virtual void Accept(ITermVisitor visitor)
        { }

        public virtual bool DoOccurCheck(Var v)
        {
            return false;
        }

		

		public virtual void Unbind()
		{
			
		}


        public virtual void Write(StreamTerm output, WriteOptions options){
            output.Write("Term");
        }


        public override string ToString()
        {
           
            return ToString(new WriteOptions());
            
        }

        public string ToString(WriteOptions ops)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriterTerm swt = new StreamWriterTerm(ms);
            Write(swt, ops);
            return Encoding.Default.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        }

        public virtual object ObjectValue
        {
            get { return null; }
        }

        public virtual bool IsObject
        {
            get { return false; }
        }
	}

    public class WriteOptions
    {
        public bool quoted;
        public bool ignore_ops;
        public bool numbervars;

        public WriteOptions()
        {

        }

        public WriteOptions(bool quoted, bool ignore_ops, bool numbervars)
        {
            this.quoted = quoted;
            this.ignore_ops = ignore_ops;
            this.numbervars = numbervars;
        }

    }


	
	/// <summary>
	/// Just in case...
	/// </summary>
	public class Const : Term
	{
		
	}




    public abstract class ObjectTerm : Term
    {
        public override bool Unify(Term that, VarList boundedvars, bool occurCheck)
        {
            return that.UnifyWithObject(this, boundedvars, occurCheck);
        }

        public override bool UnifyWithVar(Var v, VarList boundedvars, bool occurCheck)
        {
            return v.UnifyWithObject(this, boundedvars, occurCheck);
        }

        public override bool UnifyWithObject(ObjectTerm ob, VarList boundedvars, bool occurCheck)
        {
            return ob.ObjectValue.Equals(ObjectValue);

        }

        public override bool IsObject
        {
            get
            {
                return true;
            }
        }


        public override void Write(StreamTerm output, WriteOptions options)
        {
            output.Write(this.ObjectValue.ToString());
        }
    }
 


    public class GenericObjectTerm<T> : ObjectTerm
    {
        T _theObject;

        public T Value
        {
            get { return _theObject; }
            set { _theObject = value; }
        }


        public GenericObjectTerm(T theObject)
        {
            _theObject = theObject;
        }

        public override object ObjectValue
        {
            get { return _theObject; }
        }
}
}