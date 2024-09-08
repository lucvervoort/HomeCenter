using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Builtins;

namespace Canna.Prolog.Runtime.Lexical
{
    class CharConversionTable
    {
        private static CharConversionTable _current = new CharConversionTable();

        private PrologFlag _char_conversion = null;
        private bool bEnabled = true;

        public bool Enabled
        {
            get { return bEnabled; }
            set { bEnabled = value; }
        }
        private Dictionary<char, char> _table=new Dictionary<char,char>();

        internal static CharConversionTable Current
        {
            get { return CharConversionTable._current; }
            set { CharConversionTable._current = value; }
        }

        public void AddPair(char inchar, char outchar)
        {
            if (_table.ContainsKey(inchar))
            {
                _table.Remove(inchar);
            }
            if (inchar != outchar)
            {
                _table.Add(inchar, outchar);
            }
        }

        public IEnumerator<KeyValuePair<char,char>> GetPairs()
        {
            return _table.GetEnumerator();
        }


        public char Convert(char inchar)
        {
            char outchar = inchar;
            if (Enabled)
            {
                if (_table.ContainsKey(inchar))
                {
                    outchar = _table[inchar];
                }
            }
            return outchar;
        }

        private PrologFlag CharConversionFlag
        {
            get
            {
                if (_char_conversion == null)
                {
                    _char_conversion = PrologFlagCollection.Current.GetFlag("char_conversion");
                }
                return _char_conversion;
            }
        }
        
    }
}
