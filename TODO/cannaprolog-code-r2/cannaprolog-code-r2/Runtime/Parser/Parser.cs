using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins;

namespace Canna.Prolog.Runtime.Lexical
{
    public class Parser
    {
        Stack<ParseEntry> _stack = new Stack<ParseEntry>();

        ITokenizer _tokenizer;
		public Parser(ITokenizer tokenizer)
		{
            _tokenizer = tokenizer;
        }

        public Term ReadTerm()
        {
            CharConversionTable.Current.Enabled =
                (PrologFlagCollection.Current.GetAtom("char_conversion") == "true");
            _stack.Clear();
            Term term=null;
            do
            {
                ShiftToken();
            }
            while ((_tokenizer.CurrentToken.Type != TokenType.EOF)
            && (_tokenizer.CurrentToken.Type != TokenType.STOP));

            Reduce(1400);
            if (_tokenizer.CurrentToken.Type == TokenType.EOF)
            {
                if (_stack.Count > 1)
                {
                    Error("Unexpected EOF");
                }
            }

            if ((_tokenizer.CurrentToken.Type == TokenType.EOF
                ||
                _tokenizer.CurrentToken.Type == TokenType.STOP)
                &&
                (_stack.Count == 1)
                )
            {
                ParseEntry entry = _stack.Pop();
                term = entry.Term;
            }
            else
            {
                throw new PrologParserException("Syntax Error", _tokenizer.NumLine, _tokenizer.NumChar);
            }

            return term;
        }

        public Clause ReadClause()
        {
            Term t = ReadTerm();
            if (t == null) return null;
            Structure str = t as Structure;
            if (str == null)
            {
                Error("Malformed clause");
                return null;
            }
            Clause c = new Clause();
            if (str.Name == ":-")
            {
                if (str.Arity == 2)
                {
                    Structure head = str[0] as Structure;
                    if (head == null)
                    {
                        Error("Malformed clause");
                        return null;
                    }
                    c.Head = head;
                    Structure body = str[1] as Structure;
                    if (body == null)
                    {
                        c.AppendSubGoals(new Structure("call", str[1]));
                    }
                    else
                    {
                        c.AppendSubGoals(body);
                    }
                }
                else if (str.Arity == 1)
                {// a directive
                    Structure body = str[0] as Structure;
                    if (body == null)
                    {
                        Error("Malformed clause");
                        return null;
                    }
                    c.AppendSubGoals(body);
                }
                else
                {
                    Error("Malformed clause");
                    return null;
                }

            }
            else if(str.Name == "end_of_file"){
                return null;
            }
            else//a fact
            {
                c.Head = str;
            }
            return c;
        }

        #region Private Methods

        private void Reduce(int pri)
        {
            reduce:
            if(_stack.Count>2)
            {
                ParseEntry se1 = _stack.Pop();
                ParseEntry se2 = _stack.Pop();
                ParseEntry se3 = _stack.Pop();
                if (Op.IsNotAssoc(se2.Specifier))//that is, xfx
                {
                    if (
                        (se2.Priority <= pri) &&
                        (se3.IsTerm()) && (se3.Priority < se2.Priority) &&
                        (se1.IsTerm()) && (se1.Priority < se2.Priority)
                        )
                    {
                        Structure str = se2.Term as Structure;
                        str.AddArg(se3.Term);
                        str.AddArg(se1.Term);
                        se2.Specifier = Specifier.lterm;
                        se2.TokenType = TokenType.TERM;
                        _stack.Push(se2);
                        goto reduce;
                    }

                }
                else if (Op.IsLeftAssoc(se2.Specifier))
                {
                    if (
                        se2.Priority <= pri &&
                        (se3.IsTerm() && se3.Priority < se2.Priority ||
                        se2.IsLTerm() && se3.Priority == se2.Priority
                       // || !se3.Term.IsCompound
                        ) &&
                        se1.IsTerm() && se1.Priority < se2.Priority
                        )
                    {
                        Structure str = se2.Term as Structure;
                        str.AddArg(se3.Term);
                        str.AddArg(se1.Term);
                        se2.Specifier = Specifier.lterm;
                        se2.TokenType = TokenType.TERM;
                        _stack.Push(se2);
                        goto reduce;
                    }
                }
                else if(Op.IsRightAssoc(se2.Specifier))
                {
                    if(
                        se2.Priority < pri &&
                        se3.IsTerm() && se3.Priority < se2.Priority &&
                        se1.IsTerm() && se1.Priority <= se2.Priority
                        )
                    {
                        //TODO: gestire COMMA?
                        Structure str = se2.Term as Structure;
                        str.AddArg(se3.Term);
                        str.AddArg(se1.Term);
                        se2.Specifier = Specifier.lterm;
                        se2.TokenType = TokenType.TERM;
                        _stack.Push(se2);
                        goto reduce;
                    }
                }
                _stack.Push(se3);
                _stack.Push(se2);
                _stack.Push(se1);


            }// (_termStack.Count > 2)

            if (_stack.Count > 1)
            {
                ParseEntry se1 = _stack.Pop();
                ParseEntry se2 = _stack.Pop();
                if (se1.IsYF())
                {
                    if (se2.IsTerm() && se2.Priority < se1.Priority ||
                        se2.IsLTerm() && se2.Priority == se1.Priority)
                    {
                        Structure str = se1.Term as Structure;
                        str.AddArg(se2.Term);
                        se1.Specifier = Specifier.lterm;
                        se1.TokenType = TokenType.TERM;
                        _stack.Push(se1);
                        goto reduce;
                    }
                }
                else if (se1.IsXF())
                {
                    if (se2.IsTerm() &&
                        se2.Priority < se1.Priority)
                    {
                        Structure str = se1.Term as Structure;
                        str.AddArg(se2.Term);
                        se1.Specifier = Specifier.lterm;
                        se1.TokenType = TokenType.TERM;
                        _stack.Push(se1);
                        goto reduce;
                    }

                }
                else if (se2.IsFY())
                {
                    if (se2.Priority < pri &&
                        se1.IsTerm() && se1.Priority <= se2.Priority)
                    {
                        Structure str = se2.Term as Structure;
                        str.AddArg(se1.Term);
                        se2.Specifier = Specifier.lterm;
                        se2.TokenType = TokenType.TERM;
                        _stack.Push(se2);
                        goto reduce;

                    }

                }
                else if (se2.IsFX())
                {
                    if (se2.Priority <= pri &&
                        se1.IsTerm() && se1.Priority < se2.Priority)
                    {
                        Structure str = se2.Term as Structure;
                        str.AddArg(se1.Term);
                        se2.Specifier = Specifier.lterm;
                        se2.TokenType = TokenType.TERM;
                        _stack.Push(se2);
                        goto reduce;
                    }
                }
                _stack.Push(se2);
                _stack.Push(se1);

            }
        }

       
        private void ShiftToken()
        {
            PrologToken tok = _tokenizer.NextToken();
            switch (tok.Type)
            {
                case TokenType.NAME:
                    Structure str = new Structure(tok.StringValue);

                    if (Op.isOp(tok.StringValue))//it's an operator
                    {
                        Op op = Op.GetOp(tok.StringValue);
                        int inpostpri = op.InfixPriority + op.PostfixPriority;
                        //check if it's overloaded
                        if (op.PrefixPriority > 0 && inpostpri > 0)
                        {
                            /* infix and postfix do never occur together 6.3.4.2 */
                            if (_tokenizer.LookAheadChar == '(')
                            {
                                /* prefix can never be followed directly by an open char */
                                Reduce(inpostpri);
                                Shift(TokenType.NAME, inpostpri, op.Type & (Specifier.xfx | Specifier.xfy | Specifier.yfx | Specifier.yf | Specifier.xf), str);
                            }
                            else
                            {
                                if (_stack.Count > 0)
                                {
                                    Reduce(inpostpri); // must be done before testing the stack 
                                    if (_stack.Peek().IsTerm())
                                    {
                                        // can be either infix of postfix
                                        Shift(TokenType.NAME, inpostpri,
                                            op.Type & (Specifier.xfx | Specifier.xfy | Specifier.yfx | Specifier.xf | Specifier.yf), str);
                                    }
                                    else
                                    {
                                        // in the beginning of an expression -> must be prefix
                                        Shift(TokenType.NAME, op.PrefixPriority, op.Type & (Specifier.fx|Specifier.fy),str);
                                    }
                                }
                                else//empty stack
                                {
                                    Shift(TokenType.NAME, op.PrefixPriority, op.Type & (Specifier.fx | Specifier.fy), str);
                                }
                            }

                        }
                        else //it's not overloaded
                        {
                            int prec = op.PrefixPriority + op.PostfixPriority + op.InfixPriority;
                            Reduce(prec);
                            Shift(tok.Type, prec, op.Type, str);
                        }
                    }
                    else //not an operator
                    {
                        Shift(tok.Type, 0, Specifier.term, str);
                    }
                    break;
                case TokenType.VAR:
                    Var v = new Var(tok.StringValue);
                    Shift(tok.Type, 0, Specifier.term, v);
                    break;
                case TokenType.INTEGER:
                    Integer i = new Integer(tok.IntValue);

                    if (_stack.Count > 0)
                    {
                        ParseEntry entry = _stack.Peek();
                        if ((entry.Term != null)&&(entry.Term.IsAtom)&&entry.IsPrefix())
                        {
                            if (((Structure)entry.Term).Name == "-")
                            {
                                _stack.Pop();
                                i.Value = -i.Value;
                            }
                        }
                    }

                    Shift(tok.Type, 0, Specifier.term, i);
                    break;
                case TokenType.FLOAT:
                    Floating f = new Floating(tok.FloatValue);

                    if (_stack.Count > 0)
                    {
                        ParseEntry entry = _stack.Peek();
                        if ((entry.Term != null) && (entry.Term.IsAtom) && entry.IsPrefix())
                        {
                            if (((Structure)entry.Term).Name == "-")
                            {
                                _stack.Pop();
                                f.Value = -f.Value;
                            }
                        }
                    }

                    Shift(tok.Type, 0, Specifier.term, f);
                    break;
                case TokenType.STRING:
                    ShiftString(tok.StringValue);
                    break;
                case TokenType.OPEN:
                    Shift(TokenType.OPEN, 1300, Specifier.delimiter, null);
                    break;
                case TokenType.OPEN_CT:
                    Shift(TokenType.OPEN_CT, 1300, Specifier.delimiter, null);
                    break;

                case TokenType.CLOSE:
                    if(!ReduceStructure())
                        if (!ReduceBrackets())
                        {
                            throw new PrologParserException("Incomplete reduction", _tokenizer.NumLine, _tokenizer.NumChar);
                        }
                    break;
                case TokenType.COMMA:
                    Reduce(1000);
                    Shift(TokenType.COMMA, 1000, Specifier.xfy, new Structure(","));
                    break;
                case TokenType.OPEN_LIST:
                    Shift(TokenType.OPEN_LIST, 1300, Specifier.delimiter, null);
                    break;
                case TokenType.CLOSE_LIST:
                    if(!ReduceList())
                    {
                        throw new PrologParserException("Incomplete reduction", _tokenizer.NumLine, _tokenizer.NumChar);
                    }
                    break;
                case TokenType.PIPE:
                    Reduce(1000);
                    Shift(TokenType.PIPE, 1000, Specifier.delimiter, null);
                    break;
                case TokenType.OPEN_CURLY:
                    Shift(TokenType.OPEN_CURLY, 1300, Specifier.delimiter, null);
                    break;
                case TokenType.CLOSE_CURLY:
                    if (!ReduceCurly())
                    {
                        Error("Incomplete reduction");
                    }
                    break;
                case TokenType.STOP:
                    break;
                case TokenType.EOF:
                    Shift(TokenType.EOF, 1400, Specifier.delimiter, new Structure("end_of_file"));
                    break;
            }
        }

        private bool ReduceCurly()
        {
            if (_stack.Count == 0)
            {
                Error("Incomplete Curly Expression");
                return false;
            }
            
            if (_stack.Peek().TokenType == TokenType.OPEN_CURLY)
            {
                _stack.Pop();
                Shift(TokenType.NAME, 0, Specifier.term, new Structure("{}"));
            }
            else
            {
                Reduce(1300);
                ParseEntry exp = _stack.Pop();
                if (_stack.Peek().TokenType != TokenType.OPEN_CURLY)
                {
                    Error("Missing Opening '{'");
                    return false;
                }
                _stack.Pop();
                if (exp.Term == null)
                {
                    if (exp.TokenType == TokenType.COMMA)
                    {
                        Error("Unexpected ','");
                        return false;
                    }
                    exp.Term = SeparatorToAtom(exp.TokenType);
                }
                Shift(TokenType.TERM, 0, Specifier.term, new Structure("{}", exp.Term));
            }
            return true;
        }

        private bool ReduceList()
        {
            if (_stack.Count == 0) return false;

            if (_stack.Peek().TokenType == TokenType.OPEN_LIST)
            {//empty list
                _stack.Pop();
                Shift(TokenType.NAME, 0, Specifier.term, new PrologList());
                return true;
            }
            else
            {
                bool inlist = true;
                bool bPipeFound = false;
                int arity = 0;
                TermList args = new TermList();
                Reduce(1000);
                do
                {
                    if (_stack.Peek().IsTerm() ||
                        (_stack.Peek().IsOp() && _stack.Peek().TokenType != TokenType.COMMA))
                    {
                        args.Add(_stack.Pop().Term);
                        arity++;
                    }
                    else
                    {
                        Error("Term or operator expected");
                        return false;
                    }
                    if (_stack.Count == 0)
                    {
                        Error("'[' or ',' expected");
                        return false;
                    }
                    if (_stack.Peek().TokenType == TokenType.PIPE)
                    {
                        bPipeFound = true;

                        if (arity == 1)
                        {
                        }
                        else
                        {
                            //We support commas after |
                            Structure comma;
                            while (args.Count > 1)
                            {
                                comma = new Structure(",",args[1], args[0]);
                                args.RemoveRange(0, 2);
                                args.Insert(0, comma);
                            }
                            arity = 1;
                        }
                        _stack.Pop();
                        if (_stack.Count == 0)
                        {
                            Error("Illegal List");
                            return false;
                        }

                    }
                    else if (_stack.Peek().TokenType == TokenType.COMMA)
                    {
                        _stack.Pop();
                        if (_stack.Count == 0)
                        {
                            Error("Incomplete list");
                            return false;
                        }
                    }
                    else if (_stack.Peek().TokenType == TokenType.OPEN_LIST)
                    {
                        inlist = false;
                        _stack.Pop();
                    }
                    else
                    {
                        Error(", expected");
                        return false;
                    }

                } while (inlist);
                //Build list
                PrologList list;
                if (!bPipeFound)
                {
                    args.Insert(0,new PrologList());
                    arity++;
                }
                while (args.Count > 1)
                {
                    list = new PrologList(args[1], args[0]);
                    args.RemoveRange(0, 2);
                    args.Insert(0,list);
                }
                list = args[0] as PrologList;
                Shift(TokenType.TERM, 0, Specifier.term, list);
            }
            
            return true;
        }

        private bool ReduceBrackets()
        {
            if (_stack.Count == 0) return false;
            Reduce(1300);
            ParseEntry exp = _stack.Pop();
            if (_stack.Peek().TokenType == TokenType.OPEN
            || _stack.Peek().TokenType == TokenType.OPEN_CT)
            {
                _stack.Pop();
                _stack.Push(exp);
                //if (!exp.IsTerm())
                if(exp.Term == null)
                {
                    if (IsPunctuation(exp.TokenType)) return false;
                    exp.Term = SeparatorToAtom(exp.TokenType);
                }
                exp.Specifier = Specifier.term;
                exp.TokenType = TokenType.TERM;
                exp.Priority = 0;
            }
            else
            {
                _stack.Push(exp);
                Error("')' expected");
                return false;
            }

            return true;
        }

        private Term SeparatorToAtom(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.OPEN_LIST:
                    return new Structure("[");
                case TokenType.CLOSE_LIST:
                    return new Structure("]");
                case TokenType.PIPE:
                    return new Structure("|");
                case TokenType.OPEN_CT:
                case TokenType.OPEN:
                    return new Structure("(");
                case TokenType.CLOSE:
                    return new Structure(")");
                case TokenType.OPEN_CURLY:
                    return new Structure("{");
                case TokenType.CLOSE_CURLY:
                    return new Structure("}");
                case TokenType.STOP:
                    return new Structure(".");
                case TokenType.COMMA:
                    return new Structure(",");
            }
            return null;
        }

        private bool ReduceStructure()
        {
            if (_stack.Count == 0)
            {
                return false;
            }
            Reduce(1000);

            bool bInTerm = true;
            ParseEntry se;
            ParseEntry[] entries = _stack.ToArray();
            TermList args = new TermList();
            int i = 0;
            int arity = 0;
            while (bInTerm)
            {
                se = entries[i];
                if (se.IsTerm() && se.Priority < 1000
                    || (se.IsOp() && (!se.Term.IsAtom || ((Structure)se.Term).Name != ",")))
                {
                    i++;
                    arity++;
                    args.Add(se.Term);
                }
                else
                    return false;
                if (i==entries.Length)
                    return false;
                se = entries[i];
                if (se.TokenType == TokenType.COMMA)
                {
                    i++;
                    if (i == entries.Length)
                        return false;
                }
                else if (se.TokenType == TokenType.OPEN_CT)
                {
                    bInTerm = false;
                }
                else
                {
                    return false;
                }

            }
            i++;
            if (i == entries.Length)
                return false;
            se = entries[i];
            if (se.TokenType != TokenType.NAME)
            {
                return false;
            }
            if (se.IsInfix() && entries.Length>(i+1))
            {
                if(!(IsPunctuation(se.TokenType)||se.IsOp()))
                {
                    return false;
                }
            }
            //Build the Term
            Structure str = se.Term as Structure;
            while (arity>0)
            {
                _stack.Pop();
                str.AddArg(args[arity-1]);
                _stack.Pop();
                arity--;
            }
            se.Priority = 0;
            se.TokenType = TokenType.TERM;
            se.Specifier = Specifier.term;
            return true;
        }

        private bool IsPunctuation(TokenType type)
        {
            return type == TokenType.OPEN ||
                    type == TokenType.OPEN_CT ||
                    type == TokenType.OPEN_CURLY ||
                    type == TokenType.OPEN_LIST ||
                    type == TokenType.PIPE ||
                    type == TokenType.CLOSE ||
                    type == TokenType.CLOSE_CURLY ||
                    type == TokenType.CLOSE_LIST ||
                    type == TokenType.COMMA;
        }

        private void ShiftString(string str)
        {
            PrologList list = PrologList.EmptyList;
            for (int i = 0; i < str.Length; i++)
            {
                Integer curchar = new Integer(str[i]);
                PrologList newitem = new PrologList(curchar);
                if (!list.isEmpty())
                {
                    list.Append(newitem);
                }
                else
                {
                    list = newitem;
                }
            }
            Shift(TokenType.TERM, 0, Specifier.term, list);
        }

        private void Shift(TokenType tok, int pri, Specifier spec, Term term)
        {
            ParseEntry entry = new ParseEntry(tok, pri, spec,term);
            _stack.Push(entry);
        }

        private void Error(string reason)
        {
            throw new PrologParserException(reason, _tokenizer.NumLine, _tokenizer.NumChar);
        }

        #endregion
    }

 

    class ParseEntry
    {
        TokenType _tokenType;

        public TokenType TokenType
        {
            get { return _tokenType; }
            set { _tokenType = value; }
        }
        int _priority;

        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
        Term _term;

        public Term Term
        {
            get { return _term; }
            set { _term = value; }
        }
        Specifier _spec;

        public Specifier Specifier
        {
            get { return _spec; }
            set { _spec = value; }
        }

        public ParseEntry(TokenType tok, int pri, Specifier spec, Term term)
        {
            _tokenType = tok;
            _priority = pri;
            _term = term;
            _spec = spec;
        }

        public bool IsOp()
        {
            return Op.IsPostfix(_spec) || Op.IsPrefix(_spec) || Op.IsInfix(_spec);
        }

        public bool IsTerm()
        {
            return 0 != (_spec & Specifier.term);
        }

        public bool IsLTerm()
        {
            return 0 != (_spec & Specifier.lterm);
        }


        public bool IsPrefix()
        {
            return Op.IsPrefix(_spec);
        }

        public bool IsPostfix()
        {
            return Op.IsPostfix(_spec);
        }

        public bool IsInfix()
        {

            return Op.IsInfix(_spec);
        }

        public bool IsYF()
        {
            return 0 != (_spec & Specifier.yf);
        }

        public bool IsXF()
        {
            return 0 != (_spec & Specifier.xf);
        }

        public bool IsFY()
        {
            return 0 != (_spec & Specifier.fy);
        }

        public bool IsFX()
        {
            return 0 != (_spec & Specifier.fx);
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
