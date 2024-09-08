/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Meta
{
    [PrologPredicate(Name = "var", Arity = 1)]
    public class var_1 : BasePredicate
    {

        Term _arg1;

        public var_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            if (!_arg1.Dereference().IsBound)
            {
                return Success();
            }
            return Fail();
        }

    }


    [PrologPredicate(Name = "nonvar", Arity = 1)]
    public class nonvar_1 : BasePredicate
    {

        Term _arg1;

        public nonvar_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            if (!(_arg1.Dereference() is Var))
            {
                return Success();
            }
            return Fail();
        }

    }

    [PrologPredicate(Name = "integer", Arity = 1)]
    public class integer_1 : BasePredicate
    {

        Term _arg1;

        public integer_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            if ((_arg1.Dereference().IsInteger))
            {
                return Success();
            }
            return Fail();
        }

    }

    [PrologPredicate(Name = "float", Arity = 1)]
    public class float_1 : BasePredicate
    {

        Term _arg1;

        public float_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            if ((_arg1.Dereference() is Floating))
            {
                return Success();
            }
            return Fail();
        }

    }


    [PrologPredicate(Name = "number", Arity = 1)]
    public class number_1 : BasePredicate
    {

        Term _arg1;

        public number_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            if ((_arg1.Dereference().IsNumber))
            {
                return Success();
            }
            return Fail();
        }

    }

    [PrologPredicate(Name = "atom", Arity = 1)]
    public class atom_1 : BasePredicate
    {

        Term _arg1;

        public atom_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            if ((_arg1.Dereference().IsAtom))
            {
                return Success();
            }
            return Fail();
        }

    }

    [PrologPredicate(Name = "atomic", Arity = 1)]
    public class atomic_1 : BasePredicate
    {

        Term _arg1;

        public atomic_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            Term t = _arg1.Dereference();
            if ((t.IsAtom)||(t.IsNumber))
            {
                return Success();
            }
            return Fail();
        }

    }

    [PrologPredicate(Name = "compound", Arity = 1)]
    public class compound_1 : BasePredicate
    {

        Term _arg1;

        public compound_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            Term t = _arg1.Dereference();
            if (t.IsCompound)
            {
                return Success();
            }
            return Fail();
        }

    }

    [PrologPredicate(Name = "callable", Arity = 1)]
    public class callable_1 : BasePredicate
    {

        Term _arg1;

        public callable_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            if (_arg1.Dereference() is Structure)
            {
                return Success();
            }
            return Fail();
        }

    }

    [PrologPredicate(Name = "ground", Arity = 1)]
    public class ground_1 : BasePredicate
    {

        Term _arg1;

        public ground_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation,engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            if (_arg1.Dereference().IsGround)
            {
                return Success();
            }
            return Fail();
        }

    }

    [PrologPredicate(Name = "object", Arity = 1)]
    public class object_1 : BasePredicate
    {

        Term _arg1;

        public object_1(IPredicate continuation, IEngine engine, Term arg1)
            : base(continuation, engine)
        {
            _arg1 = arg1;
        }

        public override PredicateResult Call()
        {
            if (_arg1.Dereference().IsObject)
            {
                return Success();
            }
            return Fail();
        }

    }
}

