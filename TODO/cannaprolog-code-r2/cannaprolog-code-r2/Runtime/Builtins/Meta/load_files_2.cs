using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Loading
{
    [PrologPredicate(Name = "load_files", Arity = 2)]
    public class load_files_2 : BasePredicate
    {



        public load_files_2(IPredicate continuation, IEngine engine, Term file, Term opts)
        {

        }
    }
}
