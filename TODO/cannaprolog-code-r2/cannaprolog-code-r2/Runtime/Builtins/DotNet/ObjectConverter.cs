using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using System.Collections;

namespace Canna.Prolog.Runtime.Builtins.DotNet
{
    class ObjectConverter
    {
        object theObject;

        public ObjectConverter(object ob)
        {
            theObject = ob;
        }

        public Term Convert()
        {

            Type type = theObject.GetType();
            if (type.IsValueType)
            {
                return ConvertValue(type);
            }
            else
            {
                return ConvertReference(type);
            }
              
        }

        private Term DefaultAction()
        {
            return new Structure(theObject.ToString());
        }

        private Term ConvertReference(Type type)
        {
            if (theObject is string)
            {
                return new Structure(theObject.ToString());
            }
            if (theObject is IEnumerable)
            {
                PrologList list = new PrologList();
                foreach (object o in (IEnumerable)theObject)
                {
                    ObjectConverter oc = new ObjectConverter(o);
                    Term t = oc.Convert();
                    list = list.Append(new PrologList(t));
                }
                return list;
            }
            return DefaultAction();
        }

        private Term ConvertValue(Type type)
        {
            if (type.IsPrimitive)
                return ConvertPrimitive(type);
            else
                return ConvertStruct(type);
        }

        private Term ConvertStruct(Type type)
        {
            return DefaultAction();
        }

        private Term ConvertPrimitive(Type type)
        {
            if (theObject is int)
            {
                return new Integer((int)theObject);
            }
            if (theObject is Double || theObject is Floating)
            {
                return new Floating((double)theObject);
            }
            if (theObject is char)
            {
                return new Structure(theObject.ToString());
            }
            return DefaultAction();
        }
    }
}
