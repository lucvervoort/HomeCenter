/* *******************************************************************
 * Copyright (c) 2005, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections;
using Canna.Prolog.Runtime.Objects;
using System.Text;
using System.Globalization;
using Canna.Prolog.Runtime.Builtins;

namespace Canna.Prolog.Runtime.Parser
{
    




    /// <summary>
    ///    Summary description for PrologTokenizer.
    /// </summary>
    public class PrologTokenizer
    {
		enum TokenizerState 
		{
            NORMAL,
			SINGLELINECOMMENT,
            MULTILINECOMMENT
		};

		//TokenizerState m_state;
		System.IO.TextReader m_in;
		PrologToken m_token;	
		int m_line;
		int m_char;
		bool m_bSkipLayout;
		bool m_bStopfound;
        int m_peek=0;
        bool m_bLayoutInserted;
		//static string punctuationchars = "%(),[]{|}";
		//removed '[' and ']'
		static string punctuationchars = "[]%(),{|}";
		static string symbolchars = "+-*/\\^<>=`~:?@#$&.;";
		static string solochars = "!";

        private bool bPipeIsPunctuation = false;

        public bool PipeIsPunctuation
        {
            get { return bPipeIsPunctuation; }
            set { bPipeIsPunctuation = value; }
        }


        public PrologTokenizer(string s)
        {
            m_in = new System.IO.StringReader(s);

			Init();
       }

        public PrologTokenizer(System.IO.TextReader txt)
        {
            m_in = txt;
            Init();
        }


		

		private void Init()
		{
			//opchars = "():-.=";
		    reset();
            read();
		}

		public void reset()
		{
			m_token = null;
			m_bSkipLayout = true;
			m_line=1;
			m_char=1;
			m_bStopfound=false;
		}

		public PrologToken CurrentToken()
		{
			if(null == m_token)
			{
				NextToken();
			}
			return m_token;
		}

		public int NumLine
		{
			get
			{
				return m_line;
			}
		}
		public int NumChar
		{
			get
			{
				return m_char;
			}
		}
		public void read()
		{
			m_peek = m_in.Read();
			m_char++;
            if (m_peek == '\n')
			{
				m_line++;
				m_char=1;
                m_peek = m_in.Read();
				m_char++;
			}
		}

        //private void ConvertChar()
        //{
        //    string flag = PrologFlagCollection.Current.GetAtom("char_conversion")
        //}

		private void error (string reason)
		{
				throw new PrologTokenizerException(reason,NumLine,NumChar);
		}

		private void appendChar()
		{
			m_token.StringValue = m_token.StringValue+CurrentChar;
			read();
		}

        private void appendChar(char c)
        {
            m_token.StringValue = m_token.StringValue + c;
            read();
        }
		public PrologToken NextToken()
		{
			//skipSpaces();
			
            m_bLayoutInserted = false;

        token:
            m_token = new PrologToken();

            int ic = CurrentChar;

            if (m_bStopfound)
			{
				m_token.Type = TokenType.STOP;
				m_bStopfound=false;
			}
			else if (EOF) //that is, EOF
			{
				m_token.Type = TokenType.EOF ;
			}
			else //try to understand which kind of token it is
			{
				char c = (char)ic;

                if (c == '%')
                {
                    skipLineComment();
                    return NextToken();
                }
                if ((c == '/') && this.LookAheadChar == '*')
                {
                    skipMultiLineComment();
                    return NextToken();
                }
				//Check which kind of token
				if (Char.IsDigit(c))
				{
					readNumber();
				}
                else if (isLayout(c))
                {
                    m_bLayoutInserted = true;
                    readLayout();
                    goto token;
                }
				else if (c == '"')
				{
					readString();
				}
				else if (c == '\'')
				{
					readQuotedName();
				}
                //else if (c == '+' || c == '-')
                //{
                //    if (Char.IsDigit(LookAheadChar))
                //    {
                //        readNumber();
                //    }
                //    else
                //    {
                //        readName();
                //    }
                //}
                else if (isPunctuation(c))
                {
                    readPunctuation();
                }
                else if ((c == '.')&&isLayout(LookAheadChar))
                {

                    m_token.Type = TokenType.STOP;

                    appendChar();
                }
                else if (Char.IsLower(c) || isSymbol(c))
                {
                    readName();
                }
                else if (Char.IsUpper(c) || (c == '_'))
                {
                    readVariable();
                }

                else if (isSoloChar(c))
                {
                    readSolochar();
                }
                else
                {
                    m_token.Type = TokenType.ERROR;
                    read();
                }
			} // not eof and not stop
			return m_token;
		}

        private void skipMultiLineComment()
        {
            do { 
                read(); 
            } while (!((CurrentChar == '*') && (this.LookAheadChar == '/')) && !EOF);
        }

        private void skipLineComment()
        {
            do {read();}   while ((CurrentChar != '\r')&&!EOF);
            do { read(); } while (isLayout(CurrentChar)&&!EOF);
          
        }


		public bool SkipLayout
		{
			get
			{
				return m_bSkipLayout;
			}
			set
			{
				m_bSkipLayout = value;
			}
		}

		void skipSpaces()
		{
			while(Char.IsWhiteSpace(CurrentChar))
			{
				read();
			}
		}

		void readName()
		{
			if(isSymbol(CurrentChar))
			{
				while(isSymbol(CurrentChar))
				{
					appendChar();
				}
			}
			else
			{
				while(isAlpha(CurrentChar))
				{
					appendChar();
				}
			}
			//Returns only names, op are resolved at parser level
			/*
			if(isOp(m_token.sval))
			{
				//m_token.type = TokenType.OP;
				m_token.stype = ((Op)m_Ops[m_token.sval]).Type;
				m_token.prec = ((Op)m_Ops[m_token.sval]).Precedence;
			}
			else
			*/
			{
				m_token.Type = TokenType.NAME;
				//m_token.prec = 0;
			}
			//special case for full stop.
			if(m_token.StringValue.Equals("."))
			{
				//read();
				if(isLayout(CurrentChar) || (EOF))
				{
					m_token.Type = TokenType.STOP;
				}
			}

		}

		void readVariable()
		{
			appendChar(); //we know it's a capital letter or '_'
			while(isAlpha(CurrentChar))
			{
				appendChar();
			}
			m_token.Type = TokenType.VAR;
		}

		void readLayout()
		{
			appendChar(); //we know it's a capital letter or '_'
			while(isLayout(CurrentChar))
			{
				appendChar();
			}
			m_token.Type = TokenType.LAYOUT;
		}

		void readQuotedName()
		{
			read();//skip first '
			while(CurrentChar != '\'')
			{
                if (CurrentChar == '\\')
                {
                    read();
                    switch (CurrentChar)
                    {
                        case 'b':
                            appendChar('\b');
                            break;
                        case 't':
                            appendChar('\t');
                            break;
                        case 'n':
                            appendChar('\n');
                            break;
                        case 'v':
                            appendChar('\v');
                            break;
                        case 'f':
                            appendChar('\f');
                            break;
                        case 'r':
                            appendChar('\r');
                            break;
                        case 'e':
                            appendChar('\u001B');
                            break;
                        case 'd':
                            appendChar('\x7F');
                            break;
                        case 'a':
                            appendChar('\x07');
                            break;
                        case 'x':
                            readHexChar();
                            break;
                        default:
                            appendChar();
                            break;
                    }
                }
                else
                {
                    if (!EOF)
                    {
                        appendChar();
                    }
                    else
                    {
                        error("Unexpected EOF");
                    }
                }
			}
			read();
		}

        private void readHexChar()
        {
            StringBuilder sb = new StringBuilder();
            read();
            while (Char.IsDigit(CurrentChar) || ('A' <= CurrentChar && CurrentChar <= 'F')
                || ('a' <= CurrentChar && CurrentChar <= 'f'))
            {
                sb.Append(CurrentChar);
                read();
            }
            appendChar((char)int.Parse(sb.ToString(), NumberStyles.HexNumber));
        }

		void readString()
		{
			read();
			char c;
			bool instring = true;
			while(instring)
			{
				appendChar();
				c=CurrentChar;
				if ((c!='\"')&&CurrentChar>=0)continue;
				read();
				c=CurrentChar;
				if (c=='\"')
				{
					appendChar();
				}
				else
				{
					instring = false;
				}
				
			}
			m_token.Type = TokenType.STRING;
		}


		void readNumber()
		{
			bool isfloat = false;
            bool negative = false;
			int m_decimals=-1;
            if (CurrentChar == '+')
            {
                read();
            }
            if (CurrentChar == '-')
            {
                negative = true;
                read();
            }
			while((Char.IsDigit(CurrentChar)||'.'==(CurrentChar))&&(!EOF))
			{
				if('.'==(CurrentChar))
				{
					if(isfloat)
					{
						break;
					}
					else
					{
						isfloat = true;
					}
				}
				if(isfloat)
					m_decimals++;
				appendChar();
			}
			if(m_decimals<=0) 
				isfloat = false;
			if(isfloat)
			{
				m_token.Type = TokenType.FLOAT;
				m_token.StringValue = m_token.StringValue.Remove(m_token.StringValue.IndexOf("."),1);
				m_token.FloatValue=Double.Parse(m_token.StringValue); 
				m_token.FloatValue/=System.Math.Pow(10,m_decimals);
                if (negative) m_token.FloatValue = -m_token.FloatValue;
			}
			else//Integer
			{
				m_token.Type = TokenType.INTEGER;
				if(m_token.StringValue.EndsWith("."))
				{
					m_token.StringValue = m_token.StringValue.Substring(0,m_token.StringValue.Length -1);
					m_bStopfound = true;
				}
				m_token.IntValue = Int32.Parse(m_token.StringValue);
                if (negative) m_token.IntValue = -m_token.IntValue;
			}
		}
		
		private void readPunctuation()
		{
            switch (CurrentChar)
            {
                case '[':
                    m_token.Type = TokenType.OPEN_LIST;
                    break;
                case ']':
                    m_token.Type = TokenType.CLOSE_LIST;
                    break;
                case '{':
                    m_token.Type = TokenType.OPEN_CURLY;
                    break;
                case '}':
                    m_token.Type = TokenType.CLOSE_CURLY;
                    break;
                case '|':
                    m_token.Type = TokenType.PIPE;
                    break;
                case ',':
                    m_token.Type = TokenType.COMMA;
                    break;
                case '(':
                    if (m_bLayoutInserted)
                        m_token.Type = TokenType.OPEN;
                    else
                        m_token.Type = TokenType.OPEN_CT;
                    break;
                case ')':
                    m_token.Type = TokenType.CLOSE;
                    break;
            }

			appendChar();
		}

		private void readSolochar()
		{
			appendChar();
			m_token.Type = TokenType.NAME;
		}


		private void readFullstop()
		{
			appendChar();
			m_token.Type = TokenType.STOP;
		}


		private char CurrentChar
		{
			get
			{
				return (char)m_peek;
			}
		}

        internal char LookAheadChar
        {
            get
            {
                return (char)m_in.Peek();
            }
        }
        

		bool isSymbol(char c)
		{
            //if ((c == '|') && PipeIsPunctuation)
            //{
            //    return false;
            //}

			int i = symbolchars.IndexOf(c);
			if(i>=0)
				return true;
			else
				return false;
		}
    	
		bool isPunctuation(char c)
		{
			return punctuationchars.IndexOf(c)>=0;
		}

		bool isSoloChar(char c)
		{
			return solochars.IndexOf(c)>=0;
		}

		bool isQuote(char c)
		{
			return (c == '"') || (c == '\'');
		}

		bool isAlpha(char c)
		{
			return Char.IsLetter(c) || Char.IsDigit(c) || '_' == c;
		}

		bool isLayout(char c)
		{
			return Char.IsWhiteSpace(c);
		}

		public bool EOF
		{
			get
			{
				return m_peek<0;
			}
		}
	}

    public class PrologTokenizerException : PrologException
    {
        int line, nchar;

        public PrologTokenizerException(string _reason, int _line, int _nchar)
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
