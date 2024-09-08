using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins
{
    public static class Operations
    {
        public static readonly Structure access = new Structure("access");
        public static readonly Structure create = new Structure("create");
        public static readonly Structure input = new Structure("input");
        public static readonly Structure modify = new Structure("modify");
        public static readonly Structure open = new Structure("open");
        public static readonly Structure output = new Structure("output");
        public static readonly Structure reposition = new Structure("reposition");

    }

    public static class PermissionsTypes
    {
        public static readonly Structure binary_stream = new Structure("binary_stream");
        public static readonly Structure flag = new Structure("flag");
        public static readonly Structure Operator = new Structure("operator");
        public static readonly Structure past_end_of_stream = new Structure("past_end_of_stream");
        public static readonly Structure private_procedure = new Structure("private_procedure");
        public static readonly Structure static_procedure = new Structure("static_procedure");
        public static readonly Structure source_sink = new Structure("source_sink");
        public static readonly Structure stream = new Structure("stream");
        public static readonly Structure text_stream = new Structure("text_stream");

    }
}
