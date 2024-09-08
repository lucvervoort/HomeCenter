using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using System.Reflection;

namespace Canna.Prolog.Runtime.Builtins.DotNet
{
    internal class FXIntegration
    {
        public static ObjectTerm CreateObject(Type t, object[] args)
        {
            ObjectTerm ct = null;
            object o = Activator.CreateInstance(t, args);
            if (o != null)
            {
                Type gt = Type.GetType("Canna.Prolog.Runtime.Objects.GenericObjectTerm`1[[" + t.AssemblyQualifiedName + "]]");
                ct = Activator.CreateInstance(gt, o) as ObjectTerm;
            }
            return ct;
        }

        public static ObjectTerm Invoke(Type t, object o, string method, object[] args)
        {
            ObjectTerm ct = null;
            BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Public;
             flags |= BindingFlags.InvokeMethod;
            flags |= BindingFlags.Static;
            if (o != null)
            {
                flags |= BindingFlags.Instance;
            }
            
            if (args == null || args.Length == 0)
            {
                flags |= BindingFlags.GetProperty;
                flags |= BindingFlags.GetField;
            }
            else
            {
                FieldInfo fi = t.GetField(method);
                if (fi != null)
                {
                    flags ^= BindingFlags.InvokeMethod;
                    flags |= BindingFlags.SetField;
                }
                else
                {
                    PropertyInfo pi = t.GetProperty(method);
                    if (pi != null)
                    {
                        flags ^= BindingFlags.InvokeMethod;
                        flags |= BindingFlags.SetProperty;
                    }
                }

            }


            object res = t.InvokeMember(method,
             flags, null,
            o, args);
            if (res != null)
            {
                Type gt = Type.GetType("Canna.Prolog.Runtime.Objects.GenericObjectTerm`1[[" + res.GetType().AssemblyQualifiedName + "]]");
                ct = Activator.CreateInstance(gt, res) as ObjectTerm;
            }
            return ct;
        }

        public static Type GetTypeFromLoadedAssemblies(string typename)
        {
            Type t = null;
            t = Type.GetType(typename, false, true);
            if (t == null)
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = a.GetType(typename, false, true);
                    if (t != null) break;
                }
            }
            return t;
        }
    }


}
