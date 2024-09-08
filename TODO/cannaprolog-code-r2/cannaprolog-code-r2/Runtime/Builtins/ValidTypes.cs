using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins
{
    static class ValidTypes
    {
        public static readonly Structure Atom = new Structure("atom");
        public static readonly Structure Atomic = new Structure("atomic");
        public static readonly Structure Byte = new Structure("byte");
        public static readonly Structure Callable = new Structure("callable");
        public static readonly Structure Character = new Structure("character");
        public static readonly Structure Evaluable = new Structure("evaluable");
        public static readonly Structure InByte = new Structure("in_byte");
        public static readonly Structure InCharacter = new Structure("in_character");
        public static readonly Structure Integer = new Structure("integer");
        public static readonly Structure List = new Structure("list");
        public static readonly Structure Number = new Structure("number");
        public static readonly Structure PredicateIndicator = new Structure("predicate_indicator");
        public static readonly Structure Variable = new Structure("variable");
        public static readonly Structure Object = new Structure("object");
    
    
    }

    static class RepresentationFlags
    {
        public static readonly Structure Character = new Structure("character");
        public static readonly Structure CharacterCode = new Structure("character_code");
        public static readonly Structure InCharacterCode = new Structure("in_character_code");
        public static readonly Structure MaxArity = new Structure("max_arity");
        public static readonly Structure MaxInteger = new Structure("max_integer");
        public static readonly Structure MinInteger = new Structure("min_integer");


    }
}
