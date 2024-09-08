using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.DotNet
{
    [PrologPredicate(Name = "load_assembly", Arity = 1)]
    public class load_assembly_1 : BasePredicate
    {
        Term _a, _ia;

        public load_assembly_1(IPredicate continuation, IEngine engine,  Term assembly)
            : base(continuation, engine)
        {
            _ia = assembly;
        }

        public override PredicateResult Call()
        {
            _a = _ia.Dereference();
            if (!_a.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _a, this);
            }
            string assembly = (string)_a.ObjectValue;
            try
            {
                System.Reflection.Assembly.LoadWithPartialName(assembly);
            }
            catch (Exception dotnetexception)
            {
                throw new DotNetException(dotnetexception, this);
            }
            return base.Call();
        }
    }
}
