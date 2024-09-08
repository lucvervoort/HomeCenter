using System;
using System.Collections.Generic;
using System.Text;

namespace Canna.Prolog.Runtime.Lexical
{
    public class PrologToken
    {

        private TokenType _type;

        public TokenType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private StringBuilder _sval=new StringBuilder();

        public string StringValue
        {
            get { return _sval.ToString(); }
        }

        public void AppendChar(char c)
        {
            _sval.Append(c);
        }

        private int _ival;

        public int IntValue
        {
            get { return _ival; }
            set { _ival = value; }
        }

        private double _fval;

        public double FloatValue
        {
            get { return _fval; }
            set { _fval = value; }
        }
    }

    public enum TokenType
    {
        NAME,
        STRING,
        VAR,
        INTEGER,
        FLOAT,
        //PUNCT,
        STOP,
        ERROR,
        /*OP,*/
        LAYOUT,
        TERM, //for parsing
        //added on 14/08/05
        COMMA,
        OPEN,
        OPEN_CT,
        CLOSE,
        OPEN_LIST,
        CLOSE_LIST,
        OPEN_CURLY,
        CLOSE_CURLY,
        PIPE,
        EOF
    };

    public interface ITokenizer
    {
        PrologToken NextToken();
        PrologToken CurrentToken{ get; }
        int NumLine { get;}
        int NumChar { get;}
        char CurrentChar { get; }
        char LookAheadChar { get; }
    }
}
