using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Canna.Prolog.Runtime.Objects;
using Canna.Prolog.Runtime.Builtins;
using System.Collections.Specialized;
using System.Diagnostics;
using Canna.Prolog.Runtime.Builtins.Control;

namespace Canna.Prolog.Runtime
{
    class PredicateTable
    {
        private static PredicateTable _theTable = new PredicateTable();

        private Dictionary<string, PredicateInfo> _table = new Dictionary<string, PredicateInfo>();

        private PredicateTable()
        {
                Assembly builtins = Assembly.GetAssembly(typeof(call_1));
                PrologTextInfo pti = new PrologTextInfo(builtins, "<Builtins>");
                AddAssembly(pti);
        }

        internal PredicateTable(bool s)
        {

        }

        internal static PredicateTable Current
        {
            get { return PredicateTable._theTable; }
            set { PredicateTable._theTable = value; }
        }

        public void AddAssembly(PrologTextInfo pti)
        {
            Type[] types = pti.Assembly.GetExportedTypes();
            foreach (Type type in types)
            {
                if(type.GetInterface("IPredicate")!=null)
                    if ((!type.IsNested))
                    {
                        AddType(type, pti);
                    }
            }
        }

        public PredicateInfo GetPredicateInfo(string name)
        {
            PredicateInfo pinfo = null;
            string key = name;
            if (_table.ContainsKey(key))
            {
                pinfo = _table[key];
            }
            return pinfo;
        }

        public PredicateInfo GetPredicateInfo(PredicateIndicator pi)
        {
            return GetPredicateInfo(pi.GetPredicateName());
        }

        private void AddType(Type type, PrologTextInfo pti)
        {
            
            object[] attr = type.GetCustomAttributes(typeof(PrologPredicateAttribute), false);
            if (attr.Length > 0)
            {
                PredicateInfo predInfo = new PredicateInfo();
                predInfo.Assembly = pti.Assembly;
                predInfo.SourceFile = pti.Filename;
                predInfo.Type = type;
                PrologPredicateAttribute ppa = (PrologPredicateAttribute)attr[0];
                predInfo.PredicateIndicator = new PredicateIndicator(ppa.Name, ppa.Arity);
                predInfo.IsPublic = ppa.IsPublic;
                string predName = GetPredicateName(type);
                try
                {
                    _table.Add(predName, predInfo);
                }
                catch (Exception )
                {
                    Debug.WriteLine("Error inserting " + predName);
                }
            }

        }

        public static string GetPredicateName(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(PrologPredicateAttribute), false);
            if (attributes.Length > 0)
            {
                PrologPredicateAttribute ppa = (PrologPredicateAttribute)attributes[0];
                return new PredicateIndicator( ppa.Name, ppa.Arity).GetPredicateName();
            }
            else
            {
                return type.Name;
            }
        }

        internal void AddPredicate(PredicateIndicator pi, DynamicPredicate dp)
        {
            PredicateInfo predInfo = new PredicateInfo();
            predInfo.IsDynamic = true;
            predInfo.IsPublic = true;
            //TODO: find a better way
            predInfo.PredicateIndicator = pi;
            predInfo.DynamicPredicate = dp;
            _table.Add(pi.GetPredicateName(), predInfo);
        }

        internal Type GetPredicateType(PredicateIndicator pi)
        {
            Type t = null;
            string pname = pi.GetPredicateName();
            if (_table.ContainsKey(pname))
            {
                PredicateInfo predInfo = _table[pname];
                    t = predInfo.Type;
            }
            return t;
        }

        internal IPredicate GetPredicate(PredicateIndicator pi, IPredicate continuation, IEngine engine, params Term[] args)
        {
            PredicateInfo info = GetPredicateInfo(pi);
            if (info == null)
            {
                throw new UnknownPredicateException(pi);
            }
            if (!info.IsDynamic && info.Type != null)
                return CreatePredicateFromType(info.Type, continuation, engine, args);

            if (info.IsDynamic)
            {
                DynamicPredicate dp = info.DynamicPredicate;
                if (dp == null)
                {
                    throw new UnknownPredicateException(pi);
                }
                return dp.GetNew(continuation, engine, args);
            }
            return null;
        }

        private IPredicate CreatePredicateFromType(Type type, IPredicate continuation, IEngine engine, params Term[] arguments)
        {
            object[] args = new object[arguments.Length + 2];
            args[0] = continuation;
            args[1] = engine;
            for (int i = 0, j = 2; i < arguments.Length; ++i, ++j)
            {
                args[j] = arguments[i];
            }
            IPredicate pred=null;
               pred  = type.InvokeMember("", BindingFlags.CreateInstance, null, null, args) as IPredicate;
            return pred;

        }

        internal void RemoveAssembly(PrologTextInfo prologTextInfo)
        {
            StringCollection toRemove = new StringCollection();
            foreach (KeyValuePair<string, PredicateInfo> entry in _table)
            {
                if (entry.Value.SourceFile == prologTextInfo.Filename)
                {
                    toRemove.Add(entry.Key);
                }
            }
            foreach (string key in toRemove)
            {
                _table.Remove(key);
            }
        }

        internal void RemovePredicate(string pred)
        {
            if (_table.ContainsKey(pred))
                _table.Remove(pred);
        }

        public IEnumerator<PredicateIndicator> GetPredicateIndicators()
        {
            foreach (PredicateInfo pinfo in _table.Values)
            {
                yield return pinfo.PredicateIndicator;
            }
        }

        //Dynamic predicate stuff
        public void AbolishPredicate(PredicateIndicator pi)
        {
            string key = pi.GetPredicateName();
            RemovePredicate(key);
        }

        public void AssertPredicate(PredicateIndicator pi, DynamicPredicate dp)
        {
            string key = pi.GetPredicateName();
            AddPredicate(pi, dp);
        }

        internal bool Contains(PredicateIndicator pi)
        {
            return _table.ContainsKey(pi.GetPredicateName());
        }

        internal DynamicPredicate GetDynamicPredicate(PredicateIndicator pi)
        {
            PredicateInfo info = GetPredicateInfo(pi);
            if (info != null)
            {
                return info.DynamicPredicate;
            }
            return null;
        }

        internal void SetDynamic(PredicateIndicator pi)
        {
            PredicateInfo info = GetPredicateInfo(pi);
            if (info == null)
            {
                info = new PredicateInfo();
            }
            info.PredicateIndicator = pi;
            info.IsDynamic = true;
            info.IsPublic = true;
            info.DynamicPredicate = new DynamicPredicate();
            _table[pi.GetPredicateName()] = info;
        }

        internal void SetMultifile(PredicateIndicator pi)
        {
            PredicateInfo info = GetPredicateInfo(pi);
            if (info == null)
            {
                info = new PredicateInfo();
            }
            info.PredicateIndicator = pi;
            info.IsMultiFile = true;
            _table[pi.GetPredicateName()] = info;
        }

        internal void SetDiscontiguous(PredicateIndicator pi)
        {
            PredicateInfo info = GetPredicateInfo(pi);
            if (info == null)
            {
                info = new PredicateInfo();
            }
            info.PredicateIndicator = pi;
            info.IsDiscontiguous = true;
            _table[pi.GetPredicateName()] = info;
        }
    }
}
