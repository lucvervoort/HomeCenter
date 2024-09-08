using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins
{
    static class ValidDomains
    {
        public static readonly Structure character_code_list = new Structure("character_code_list");
        public static readonly Structure close_option = new Structure("close_option");
        public static readonly Structure flag_value = new Structure("flag_value");
        public static readonly Structure io_mode = new Structure("io_mode");
        public static readonly Structure not_empty_list = new Structure("not_empty_list");
        public static readonly Structure not_less_than_zero = new Structure("not_less_than_zero");
        public static readonly Structure operator_priority = new Structure("operator_priority");
        public static readonly Structure operator_specifier = new Structure("operator_specifier");
        public static readonly Structure prolog_flag = new Structure("prolog_flag");
        public static readonly Structure read_option = new Structure("read_option");
        public static readonly Structure source_sink = new Structure("source_sink");
        public static readonly Structure stream = new Structure("stream");
        public static readonly Structure stream_option = new Structure("stream_option");
        public static readonly Structure stream_or_alias = new Structure("stream_or_alias");
        public static readonly Structure stream_position = new Structure("stream_position");
        public static readonly Structure stream_property = new Structure("stream_property");
        public static readonly Structure write_option = new Structure("write_option");
        public static readonly Structure type_name = new Structure("type_name");
 
    }
}
