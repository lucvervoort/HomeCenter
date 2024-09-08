/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Canna.Prolog.Runtime.Objects
{
    /// <summary>
    /// Var is a prolog variable (X)
    /// </summary>

    public class Var : Term
    {
        string m_name;
        static int s_varid;
        protected int m_id;
        Term m_ref;
        public Var(string name)
        {
            m_name = name;
            m_id = s_varid++;
            Unbind();
            Anonymous = false;
        }

        public Var()
        {
            m_id = s_varid++;
            m_name = "_" + m_id;
            Unbind();
            Anonymous = false;
        }

        public override void Unbind()
        {
            m_ref = this;
        }

        public void bindTo(Term t)
        {
            m_ref = t;
        }

        public override Term Dereference()
        {
            if (IsBound)
            {
                return m_ref.Dereference();
            }
            else
            {
                return this;
            }
        }

        public override bool IsBound
        {
            get
            {
                return m_ref != this;
            }
        }

        public override bool IsGround
        {
            get
            {
                if (m_ref != this)
                    return m_ref.IsGround;
                else
                    return false;
            }
        }


        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public override bool Unify(Term that, VarList boundedvars, bool occurCheck)
        {
                return that.UnifyWithVar(this, boundedvars, occurCheck);
        }
        public override bool UnifyWithStructure(Structure f, VarList boundedvars, bool occurCheck)
        {
            if (IsBound)
            {
                return Dereference().UnifyWithStructure(f, boundedvars,occurCheck);
            }
            else
            {
                if (occurCheck)
                {
                    if (f.DoOccurCheck(this)) return false;
                }
                bindTo(f);
                boundedvars.Add(this);
            }
            return true;
        }//
        public override bool UnifyWithVar(Var v, VarList boundedvars, bool occurCheck)
        {
            if (IsBound)
            {
                if (v.IsBound)
                {
                    return Dereference().Unify(v.Dereference(), boundedvars,occurCheck);
                }
                else
                {
                    v.bindTo(this);
                    boundedvars.Add(v);
                    return true;
                }

            }
            else //we're not bound
            {
                if (m_id < v.m_id)
                {
                    bindTo(v);
                    boundedvars.Add(this);
                }
                else
                {
                    if (!v.IsBound)
                    {
                        v.bindTo(this);
                        boundedvars.Add(v);
                    }
                    else
                    {
                        bindTo(v);
                        boundedvars.Add(this);
                    }
                }

            }
            return true;
        }
        public override bool UnifyWithInteger(Integer i, VarList boundedvars, bool occurCheck)
        {
            if (IsBound)
            {
                return Dereference().Unify(i, boundedvars,occurCheck);
            }
            else
            {
                bindTo(i);
                boundedvars.Add(this);
                return true;
            }
        }
        public override bool UnifyWithFloating(Floating f, VarList boundedvars, bool occurCheck)
        {
            if (IsBound)
            {
                return Dereference().Unify(f, boundedvars,occurCheck);
            }
            else
            {
                bindTo(f);
                boundedvars.Add(this);
                return true;
            }
        }

        public override bool UnifyWithObject(ObjectTerm ob, VarList boundedvars, bool occurCheck)
        {
            if (IsBound)
            {
                return base.UnifyWithObject(ob,boundedvars,occurCheck);
            }
            else
            {
                bindTo(ob);
                boundedvars.Add(this);
                return true;
            }
        }

        public override VarList GetFreeVariables()
        {
            VarList list = new VarList();
            if (!IsBound)
            {
                list.Add(this);
            }
            return list;
        }

        public override Term Copy(Variables v)
        {
            return v.getVar(this);
        }

       

        public override void Accept(ITermVisitor visitor)
        {
            visitor.VisitVar(this);
        }

        /// used to check for singletons
        public int NRef
        {
            get
            {

                return m_references;
            }
            set
            {
                m_references = value;
            }
        }

        protected int m_references;

        public bool Anonymous
        {
            get
            {

                return m_bAnonymous;
            }
            set
            {
                m_bAnonymous = value;
            }
        }

        protected bool m_bAnonymous;

        public override void Write(StreamTerm output, WriteOptions options)
        {
            //TODO: use numbervars
            output.Write(Name);
        }

        public override object ObjectValue
        {
            get
            {
                if (this.IsBound)
                    return this.Dereference().ObjectValue;
                else
                    return null;
            }
        }


    }

    /// <summary>
    /// Variables holds the variables in a clause
    /// </summary>
    public class Variables : Dictionary<string, Var>
    {
        //Hashtable m_vars;
        public bool keepnames;
        public Variables()
        {
            keepnames = false;
        }
        /// <summary>
        /// Returns a Variable object given a parsed variabled.
        /// </summary>
        /// <remarks>
        /// The variable passed is compared with the ones already parsed in the clause, 
        /// and stored in this object, if it's already there, the same reference is returned,
        /// otherwise a new one is created.
        /// </remarks>
        /// <param name="v">The variable parsed.</param>
        /// <returns>A Var object.</returns>
        public Var getVar(Var v)
        {
            return getVar(v.Name);
        }


        public Var getVar(string name)
        {
            // Is it an anonymous variable?
            if (name.Equals("_"))
            {
                Var newvar = new Var();
                newvar.Anonymous = true;
                this.Add(newvar.Name, newvar);
                return newvar;
            }
            //Or is it a variable already present in this clause's var list?
            else if (this.ContainsKey(name))
            {
                Var var = this[name];
                var.NRef++;
                return var;
            }
            //It's a new one.
            else
            {
                Var newvar;
                if (keepnames)
                {
                    newvar = new Var(name);
                }
                else
                {
                    newvar = new Var();
                }
                newvar.NRef = 1;
                this.Add(name, newvar);
                return newvar;
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            int i = 1;
            foreach (KeyValuePair<string, Var> en in this)
            {
                s.Append(en.Key);
                s.Append(" = ");
                s.Append(((Var)en.Value).Dereference());
                if (i++ < this.Count)
                {
                    s.Append("\r\n");
                }
            }
            return s.ToString();
        }

        public PrologList getSingletons()
        {
            PrologList list = PrologList.EmptyList;

            foreach (KeyValuePair<string, Var> entry in this)
            {
                if (!entry.Value.Anonymous)
                {
                    if (entry.Value.NRef == 1)
                    {
                        PrologList newitem = new PrologList( (new Var(entry.Key)));
                        list = list.Append(newitem);
                    }
                }

            }
            return list;
        }

        public PrologList ToPrologList()
        {
            PrologList plist = new PrologList();
            foreach (Var var in Values)
            {
                plist = plist.Append(new PrologList(var));
            }
            return plist;
        }

    }


    
}
