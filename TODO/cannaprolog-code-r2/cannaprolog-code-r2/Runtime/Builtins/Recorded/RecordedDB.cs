using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Builtins.Recorded
{
    internal class RecordedDB
    {

        private Dictionary<Term, TermList> _db = new Dictionary<Term, TermList>(new TermComparer());

        private static RecordedDB _current=new RecordedDB();
        //private Dictionary<int, RecordReference> _index = new Dictionary<int, RecordReference>();
        //private Dictionary<Term, int> _revlooup = new Dictionary<Term, int>();
        //private int _uid = 0;

        public static RecordedDB Current
        {
            get { return _current; }
            set { _current = value; }
        }

        public RecordReferenceTerm Record(Term key, Term term, bool atEnd)
        {
            TermList tl = null;
            if (!_db.ContainsKey(key))
            {
                tl = new TermList();
                _db.Add(key, tl);
            }
            else
            {
                tl = _db[key];
            }
            if (atEnd) tl.Add(term);
            else tl.Insert(0, term);

            //TODO: multithread
            //_index.Add(_uid, new RecordReference(key,term));
            //_revlooup.Add(term, _uid);
            return new RecordReferenceTerm(new RecordReference(key, term));
        }

        public void Erase(RecordReferenceTerm rterm)
        {
            RecordReference r = rterm.Value;
            

            _db[r.Key].Remove(r.Term);
            //_index.Remove(i.Value);
            //_revlooup.Remove(r.Term);
        }

        //public Integer ReverseLoopup(Term term)
        //{
        //    int i = _revlooup[term];
        //    return new Integer(i);
        //}

        public TermList GetByKey(Term key)
        {
            if (_db.ContainsKey(key))
            {
                return _db[key];
            }
            return null;
        }

        public RecordReferenceTerm Recorded(Term key, Term term, VarList varlist)
        {
            VarList var = new VarList();
            if (_db.ContainsKey(key))
            {
                TermList tl = _db[key];
                foreach (Term t in tl)
                {
                    var.Unbind();
                    var.Clear();
                    if (!t.Unify(term, var, false))
                    {
                        continue;
                    }
                    else
                    {
                        var.Unbind();
                        t.Unify(term, varlist, false);
                        //int i = _revlooup[t];
                        
                        return new RecordReferenceTerm(new RecordReference(key,t));
                    }
                }
            }
            return null;
        }

    }

    public class RecordReferenceTerm : GenericObjectTerm<RecordReference>
    {
        public RecordReferenceTerm(RecordReference r):base(r)
        {
            
        }

        public override void Write(StreamTerm output, WriteOptions options)
        {
            output.Write(Value.ToString());
        }
    }

    public class RecordReference
    {
        private Term _key;

        public Term Key
        {
            get { return _key; }
            set { _key = value; }
        }
        private Term _term;

        public Term Term
        {
            get { return _term; }
            set { _term = value; }
        }

        public RecordReference(Term key, Term term)
        {
            Key = key;
            Term = term;
        }

        public override string ToString()
        {
            return "<" + Key.ToString() + "," + Term.ToString() + ">";
        }
    }
}
