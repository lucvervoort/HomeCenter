using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using System.Xml.Serialization;
using System.IO;


namespace Canna.Prolog.Runtime.Help
{
    class HelpManager
    {
        private static HelpManager _current=new HelpManager();
        private Dictionary<string, Canna.Prolog.Runtime.Help.Predicates> _helps = new Dictionary<string,Canna.Prolog.Runtime.Help.Predicates>();

        internal static HelpManager Current
        {
            get { return HelpManager._current; }
            set { HelpManager._current = value; }
        }

        public Canna.Prolog.Runtime.Help.PredicatesPredicate GetPredicateHelp(PredicateIndicator pi)
        {
            PredicateInfo pinfo = PredicateTable.Current.GetPredicateInfo(pi);
            if (pinfo != null)
            {
                Predicates helps = GetHelpsForAssembly(pinfo.Assembly.Location);
                foreach (PredicatesPredicate pp in helps.Predicate)
                {
                    if(pp.PredicateIndicator.Arity == pi.Arity)
                        if (pp.PredicateIndicator.Functor == pi.Name)
                        {
                            return pp;
                        }
                }
                return null;
            }
            throw new UnknownPredicateException(pi);
        }

        private Predicates GetHelpsForAssembly(string p)
        {
            if (!_helps.ContainsKey(p))
            {
                Predicates preds = LoadFromAssemblyLocation(p);
                _helps.Add(p, preds);
            }
            return _helps[p];
        }

        private Predicates LoadFromAssemblyLocation(string p)
        {
            string xmlFile = p.Replace(".dll", "-help.xml");
            xmlFile = xmlFile.Replace(".exe", "-help.xml");
            XmlSerializer ser = new XmlSerializer(typeof(Predicates));
            StreamReader xml = new StreamReader(xmlFile);
            try
            {
                Predicates predicates = ser.Deserialize(xml) as Predicates;
                return predicates;
            }
            catch (Exception)
            {

            }
            finally
            {
                xml.Close();
            }
            return null;
        }

    }
}
