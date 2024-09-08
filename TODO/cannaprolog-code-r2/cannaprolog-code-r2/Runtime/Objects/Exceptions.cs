/* *******************************************************************
 * Copyright (c) 2005 - 2008, Gabriele Cannata
 * All rights reserved. 
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Canna.Prolog.Runtime.Objects
{
    class TypeMismatchException : PrologRuntimeException
    {

        private static Term GetErrorTerm(Structure expected, Term found, object context)
        {
            return PrologRuntimeException.BuildErrorTerm(
                new Structure("type_error", expected, found),BuildContextTerm(context));
        }

        //public TypeMismatchException(Structure expected, Term found)
        //    : base(null, GetErrorTerm(expected, found, null))
        //{

        //}

        public TypeMismatchException(Structure expected, Term found, object context)
            : base(null, GetErrorTerm(expected,found,context))
        {
            
        }
    }

    class DomainException : PrologRuntimeException
    {

        private static Term GetErrorTerm(Structure valid_domain, Term found, object context)
        {
            return PrologRuntimeException.BuildErrorTerm(
                new Structure("domain_error", valid_domain, found), BuildContextTerm(context));
        }


        public DomainException(Structure valid_domain, Term found, object context)
            : base(null, GetErrorTerm(valid_domain,found,context))
        {

        }
    }

        class DotNetException : PrologRuntimeException
    {

        private static Term GetErrorTerm(Exception exception, object context)
        {
            Structure message = new Structure(exception.Message);
            GenericObjectTerm<Exception> innerexception = new GenericObjectTerm<Exception>(exception);
            return PrologRuntimeException.BuildErrorTerm(
                new Structure("dotnet_error", message, innerexception), BuildContextTerm(context));
        }


            public DotNetException(Exception exception, object context)
            : base(null, GetErrorTerm(exception, context))
        {

        }
    }

    class InstantiationException : PrologRuntimeException
    {
        private static Term GetErrorTerm(object context)
        {
            return PrologRuntimeException.BuildErrorTerm(new Structure("instantiation_error")
                , BuildContextTerm(context));
        }

        //public InstantiationException()
        //    : base(null, GetErrorTerm(null))
        //{ }

        public InstantiationException(object context)
            : base(null, GetErrorTerm(context))
        { }


        public override string ToString()
        {
            return "Arguments are not sufficiently intantiated";
        }
    }
    class RepresentationException : PrologRuntimeException
    {
        private static Term GetErrorTerm(Structure error, object context)
        {
            return PrologRuntimeException.BuildErrorTerm(new Structure("representation_error",error)
                , BuildContextTerm(context));
        }

        public RepresentationException(Structure error, object context)
            : base(null, GetErrorTerm(error, context))
        {

        }
    }

    class SyntaxErrorException : PrologRuntimeException
    {
        private static Term GetErrorTerm(string error, object context)
        {
            return PrologRuntimeException.BuildErrorTerm(new Structure("syntax_error", new Structure(error))
                , BuildContextTerm(context));
        }

        public SyntaxErrorException(string error, object context)
            :base(null,GetErrorTerm(error,context))
        {

        }
    }

    class PermissionException : PrologRuntimeException
    {
        private static Term GetErrorTerm(Structure operation, Structure permission_type, Term culprit, object context)
        {
            return PrologRuntimeException.BuildErrorTerm(new Structure("permission_error", operation,permission_type,culprit),BuildContextTerm(context));
        }

        public PermissionException(Structure operation, Structure permission_type, Term culprit, object context)
            :base(null,GetErrorTerm(operation,permission_type,culprit,context))
        {

        }
    }

    class ExistenceException : PrologRuntimeException
    {
        private static Term GetErrorTerm(Structure object_type, Term culprit, object context)
        {
            return PrologRuntimeException.BuildErrorTerm(new Structure("existence_error", object_type, culprit), BuildContextTerm(context));
        }

        public ExistenceException( Structure object_type, Term culprit, object context)
            : base(null, GetErrorTerm(object_type, culprit, context))
        {

        }
    }

    class UnknownPredicateException : PrologException
    {

        PredicateIndicator _pi;
        public UnknownPredicateException(PredicateIndicator pi):base(string.Format("Unknown predicate {0}", pi))
        {
            _pi = pi;
            
        }
    }

    class CutException : PrologException
    {
        IPredicate continuation;

        public IPredicate Continuation
        {
            get { return continuation; }
            set { continuation = value; }
        }

        public CutException(IPredicate cont)
        {
            continuation = cont;
        }
    }

    class PrologRuntimeException : PrologException
    {
        IPredicate continuation;
        Term _term;

        public Term Term
        {
            get { return _term; }
            set { _term = value; }
        }

        public IPredicate Continuation
        {
            get { return continuation; }
            set { continuation = value; }
        }

        public PrologRuntimeException(IPredicate cont, Term term):base("Unhandled exception: "+term.ToString())
        {
            _term = term;
            continuation = cont;
        }

        public static Structure BuildErrorTerm(Term error_term, Term detail)
        {
            return new Structure("error", error_term, detail);
        }

        public static Structure BuildContextTerm(object context)
        {
            Structure ret = new Structure("context");
            if (context != null)
            {
                if (context is BasePredicate)
                {
                    ret.AddArg(new Structure(((BasePredicate)context).ToString()));
                }
            }
            return ret;
        }

        public override string ToString()
        {
            return base.ToString();
        }


    }
}
