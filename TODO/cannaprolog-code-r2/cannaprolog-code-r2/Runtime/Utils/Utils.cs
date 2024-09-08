using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Utils
{
    class Conversion
    {
        public static bool TermToBool(Term t)
        {
            Structure str = t as Structure;
            if (str != null) 
            switch (str.Name)
            {
                case "true":
                    return true;
                case "false":
                    return false;
            }
            throw new PrologException("Cannot Convert Term to Boolean");
        }

        public static Structure BoolToTerm(bool b)
        {
            return b ? Structure.True : Structure.False;
        }

        public static string AtomToString(Term t)
        {
            Structure str = t as Structure;
            if (str != null) 
                return str.Name;

            return null;
        }
    }
}
