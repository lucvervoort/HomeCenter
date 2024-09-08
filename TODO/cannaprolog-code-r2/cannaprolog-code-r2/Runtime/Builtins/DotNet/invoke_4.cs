using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Utils;
using System.Collections;

namespace Canna.Prolog.Runtime.Builtins.DotNet
{
    [PrologPredicate(Name = "invoke", Arity = 4)]
    public class invoke_4 : BasePredicate
    {
        Term _ob, _iob, _method, _imethod, _result, _iresult, _args, _iargs;
        public invoke_4(IPredicate continuation, IEngine engine,
            Term ob, Term method, Term args, Term result)
            : base(continuation, engine)
        {
            _iob = ob;
            _imethod = method;
            _iresult = result;
            _iargs = args;
        }

        public override PredicateResult Call()
        {
            _ob = _iob.Dereference();
            _method = _imethod.Dereference();
            _args = _iargs.Dereference();
            _result = _iresult.Dereference();
            ErrorCheck();
            string methodName = Conversion.AtomToString(_method);

            try
            {
                ObjectTerm obj = _ob as ObjectTerm;
                ObjectTerm result = null;
                Type t = null;
                object o = null;
                if (obj != null)
                {
                    o = obj.ObjectValue;
                    t = o.GetType();
                }
                else //static method
                {
                    string typeName = Conversion.AtomToString(_ob);
                    t = FXIntegration.GetTypeFromLoadedAssemblies(typeName);
                    if (t == null)
                    {
                        throw new DomainException(ValidDomains.type_name, _ob, this);
                    }
                }
                ArrayList list = _args.ObjectValue as ArrayList;
                object[] args = list.ToArray();

                result = FXIntegration.Invoke(t, o, methodName, args);

                if (result != null)
                {
                    _result.UnifyWithObject(result, Engine.BoundedVariables, false);
                }

            }
            catch (Exception e)
            {
                throw new DotNetException(e, this);
            }
        

            return base.Call();
        }

        private void ErrorCheck()
        {
            if (!_ob.IsBound || !_method.IsBound)
            {
                throw new InstantiationException(this);
            }
            if (!(_ob.IsObject) && !_ob.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Object, _ob, this);
            }
            if (!_method.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _method, this);
            }
            if (_result.IsBound)
            {
                throw new TypeMismatchException(ValidTypes.Variable, _result, this);
            }
            if (!_args.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _args, this);
            }
        }
    }
}
