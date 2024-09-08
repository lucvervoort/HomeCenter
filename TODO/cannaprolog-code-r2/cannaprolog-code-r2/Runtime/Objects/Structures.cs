/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Canna.Prolog.Runtime.Builtins;
using System.IO;

namespace Canna.Prolog.Runtime.Objects
{
    /// <summary>
    /// Structure is the class for prolog functors (f(a,X)), atoms ( a ) and lists ([a,b,c])
    /// </summary>
    public class Structure : Term
    {
        /// <summary>
        /// The name of the Structure. i.e. f(a,b) the name is "f".
        /// </summary>
        string m_name;

        /// <summary>
        /// The arguments of the functor: a list of Terms.
        /// </summary>
        protected TermList args;

        public static Structure True = new Structure("true");
        public static Structure False = new Structure("false");

        void Init()
        {
            args = new TermList();
        }

        /// <summary>
        /// Build a functor with no arguments
        /// </summary>
        /// <param name="name">The name of the functor.</param>
        public Structure(string name)
        {
            Init();
            m_name = name;
        }

        public Structure(string name, params Term[] terms):this(name)
        {
            foreach (Term term in terms)
            {
                Args.Add(term);
            }
            Debug.Assert((name != "."||Arity!=2) || this.GetType() == typeof(PrologList));

        }
        /// <summary>
        /// Builds a functor with two arguments. i.e. f(a,b).
        /// </summary>
        /// <param name="name">The name of the functor.</param>
        /// <param name="t1">First arg.</param>
        /// <param name="t2">Second arg.</param>
        public Structure(string name, Term t1, Term t2)
        {
            Init();
            m_name = name;
            AddArg(t1);
            AddArg(t2);
        }

        public override bool IsAtom
        {
            get
            {
                return Arity == 0;
            }
        }

        public override bool IsCompound
        {
            get
            {
                return Arity > 0;
            }
        }

        public override bool IsList
        {
            get
            {
                return m_name.Equals(".");
            }
        }

        public override bool IsOp
        {
            get
            {
                return Op.isOp(Name);
            }
        }

        public override bool IsGround
        {
            get
            {
                foreach (Term t in Args)
                {
                    if (!t.IsGround)
                        return false;
                }
                return true;
            }

        }

        public bool IsPredicateIndicator
        {
            get
            {
                return ((Arity == 2)&&(Name == "/"));
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }

            internal set
            {
                m_name = value;
            }
        }


        public int Arity
        {
            get
            {
                return args.Count;
            }
        }


        public TermList Args
        {
            get
            {
                return args;
            }
        }

        //public Term getArgAt(int i)
        //{
        //    return (Term)args[i];
        //}

        public Term this[int i]
        {
            get
            {
                return (Term)args[i];
            }
        }

        public void AddArg(Term t)
        {
            args.Add(t);
        }







       private  string Normalize(string name)
        {
           //ctrlescapes
            name = name.Replace("\x07", @"\a");
            name = name.Replace("\x08", @"\b");
            name = name.Replace("\x0C", @"\f");
            name = name.Replace("\n", @"\n");
            name = name.Replace("\r", @"\r");
            name = name.Replace("\t", @"\t");
            name = name.Replace("\x0B", @"\v");
            name = name.Replace("'", @"\'");
            name = name.Replace("\"", @"\""");
            name = name.Replace("`", @"\`");
           return name;
       }

        private bool IsUnquotedAtom(string Name)
        {
            if (Char.IsLower(Name[0]))
            {
                foreach (char c in Name)
                {
                    if (!Char.IsLetterOrDigit(c) && c != '_')
                        return false;
                }
                return true;
            }
            else
            {
                if (IsOp)
                    return true;
            }
                return false;
        }


        private void WriteOp(StreamTerm output, WriteOptions options)
        {
            output.Write('(');
            if (Arity == 2)
            {
                this[0].Write(output, options);
                output.Write(' ');
                output.Write(Name);
                output.Write(' ');
                this[1].Write(output, options);
            }
            else if (Arity == 1)
            {
                if (Op.isPostfix(Name))//postfix
                {
                    output.Write(' ');
                    this[0].Write(output, options);
                    output.Write(' ');
                    output.Write(Name);
                }
                else //prefix
                {
                    output.Write(' ');
                    output.Write(Name);
                    output.Write(' ');
                    this[0].Write(output, options);
                }

            }
             output.Write(')');

        }


        public override void Write(StreamTerm output, WriteOptions options)
        {
            if (IsOp && !options.ignore_ops  && Arity>0)
            {
                WriteOp(output, options);
                return;
            }
            if (Name == "{}" && !options.ignore_ops)
            {
                WriteCurly(output, options);
                return;
            }
            //TODO: use quoted
            WriteAtom(Name, output, options);
            if (Arity > 0)
            {
                output.Write('(');
                for (int i = 0; i < Arity; i++)
                {
                    this[i].Write(output, options);
                    if (i < (Arity - 1))
                    {
                        output.Write(',');
                    }

                }
                output.Write(')');
            }
        }

        private void WriteAtom(string atom, StreamTerm output, WriteOptions options)
        {
            if (!options.quoted || IsUnquotedAtom(atom))
            {
                output.Write(atom);
                return;
            }
            else
            {
                output.Write('\'');
                output.Write(Normalize(atom));
                output.Write('\'');
            }
        }

        private void WriteCurly(StreamTerm output, WriteOptions options)
        {
            output.Write('{');
            foreach (Term t in Args)
            {
                t.Write(output, options);
            }
            output.Write('}');
        }

        public override bool Unify(Term that, VarList boundedvars, bool occurCheck)
        {
            return that.UnifyWithStructure(this, boundedvars,false);
        }

        public override bool UnifyWithStructure(Structure f, VarList boundedvars, bool occurCheck)
        {
            if (f.Name == Name)
            {
                if (f.Arity == Arity)
                {
                    IEnumerator e1 = f.Args.GetEnumerator();
                    IEnumerator e2 = Args.GetEnumerator();
                    while (e1.MoveNext() && e2.MoveNext())
                    {
                        Term t1 = (Term)e1.Current;
                        Term t2 = (Term)e2.Current;
                        if (!t1.Unify(t2, boundedvars, false))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }//unifyStructure

        public override bool UnifyWithVar(Var v, VarList boundedvars, bool occurCheck)
        {

            return v.UnifyWithStructure(this, boundedvars,occurCheck);
        }

        public override bool UnifyWithInteger(Integer i, VarList boundedvars, bool occurCheck)
        {
            return false;
        }
        public override bool UnifyWithFloating(Floating f, VarList boundedvars, bool occurCheck)
        {
            return false;
        }

        public override VarList GetFreeVariables()
        {
            VarList list = new VarList();
            foreach (Term t in Args)
            {
                if (t == null) Debugger.Break();
                list.AddRange(t.GetFreeVariables());
            }
            return list;
        }

        public override Term Dereference()
        {
            Structure f = new Structure(Name);
            foreach (Term t in Args)
            {
                f.AddArg(t.Dereference());
            }
            return f;
        }

        public override Term Copy(Variables v)
        {
            Structure f = new Structure(Name);
            foreach (Term t in Args)
            {
                f.AddArg(t.Copy(v));
            }
            return f;
        }

        public override bool DoOccurCheck(Var v)
        {
            foreach (Term t in Args)
            {
                if (t == v) return true;
                if (t.DoOccurCheck(v)) return true;
            }
            return false;
        }

        

        public override void Unbind()
        {

            Args.Unbind();
        }

        public override void Accept(ITermVisitor visitor)
        {
            visitor.VisitStruct(this);
        }


        public PredicateIndicator GetPI()
        {
            return new PredicateIndicator(Name, Arity);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Arity;
        }

        public override object ObjectValue
        {
            get
            {
                if (IsAtom)
                    return Name;
                else
                {
                    ArrayList list = new ArrayList();
                    list.Add(Name);
                    foreach (Term t in Args)
                    {
                        list.Add(t.ObjectValue);
                    }
                    return list;
                }
            }
        }
    }


    public class PredicateIndicator
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int arity;

        public int Arity
        {
            get { return arity; }
            set { arity = value; }
        }

        public PredicateIndicator(string name, int arity)
        {
            Name = name;
            Arity = arity;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name);
            sb.Append("/");
            sb.Append(Arity);
            return sb.ToString();
        }

        public string GetPredicateName()
        {
            StringBuilder sb = new StringBuilder(Name);
            sb.Append("_");
            sb.Append(Arity);
            return sb.ToString();
        }

        public static PredicateIndicator FromTerm(Term pi, object context)
        {
            Structure str = pi as Structure;
            if (str == null || !str.IsPredicateIndicator)
            {
                throw new TypeMismatchException(ValidTypes.PredicateIndicator, pi,context);
            }
            if ((!str[0].IsBound) || (!str[1].IsBound))
            {
                throw new InstantiationException(context);
            }
            if (!str[0].IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, str[0],context);
            }
            Integer arity = str[1] as Integer;
            if (arity == null)
            {
                throw new TypeMismatchException(ValidTypes.Integer, str[1],context);
            }
            if (arity.Value < 0)
            {
                throw new DomainException(ValidDomains.not_less_than_zero, arity, context);
            }
            PredicateIndicator _pi = new PredicateIndicator(((Structure)str[0]).Name, arity.Value);

            return _pi;
        }

        public Structure GetPITerm()
        {
            return new Structure("/", new Structure(Name), new Integer(Arity));
        }
    }

}
