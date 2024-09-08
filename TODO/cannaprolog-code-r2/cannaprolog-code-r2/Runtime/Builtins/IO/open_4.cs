using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;
using System.IO;
using Canna.Prolog.Runtime.Utils;
using System.Diagnostics;

namespace Canna.Prolog.Runtime.Builtins.IO
{
    [PrologPredicate(Name = "open", Arity = 4)]
    public class open_4 : BindingPredicate
    {
        Term _srcdes, _isrcdes;
        Term _iomode, _iiomode;
        Term _stream, _istream;
        Term _opts, _iopts;

        public open_4(IPredicate continuation, IEngine engine, Term srcdes, Term iomode, Term stream, Term opts)
            : base(continuation,engine)
        {
            _isrcdes = srcdes;
            _iiomode = iomode;
            _istream = stream;
            _iopts = opts;
        }

        public override PredicateResult Call()
        {
            _srcdes = _isrcdes.Dereference();
            _iomode = _iiomode.Dereference();
            _stream = _istream.Dereference();
            _opts = _iopts.Dereference();
            ErrorCheck();
            StreamTerm st = GetStreamTerm();
            _stream.Unify(st, Engine.BoundedVariables, false);
            return Success();
        }

        private StreamTerm GetStreamTerm()
        {
            string file = Utils.Conversion.AtomToString(_srcdes);
            StreamProperties opts = StreamProperties.Create(file, _iomode, (PrologList)_opts);

            StreamTerm st = null;
            bool append = (IoMode.append == opts.Mode);
            try
            {
                if (opts.Type == StreamType.text)
                {
                    if (IoMode.read == opts.Mode)
                        st = new StreamReaderTerm(file, opts);
                    else
                        st = new StreamWriterTerm(file, opts, append);
                }
                else
                {
                    if (IoMode.read == opts.Mode)
                        st = new BinaryReaderTerm(file, opts);
                    else
                        st = new BinaryWriterTerm(file, opts, append);
                }
            }
            catch (FileNotFoundException)
            {
                throw new ExistenceException(ObjectType.source_sink, _srcdes, this);
            }
            catch (IOException)
            {
                throw new PermissionException(Operations.open, PermissionsTypes.source_sink, _srcdes, this);
            }
            catch (Exception exep)
            {
                Debug.WriteLine(exep.ToString());
                throw;
            }
            return st;
        }


       





    


        private void ErrorCheck()
        {
            if (!_srcdes.IsBound || !_iomode.IsBound || !_opts.IsGround)
            {
                throw new InstantiationException(this);
            }
            if (!_iomode.IsAtom)
            {
                throw new TypeMismatchException(ValidTypes.Atom, _iomode, this);
            }
            if (!_opts.IsList)
            {
                throw new TypeMismatchException(ValidTypes.List, _opts, this);
            }
            if (_stream.IsBound)
            {
                throw new TypeMismatchException(ValidTypes.Variable, _stream, this);
            }
        }


    }
}
