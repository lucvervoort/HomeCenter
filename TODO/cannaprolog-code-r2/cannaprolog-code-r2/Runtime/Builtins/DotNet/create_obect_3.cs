using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using System.Collections;
using System.Reflection;

namespace Canna.Prolog.Runtime.Builtins.DotNet
{
    [PrologPredicate(Name = "create_object", Arity = 3)]
    public class create_object_3 : BasePredicate
    {
        Term _ob, _iob, _type, _itype, _args, _iargs;

        public create_object_3(IPredicate continuation, IEngine engine, Term ob, Term type, Term args)
            : base(continuation, engine)
        {
            _iob = ob;
            _itype = type;
            _iargs = args;
        }

        public override PredicateResult Call()
        {
            
            _ob = _iob.Dereference();
            _type = _itype.Dereference();
            _args = _iargs.Dereference();
            ErrorCheck();
            try
            {
                CreateObject();
            }
            catch(Exception e)
            {
                throw new DotNetException(e, this);
            }
            return base.Call();
        }

        private void ErrorCheck()
        {
            if (_ob.IsBound)
            {
                throw new TypeMismatchException(ValidTypes.Variable, _ob, this);
            }
            if (!_type.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _type, this);
            }
            if (!_args.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _args, this);
            }

        }

        private void CreateObject()
        {
            Structure type = _type as Structure;
            string typename = type.Name;
            
            ArrayList list = _args.ObjectValue as ArrayList;
            object[] args = list.ToArray();
            Type t = FXIntegration.GetTypeFromLoadedAssemblies(typename);
            if (t == null)
            {
                throw new DomainException(ValidDomains.type_name, _type, this);
            }
            ObjectTerm ot = FXIntegration.CreateObject(t,args);
            _ob.Unify(ot, Engine.BoundedVariables, false);
        }



    }


}
