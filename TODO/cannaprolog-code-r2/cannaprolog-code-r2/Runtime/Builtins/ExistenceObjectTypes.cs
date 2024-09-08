using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins
{
    static class ObjectType
    {
        public static readonly Structure procedure = new Structure("procedure");
        public static readonly Structure source_sink = new Structure("source_sink");
        public static readonly Structure stream = new Structure("stream");
    }
}
