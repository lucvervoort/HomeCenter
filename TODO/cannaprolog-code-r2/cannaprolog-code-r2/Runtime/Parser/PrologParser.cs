/* *******************************************************************
 * Copyright (c) 2005, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/


namespace Canna.Prolog.Runtime.Parser
{
	using System;
	using System.Collections;
    using Canna.Prolog.Runtime.Objects;
    using System.Collections.Generic;



	/// <summary>
	///    Summary description for PrologParser.
	/// </summary>
	public class PrologParser
	{
		PrologTokenizer m_tokenizer;
		public delegate void handleFunctor(Structure list);
		public event handleFunctor warningSingletons;

        Term _lastTerm;

       // protected Dictionary<string, Op> m_Ops;

		public PrologParser(PrologTokenizer tokenizer)
		{
			//
			// TODO: Add Constructor Logic here
			//
			m_tokenizer = tokenizer;
			//m_Ops = Op.m_Ops;
		}


		
		public Clause ReadClause()
		{
			Term t;
			Clause c=null;
			t = getTerm0(999);
			if(t != null)
			{
				if(t is Structure)
				{
                    if (((Structure)t).Name == ":-")
                    {
                        return getDirective();
                    }
					c = new Clause();
					c.Head = (Structure)t;
				}
				else
				{
					//ERROR
					return null;
				}
			}
			else return c;
			PrologToken tok= m_tokenizer.CurrentToken();
			if(tok.Type == TokenType.STOP )
			{
					m_tokenizer.NextToken();
			}
			else if(tok.Type == TokenType.NAME)
			{
				if(tok.StringValue.Equals(":-"))
				{
					m_tokenizer.NextToken();
					getClauseBody(c);
                    tok = m_tokenizer.CurrentToken();
                    if (tok.Type == TokenType.STOP)
                    {
                        m_tokenizer.NextToken();
                    }
                    else
                    {
                        error("'.' expected");
                    }
				}
				else
				{
					//ERROR
				}
			}
			checkForSingleton(c);
			return c;
		}

        private Clause getDirective()
        {
            Clause c = new Clause();
            getClauseBody(c);
            PrologToken tok = m_tokenizer.CurrentToken();
            if (tok.Type == TokenType.STOP)
            {
                m_tokenizer.NextToken();
            }
            else
            {
                error("'.' expected");
            }
            return c;
        }

	
		public Clause getClause2()
		{
			Term t;
			Clause c=null;
			t = getTerm(1200);
			if(t != null)
			{
				if(t is Structure)
				{
					Structure f = (Structure)t;
					c=new Clause();

					if(f.Name == ":-")
					{
						if(f[0] is Structure)
						{
							c.Head = (Structure)f[0];
							if(f.Arity==2)
							{
								if(f[1] is Structure)
								{
                                    try
                                    {
                                        c.AppendSubGoals((Structure)f[1]);
                                    }
                                    catch (MalformedClauseException)
                                    {
                                        error("Malformed Clause");
                                    }
									//appendSubGoals(c,(Structure)f[1]);
								}
								else //right hand is not a functor o a continuation.
								{
									error("Right hand side malformed");
								}
								
							}
							else//arity != 2
							{
								error("clause malformed (arity != 2)");
							}
						}
					}
					else //a fact
					{
						c.Head = f;
					}
				}
				else //not a functor
				{
					error("clause must be a functor");
				}
			}
			if(c != null)
			{

				checkForSingleton(c);
				PrologToken tok= m_tokenizer.CurrentToken();
				if(tok.Type == TokenType.STOP )
				{
					m_tokenizer.NextToken();
				}
				else
				{
					error(". expected");
				}
			}
			return c;
		}



		void checkForSingleton(Clause clause)
		{
			if(warningSingletons != null)
			{

				Variables vars = new Variables();
				Structure head = (Structure)clause.Head.Copy(vars);
                clause.Body.Copy(vars);

                PrologList list = vars.getSingletons();
				if(!list.isEmpty())
				{
					warningSingletons(list);
				}
			}
		}

		void getClauseBody(Clause c)
		{
			Term t=null;
            //11/06/2005 da 999 a 1100 per abilitare ';'
			t = getTerm(1100);
			if(t != null)
			{
				if(t.GetType() == typeof(Structure))
				{
					//c.Body.Add((Structure)t);
                    c.AppendSubGoals((Structure)t);
				}
				else
				{
                    c.AppendSubGoals(new Structure("call", t));
				}
			}

		}


		public Term getTerm(int prec)
		{
            StructStack opStack;
			Stack termStack;
            opStack = new StructStack();
			termStack = new Stack();
			bool loop = true;
			bool expectop = false;
            bool expectterm = false;
			while(loop)
			{
				Term t = getTerm0(prec);
			
				if(t!=null)
				{
					if((t.IsOp)&&(!expectterm))// it's an operator
					{
						Structure op =(Structure)t;
						if(op.Arity==0)
						{
							doOp(opStack,termStack,op);
                            expectop = false;
                            expectterm = true;
						}
						else //yet evaluated
						{
							termStack.Push(t);
                            expectop = true;
                            expectterm = false;
						}
					}
					else if(t.GetType().IsSubclassOf(typeof(Term))) //it's a generic Term
					{
						if(expectop)
						{
							error("Operator expected");
						}
						termStack.Push(t);
						expectop = true;
                        expectterm = false;
					}
					else //what else could it be???
					{
						loop = false;
					}
				}//t != null
				else
				{
					loop = false;
				}
                _lastTerm = t;
			}//while(true)
			while(opStack.Count > 0)
			{
				PopStack(opStack,termStack);
			}
			if(termStack.Count > 0)
				return (Term)termStack.Pop();
			else
				return null;
		}

        void doOp(StructStack opStack, Stack termStack, Structure op)
		{
			bool doit = true;
			while(doit)
			{
				if(opStack.Count>0)
				{
					Structure a = opStack.Peek().Op;
					Structure b = op;
                    if (Op.isUnary(b.Name) && (_lastTerm == null || (_lastTerm.IsOp && !_lastTerm.IsCompound)))
                    {
                        opStack.Push(new OpEntry(b,true));
						doit=false;
                    }
					else//binary
					{
						if(Op.prec(a.Name) > Op.prec(b.Name))
						{
							opStack.Push(new OpEntry(b,false));
							doit=false;
						}
						else if(Op.prec(a.Name) == Op.prec(b.Name))
						{
							if(Op.isLeftAssoc(a.Name))
							{
								PopStack(opStack,termStack);
							}
							else
							{
								opStack.Push(new OpEntry(b,false));
								doit=false;
							}
						}
						else
						{
							PopStack(opStack,termStack);
						}
					}

					
				}
				else
				{
                    opStack.Push(new OpEntry(op, (Op.isUnary(op.Name) && (_lastTerm == null || (_lastTerm.IsOp && !_lastTerm.IsCompound)))));
					doit=false;
				}
			}
		}

        void PopStack(StructStack opStack, Stack termStack)
		{
			if((opStack.Count <= 0))
			{
				return;
			}
            OpEntry entry = opStack.Pop();
			Structure op = entry.Op;
            if (termStack.Count == 0)
            {
                termStack.Push(op);
                return;
            }
            if (Op.isUnary(op.Name) && (!Op.isBinary(op.Name)))
			{
                    Term a = (Term)termStack.Pop();
                    op.AddArg(a);
				    termStack.Push(op);

			}
            else if (Op.isUnary(op.Name) && entry.Unary)
            {
                //TODO: is this the best way?
                    Term a = (Term)termStack.Pop();
                    if (a.IsNumber)
                    {
                        Integer i = a as Integer;
                        if (i != null)
                        {
                            i.Value = -i.Value;
                            termStack.Push(i);
                        }
                        else
                        {
                            Floating f = a as Floating;
                            f.Value = -f.Value;
                            termStack.Push(f);
                        }
                    }
                    else
                    {
                        op.AddArg(a);
                        termStack.Push(op);
                    }
            }
            else //just binary
            {
                if (termStack.Count >= 2)
                {
                    Term a;
                    Term b;
                    a = (Term)termStack.Pop();
                    b = (Term)termStack.Pop();
                    op.AddArg(b);
                    op.AddArg(a);
                }
                termStack.Push(op);

            }
		}

		public Term getTerm0(int prec)
		{
			PrologToken tok;
			Term t=null;
			tok = m_tokenizer.CurrentToken();

			switch(tok.Type)
			{
				case TokenType.LAYOUT:
					m_tokenizer.NextToken();
					t = getTerm0(prec);
					break;
				case TokenType.VAR:
					t = (Var)new Var(tok.StringValue);
					m_tokenizer.NextToken();
					break;
				case TokenType.INTEGER:
					t = (Integer) new Integer(tok.IntValue);
					m_tokenizer.NextToken();
					break;
				case TokenType.FLOAT:
					t = (Floating) new Floating(tok.FloatValue);
					m_tokenizer.NextToken();
					break;
			    case TokenType.OPEN:
                case TokenType.OPEN_CT:
						m_tokenizer.NextToken();
						t = getTerm(1200);
						if(m_tokenizer.CurrentToken().StringValue.Equals(")"))
						{
							m_tokenizer.NextToken();
						}
						else
						{
							error(") expected");
							//ERROR
						}
                    break;
                case TokenType.OPEN_LIST:
					if(tok.StringValue.Equals("["))
					{
                        m_tokenizer.PipeIsPunctuation = true;
						m_tokenizer.NextToken();
						PrologList list = PrologList.EmptyList;
						t = getList(list);
                        m_tokenizer.PipeIsPunctuation = false;
					}
                    break;
                case TokenType.COMMA:
					if(tok.StringValue.Equals(","))
					{
                        if (prec > 999)
						{
							m_tokenizer.NextToken();
							t = (Structure) new Structure(",");
						}
					}

					break;
					//string
				case TokenType.STRING:
					t =  getString(tok);
					m_tokenizer.NextToken();

					break;
				/*
				case TokenType.OP:
					OpFunctor of = new OpFunctor(tok.sval,tok.stype,tok.prec );
					t =  of;
					m_tokenizer.SkipLayout = false;
					tok = m_tokenizer.nextToken();
					m_tokenizer.SkipLayout = true;
					if((tok.type == TokenType.PUNCT)
						&&(tok.sval.Equals("(")))
					{
						m_tokenizer.nextToken();
						t = new Structure(of.Name);
						getFunctorArgs((Structure)t);
					}
					break;
				*/
				case TokenType.NAME:
					//we have a functor
					t = new Structure(tok.StringValue);
                    Structure f = t as Structure;
                    //if (!f.isOp())
                    //if(true)
                    {

                        //m_tokenizer.SkipLayout = false;
                        tok = m_tokenizer.NextToken();
                        //m_tokenizer.SkipLayout = true;
                        if ((tok.Type == TokenType.OPEN_CT)
                            && (tok.StringValue.Equals("(")))
                        {
                            m_tokenizer.NextToken();
                            getFunctorArgs((Structure)t);
                        }
                        if (tok.Type == TokenType.LAYOUT)
                        {
                            m_tokenizer.NextToken();
                        }
                    }
                    //else
                    //{
                    //    m_tokenizer.nextToken();
                    //}
					break;
				default:
					break;
			}


			return t;
		}

		private Structure getString(PrologToken tok)
		{
            PrologList list = PrologList.EmptyList;
			for(int i =0;i<tok.StringValue.Length ;i++)
			{
				Integer curchar = new Integer(tok.StringValue[i]);
                PrologList newitem = new PrologList(curchar);
				if(!list.isEmpty())
				{
					list.Append(newitem);
				}
				else
				{
					list = newitem;
				}

			}
			return list;
		}

        //TODO: Mettere in PrologList
        private PrologList getList(PrologList list)
		{
			PrologToken tok;
			tok = m_tokenizer.CurrentToken();
			if(tok.StringValue.Equals("]"))
			{
				m_tokenizer.NextToken();
				return list;
			}

			Term t =  getTerm(999);
			if(t!= null)
			{
                PrologList newitem = new PrologList( t);
                list = list.Append(newitem);
				//newitem.AddArg(list);
                //if(!list.isEmpty())
                //{
                //    list.append(newitem);
                //}
                //else
                //{
                //    list = newitem;
                //}
			}
			else
			{
				//ERROR
				return null;
			}
			tok = m_tokenizer.CurrentToken();
            //if(tok.type == TokenType.PUNCT)
            //{
            if (tok.Type == TokenType.COMMA)
				{
					m_tokenizer.NextToken();
					return getList(list);
				}
                else if (tok.Type == TokenType.PIPE)
				{
					//TODO
					//m_tokenizer.nextToken();
					//continue;
					m_tokenizer.NextToken();
					t = getTerm(999);
					//list.setTail(t);
                    //list.Tail = t;
                    PrologList tmp = list;
                    while (tmp.Tail!=null)
                    {
                        PrologList tmp2 = tmp.Tail as PrologList;
                        if (tmp2.isEmpty())
                        {
                            tmp.Tail = t;
                            break;
                        }
                        tmp = tmp2;
                    }
					//list.append(t).
					tok = m_tokenizer.CurrentToken();
					if(!(tok.Type == TokenType.CLOSE_LIST))
					{
						error(" ] expected");
						return null;
					}
					else
					{
						m_tokenizer.NextToken();
					}
				}
				else if(tok.Type == TokenType.CLOSE_LIST)
				{
					m_tokenizer.NextToken();
				}

				else
				{
					//ERROR!!!
					error(" , or | expected");
					return null;
				}
			//}
			return list;
				
		}

		void getFunctorArgs(Structure f)
		{
            //modifica 27/4/2005
			Term t =  getTerm(999);
			if(t!=null)
			{
				f.AddArg(t);
			}

			PrologToken tok = m_tokenizer.CurrentToken();
	

				if(tok.Type == TokenType.COMMA)
				{
					m_tokenizer.NextToken();
					getFunctorArgs(f);
				}
                else if (tok.Type == TokenType.CLOSE)
				{
					 m_tokenizer.NextToken();
					 return;
				}
				else 
				{
					error(") or , expected");
					return;
				}

					

		}

		private void error (string reason)
		{
				throw new PrologParserException(reason,m_tokenizer.NumLine,m_tokenizer.NumChar);
		}

		public Clause getQuery ()
		{
			Clause c = new Clause();
			getClauseBody(c);
			return c;
		}

        //private bool isOp(string name)
        //{
            
        //    if(m_Ops != null)
        //    {
        //        return m_Ops.ContainsKey(name);
        //    }
        //    else
        //    {
        //        return false;
        //    }
			
        //}

        public class StructStack : Stack<OpEntry>
        {

        }

        public class OpEntry
        {
            private Structure _str;

            public Structure Op
            {
                get { return _str; }
                set { _str = value; }
            }
            private bool _unary;

            public bool Unary
            {
                get { return _unary; }
                set { _unary = value; }
            }

            public OpEntry(Structure str, bool unary)
            {
                _str = str;
                _unary = unary;
            }
        }

	}



    public class PrologParserException : PrologException
    {
        int line, nchar;

        public PrologParserException(string _reason, int _line, int _nchar)
            : base(_reason)
		{
			line = _line;
			nchar = _nchar;
		}
        public override string ToString()
        {
            return "ERROR: " + Message + " in line " + line + " at char " + nchar + "\r\n";
        }
    }
}
