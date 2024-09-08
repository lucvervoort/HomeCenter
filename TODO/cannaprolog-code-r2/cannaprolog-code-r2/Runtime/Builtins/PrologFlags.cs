using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins
{
    class FlagNotChangeableException : PrologException
    {

    }

    class PrologFlag
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Term value;

        public Term Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        private bool bChangable=true;

        public bool IsChangable
        {
            get { return bChangable; }
            protected set { bChangable = value; }
        }

        public PrologFlag(string name, Term value, bool changeable)
        {
            this.name = name;
            this.value = value;
            this.bChangable = changeable;
        }

        public PrologFlag(string name, Term value):this(name, value, true)
        {
            
        }
    }

    class PrologFlagCollection
    {
        private static PrologFlagCollection current = new PrologFlagCollection();

        internal static PrologFlagCollection Current
        {
            get { return PrologFlagCollection.current; }
            set { PrologFlagCollection.current = value; }
        }

        public PrologFlagCollection()
        {
            Set("bounded", Structure.True, false);
            Set("max_integer", new Integer(int.MaxValue), false);
            Set("min_integer", new Integer(int.MinValue), false);
            Set("integer_rounding_function", new Structure("down"), false);
            Set("char_conversion", Structure.True, true);
            Set("debug", Structure.False, true);
            Set("max_arity", new Structure("unbounded"), false);
            Set("unknown", new Structure("error"), true);
            Set("double_quotes", new Structure("codes"), true);
        }

        private Dictionary<string, PrologFlag> _flags = new Dictionary<string, PrologFlag>();

        public IEnumerator<PrologFlag> GetFlags()
        {
            return _flags.Values.GetEnumerator();
        }

        internal void Set(string name, Term value, bool changeable)
        {
            _flags[name] = new PrologFlag(name, value, changeable);
        }

        public void Set(string name, Term value)
        {
            if (_flags.ContainsKey(name))
            {
                Modify(name, value);
            }
            else
            {
                Add(name, value);
            }
        }

        private void Add(string name, Term value)
        {
            _flags.Add(name, new PrologFlag(name,value));
        }

        private void Modify(string name, Term value)
        {
            PrologFlag flag = _flags[name];
            if (!flag.IsChangable)
            {
                throw new FlagNotChangeableException();
            }
            flag.Value = value;
        }

        internal PrologFlag GetFlag(string name)
        {
            if (_flags.ContainsKey(name))
            {
                return _flags[name];
            }
            else
            {
                throw new PrologException("Flag NOT Found");
            }
        }

        internal string GetAtom(string name)
        {
            string ret=null;
            PrologFlag flag = _flags[name];
            if (flag != null)
            {
                Structure str = flag.Value as Structure;
                if (str != null)
                {
                    ret = str.Name;
                }
            }
            return ret;
        }
    }
}
