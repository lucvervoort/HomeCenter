using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Lexical
{
    public class Tokenizer : ITokenizer
    {
        System.IO.TextReader _in;
        int _peek;
        int _nchar;

        public int NumChar
        {
            get { return _nchar; }
            set { _nchar = value; }
        }
        int _nline;

        public int NumLine
        {
            get { return _nline; }
            set { _nline = value; }
        }
        bool _bLayoutInserted;
        int _tokenChar;

        PrologToken _token;

        public Tokenizer(string s)
        {
            _in = new System.IO.StringReader(s);

			Init();
       }

        public Tokenizer(System.IO.TextReader txt)
        {
            _in = txt;
            Init();
        }

        private void Init()
        {
            Read();
        }

        public PrologToken NextToken()
        {
            _bLayoutInserted = false;
            _token = new PrologToken();
            SkipLayout();

            if (Char.IsUpper(CurrentChar) || '_' == CurrentChar)
            {
                return Variable();
            }

            if (CurrentChar == ',')
            {
                AppendChar();
                _token.Type = TokenType.COMMA;
                return _token;
            }
            if (CurrentChar == ')')
            {
                AppendChar();
                _token.Type = TokenType.CLOSE;
                return _token;
            }

            if (CurrentChar == '(')
            {
                AppendChar();
                _token.Type = (_bLayoutInserted? TokenType.OPEN : TokenType.OPEN_CT);
                return _token;
            }
            if (CurrentChar == '.')
            {
                if (IsLayoutChar(LookAheadChar) || LookAheadChar == '%' || LookAheadChar == 0xFFFF)
                {
                    AppendChar();
                   // if (IsNewLineChar(CurrentChar)) Read();
                    _token.Type = TokenType.STOP;
                    return _token;
                }
            }

            if (IsDecimalDigit(CurrentChar))
            {
                return Number();
            }

            if (CurrentChar == ']')
            {
                AppendChar();
                _token.Type = TokenType.CLOSE_LIST;
                return _token;
            }
            if (CurrentChar == '[')
            {
                AppendChar();
                _token.Type = TokenType.OPEN_LIST;
                return _token;
            }
            if (CurrentChar == '|')
            {
                AppendChar();
                _token.Type = TokenType.PIPE;
                return _token;
            }
            if (CurrentChar == '{')
            {
                AppendChar();
                _token.Type = TokenType.OPEN_CURLY;
                return _token;
            }
            if (CurrentChar == '}')
            {
                AppendChar();
                _token.Type = TokenType.CLOSE_CURLY;
                return _token;
            }
            if (CurrentChar == '"')
            {
                if (CharCodeList())
                {
                    _token.Type = TokenType.STRING;
                    return _token;
                }
            }
            if (Name())
            {
                _token.Type = TokenType.NAME;
                return _token;
            }
            if (IsEOF)
            {
                _token.Type = TokenType.EOF;
                return _token;
            }
            Error("Syntax ERROR");
            return null;
        }

        private bool Name()
        {
            if (Char.IsLower(CurrentChar))
            {
                AppendCharWithConversion();
                while (IsAlphaNumericChar(CurrentChar))
                    AppendCharWithConversion();
                return true;
            }
            if (IsGraphicChar(CurrentChar)||CurrentChar == '\\')
            {
                AppendCharWithConversion();
                while (IsGraphicChar(CurrentChar) || CurrentChar == '\\')
                    AppendCharWithConversion();
                return true;
            }
            if (CurrentChar == '!')
            {
                AppendChar();
                return true;
            }
            if (CurrentChar == ';')
            {
                AppendChar();
                return true;
            }
            if (CurrentChar == '\'')
            {
                Read();
                while (SingleQuotedItem())
                {

                }
                if (CurrentChar == '\'')
                {
                    Read();
                    return true;
                }
                else
                {
                    Error("\"'\" expected");
                }
            }

            return false;
        }

        private bool SingleQuotedItem()
        {
            //Read();
            if (CurrentChar == '\\')
            {
                if (LookAheadChar == '\n')
                {
                    Read();
                    return true;
                }
            }
            if (SingleQuotedChar())
            {
                _token.AppendChar((char)_tokenChar);
                return true;
            }
            else
                return false;
        }

        private bool CharCodeList()
        {
            Read();
            while (DoubleQuotedItem())
            {

            }
            if (CurrentChar == '"')
            {
                Read();
                return true;
            }
            else
            {
                Error("Unexpected EOF");
                return false;
            }
        }

        private bool DoubleQuotedItem()
        {
            //Read();
            if (CurrentChar == '\\')
            {
                if (LookAheadChar == '\n')
                {
                    Read();
                    return true;
                }
            }
            if (DoubleQuotedChar())
            {
                _token.AppendChar((char)_tokenChar);
                return true;
            }
            else
                return false;
        }

        private bool DoubleQuotedChar()
        {
            if (CurrentChar == '"')
            {
                if (LookAheadChar != '"')
                {
                    return false;
                }
                else
                {
                    Read();
                    _tokenChar = CurrentChar;
                    Read();
                    return true;
                }
            }
            if (CurrentChar == '\'')
            {
                _tokenChar = CurrentChar;
                Read();
                return true;
            }
            if (CurrentChar == '`')
            {
                _tokenChar = CurrentChar;
                Read();
                return true;
            }
            if (NonQuotedChar())
            {
                CharConversion();
                return true;
            }
            return false;
        }

        private PrologToken Number()
        {
            AppendChar();
            while (IsDecimalDigit(CurrentChar))
                AppendChar();
            if (IsDecimalPoint(CurrentChar)||IsExponentChar(CurrentChar))
            {
                if(IsDecimalPoint(CurrentChar))
                {
                    if (IsDecimalDigit(LookAheadChar))
                    {
                        AppendChar();
                        while (IsDecimalDigit(CurrentChar))
                        {
                            AppendChar();
                        }
                    }
                    else
                    {
                        _token.IntValue = int.Parse(_token.StringValue);
                        _token.Type = TokenType.INTEGER;
                        return _token;
                    }

                }
                if (CurrentChar == 'e' || CurrentChar == 'E')
                {
                    if (IsSignChar(LookAheadChar) || IsDecimalDigit(LookAheadChar))
                    {
                        if (IsSignChar(LookAheadChar))
                        {
                            AppendChar();
                            AppendChar();
                            if (!IsDecimalDigit(CurrentChar))
                            {
                                Error("Invalid decimal number");
                                return null;
                            }
                        }
                        else
                        {
                            AppendChar();
                        }
                    }
                    else
                    {
                        _token.FloatValue = Double.Parse(_token.StringValue, CultureInfo.InvariantCulture);
                        _token.Type = TokenType.FLOAT;
                        return _token;
                    }
                    while (IsDecimalDigit(CurrentChar))
                    {
                        AppendChar();
                    }
                    _token.FloatValue = Double.Parse(_token.StringValue, CultureInfo.InvariantCulture);
                    _token.Type = TokenType.FLOAT;
                    return _token;
                }
                else
                {
                    _token.FloatValue = Double.Parse(_token.StringValue, CultureInfo.InvariantCulture);
                    _token.Type = TokenType.FLOAT;
                    return _token;
                }
            }
            else
            {//integer
                if (_token.StringValue[0] == '0' && _token.StringValue.Length == 1)
                {
                    if (CurrentChar == 'x')
                    {
                        if (Hex())
                        {
                            _token.Type = TokenType.INTEGER;
                            return _token;
                        }
                    }
                    if (CurrentChar == 'o')
                    {
                        if (Oct())
                        {
                            _token.Type = TokenType.INTEGER;
                            return _token;
                        }
                    }
                    if (CurrentChar == 'b')
                    {
                        if (Binary())
                        {
                            _token.Type = TokenType.INTEGER;
                            return _token;
                        }
                    }
                    if (CurrentChar == '\'')
                    {
                        Read();
                        if (SingleQuotedChar())
                        {
                            _token.Type = TokenType.INTEGER;
                            _token.IntValue = _tokenChar;
                            return _token;
                        }
                        if (CurrentChar == '\'')
                        {
                            _token.Type = TokenType.INTEGER;
                            _token.IntValue = CurrentChar;
                            Read();
                            return _token;
                        }
                    }
                    _token.IntValue = 0;
                    _token.Type = TokenType.INTEGER;
                    return _token;
                }
                else
                {//just a decimal integer
                    _token.IntValue = int.Parse(_token.StringValue);
                    _token.Type = TokenType.INTEGER;
                    return _token;
                }
            }
            //Error("Syntax error");
            //return _token;
        }

        private bool SingleQuotedChar()
        {
            if ((CurrentChar == '\'')&&(LookAheadChar == '\''))
            {
                Read();
                _tokenChar = CurrentChar;
                return true;
            }
            if (CurrentChar == '"')
            {
                Read();
                _tokenChar = CurrentChar;
                return true;
            }
            if (CurrentChar == '`')
            {
                Read();
                _tokenChar = CurrentChar;
                return true;
            }
            if (NonQuotedChar())
            {
                CharConversion();
                return true;
            }

            return false;
        }

        private void CharConversion()
        {
            _tokenChar = CharConversionTable.Current.Convert((char)_tokenChar);
        }

        private bool NonQuotedChar()
        {
            if (IsGraphicChar(CurrentChar) || IsAlphaNumericChar(CurrentChar)
                || IsSoloChar(CurrentChar) || CurrentChar == ' ' || CurrentChar == 9)
            {
                _tokenChar = CurrentChar;
                Read();
                return true;
            }
            else
            {
                if (MetaEscapeSequence())
                {
                    return true;
                }
                if (CtrlEscapeSequence())
                {
                    return true;
                }
                if (OctalEscapeSequence())
                {
                    return true;
                }
                if (HexadecimalEscapeSequance())
                {
                    return true;
                }
            }
            return false;
        }

        private bool HexadecimalEscapeSequance()
        {
            if (CurrentChar == '\\' && LookAheadChar == 'x')
            {
                Read();
                Read();
                int n = 0;
                while (IsHexadecimalDigit(CurrentChar))
                {
                    if (IsDecimalDigit(CurrentChar))
                    {
                        n = (n << 4) + CurrentChar - '0';
                    }
                    else
                    {
                        n = (n << 4) + Char.ToLower(CurrentChar) - 'a';
                    }
                    Read();
                }
                if (CurrentChar == '\\')
                {
                    Read();
                    _tokenChar = n;
                    return true;
                }
                else
                {
                    Error("Invalid hexadecimal escape sequence");
                    return false;
                }
            }
            else
                return false;
        }

        private bool OctalEscapeSequence()
        {
            if (CurrentChar == '\\' && IsOctalDigit(LookAheadChar))
            {
                Read();
                int n = 0;
                while (IsOctalDigit(CurrentChar))
                {
                    n = (n << 3) + CurrentChar - '0';
                    Read();
                }
                if (CurrentChar == '\\')
                {
                    Read();
                    _tokenChar = n;
                    return true;
                }
                else
                {
                    Error("Invalid Octal Escape Sequence");
                    return false;
                }
            }
            else
                return false;
        }

        private bool CtrlEscapeSequence()
        {
            if (CurrentChar == '\\')
            {
                if("abfnrtv".IndexOf(LookAheadChar)>=0)
                {
                    Read();
                    switch(CurrentChar)
                    {
                        case 'a':
                            _tokenChar = 7;
                            break;
                        case 'b':
                            _tokenChar = 8;
                            break;
                        case 'f':
                            _tokenChar = 12;
                            break;
                        case 'n':
                            _tokenChar = 10;
                            break;
                        case 'r':
                            _tokenChar = 13;
                            break;
                        case 't':
                            _tokenChar = 9;
                            break;
                        case 'v':
                            _tokenChar = 11;
                            break;

                    }
                    Read();
                    return true;
                }
            }
            return false;
        }

        private bool MetaEscapeSequence()
        {
            if (CurrentChar == '\\')
            {
                if (IsMetaChar(LookAheadChar))
                {
                    Read();
                    _tokenChar = CurrentChar;
                    Read();
                    return true;
                }
            }
            return false;
        }

        private bool Binary()
        {
            if (IsBinaryDigit(LookAheadChar))
            {
                AppendChar();
                while (IsBinaryDigit(CurrentChar))
                {
                    _token.IntValue = (_token.IntValue << 1) + CurrentChar - '0';
                    AppendChar();
                }
                return true;
            }
            return false;
        }

        private bool Oct()
        {
            _token.IntValue = 0;
            if(IsOctalDigit(LookAheadChar))
            {
                AppendChar();
                while (IsOctalDigit(CurrentChar))
                {
                    _token.IntValue = (_token.IntValue << 3) + CurrentChar - '0';
                    AppendChar();
                }
                return true;
            }
            return false;
        }

        private bool Hex()
        {
            _token.IntValue = 0;
            if (IsHexadecimalDigit(LookAheadChar))
            {
                AppendChar();
                while (IsHexadecimalDigit(CurrentChar))
                {
                    if (IsDecimalDigit(CurrentChar))
                    {
                        _token.IntValue = (_token.IntValue << 4) + CurrentChar - '0';
                    }
                    else
                    {
                        _token.IntValue = (_token.IntValue << 4) + Char.ToLower(CurrentChar) - 'a' + 10;
                    }
                    AppendChar();
                }
                return true;
            }
            return false;

        }

        private PrologToken Variable()
        {
            AppendChar();
            while (IsAlphaNumericChar(CurrentChar))
            {
                AppendChar();
            }
            _token.Type = TokenType.VAR;
            return _token;
        }

        private void SkipLayout()
        {
            bool moreLayout=true;
            do
            {
                if (IsLayoutChar(CurrentChar))
                {
                    Read();
                    _bLayoutInserted = true;
                }
                else if (IsEndLineCommentChar(CurrentChar))
                {
                    SingleLineComment();
                    _bLayoutInserted = true;
                }
                else if (CurrentChar == '/')
                {
                    if (MultiLineComment())
                    {
                        _bLayoutInserted = true;
                    }
                    else
                    {
                        moreLayout = false;
                    }
                }
                else
                {
                    moreLayout = false;
                }

            }
            while (moreLayout);
        }

        private bool MultiLineComment()
        {
            if ((CurrentChar == '/')&&(LookAheadChar== '*'))
            {
                Read();
                Read();
                do
                {
                    while (IsPrologChar(CurrentChar) && CurrentChar != '*')
                    {
                        Read();
                    }
                    if (IsPrologChar(CurrentChar)) Read();
                }
                while (IsPrologChar(CurrentChar) && CurrentChar != '/');
                if (IsPrologChar(CurrentChar))
                {
                    Read();
                }
                else
                {
                    Error("unexpected eof");
                }
                return true;
            }
            return false;
        }


        private void SingleLineComment()
        {
            Read();
            while (IsPrologChar(CurrentChar) && !IsNewLineChar(CurrentChar))
            {
                Read();
            }
            Read();
        }

        private void Error(string reason)
        {
            throw new PrologTokenizerException(reason, _nline, _nchar);
        }


        private char Read()
        {
            _peek = _in.Read();
            _nchar++;
            if (_peek == '\n')
            {
                _nline++;
                _nchar = 1;
                _peek = _in.Read();
                _nchar++;
            }
            return (char)_peek;
        }

        private char AppendChar()
        {
            _token.AppendChar(CurrentChar);
            return Read();
        }

        private char AppendCharWithConversion()
        {
            _token.AppendChar(CharConversionTable.Current.Convert(CurrentChar));
            return Read();
        }

        public char CurrentChar
        {
            get
            {
                return (char)_peek;
            }
        }

        public char LookAheadChar
        {
            get
            {
                return (char)_in.Peek();
            }
        }

        private bool IsEOF
        {
            get
            {
                return _peek < 0;
            }
        }

        private bool IsLayoutChar(char c)
        {
            return Char.IsWhiteSpace(c);
        }

        private bool IsEndLineCommentChar(char c)
        {
            return c == '%';
        }

        private bool IsGraphicChar(char c)
        {
            return @"#$&*+-./:<=>?@^~".IndexOf(c) >= 0;
        }

        private bool IsAlphaNumericChar(char c)
        {
            return Char.IsLetterOrDigit(c)||c=='_';
        }

        private bool IsSoloChar(char c)
        {
            return @"!(),;[]{}|%".IndexOf(c) >= 0;
        }

        private bool IsMetaChar(char c)
        {
            return @"\'""`".IndexOf(c) >= 0;
        }

        private bool IsPrologChar(char c)
        {
            return IsAlphaNumericChar(c) || IsGraphicChar(c) || IsSoloChar(c)
                        || IsLayoutChar(c) || IsMetaChar(c);
        }

        private bool IsNewLineChar(char c)
        {
            return c == '\r';
            //return Environment.NewLine.IndexOf(c) >= 0;
        }

        private bool IsDecimalDigit(char c)
        {
            return Char.IsDigit(c);
        }

        private bool IsDecimalPoint(char c)
        {
            return c == '.';
        }

        private bool IsSignChar(char c)
        {
            return c == '-' || c == '+';
        }

        private bool IsExponentChar(char c)
        {
            return c == 'e' || c == 'E';
        }

        private bool IsHexadecimalDigit(char c)
        {
            return Char.IsDigit(c) || "abcdef".IndexOf(Char.ToLower(c)) >= 0;
        }
        private bool IsOctalDigit(char c)
        {
            return c >= '0' && c <= '7';
        }

        private bool IsBinaryDigit(char c)
        {
            return c == '0' || c == '1';
        }


        #region ITokenizer Members


        public PrologToken CurrentToken
        {
            get
            {
                return _token;
            }
        }



        #endregion
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
