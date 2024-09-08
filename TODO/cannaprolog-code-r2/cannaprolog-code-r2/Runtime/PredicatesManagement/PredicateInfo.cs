using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime
{
    class PredicateInfo
    {
        private string sourceFile;

        public string SourceFile
        {
            get { return sourceFile; }
            set { sourceFile = value; }
        }

        private Assembly assembly=null;

        public Assembly Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }

        private Type type=null;

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        private bool bDynamic=false;

        public bool IsDynamic
        {
            get { return bDynamic; }
            set { bDynamic = value; }
        }

        private bool bMultiFile=false;

        public bool IsMultiFile
        {
            get { return bMultiFile; }
            set { bMultiFile = value; }
        }

        private bool bDiscontiguous = false;

        public bool IsDiscontiguous
        {
            get { return bDiscontiguous; }
            set { bDiscontiguous = value; }
        }

        private bool bPublic=false;

        public bool IsPublic
        {
            get { return bPublic; }
            set { bPublic = value; }
        }

        private PredicateIndicator predicateIndicator;

        public PredicateIndicator PredicateIndicator
        {
            get {
                return predicateIndicator; 
            }
            set { predicateIndicator = value; }
        }

        private DynamicPredicate _dp = null;

        public DynamicPredicate DynamicPredicate
        {
            get { return _dp; }
            set { _dp = value; }
        }
    }
}
