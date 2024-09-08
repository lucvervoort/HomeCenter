using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Canna.Prolog.Runtime.Utils;
using Canna.Prolog.Runtime.Builtins;

namespace Canna.Prolog.Runtime.Objects
{
    public abstract class StreamTerm : ObjectTerm
    {
        #region Static Stuff
        private static StreamTerm currentInput;
        private static StreamTerm currentOutput;

        private static Dictionary<string, StreamTerm> _aliases = new Dictionary<string, StreamTerm>();
        private static List<StreamTerm> _streams = new List<StreamTerm>();
        private static int _openedStreams = 0;
        private int _streamUID = -1;

        protected static int IncrementOpened()
        {
            return ++_openedStreams;
        }

        private static void AddToOpenStreams(StreamTerm stream)
        {
            _streams.Add(stream);
            if (stream.Properties.Alias != null)
            {
                string alias = Utils.Conversion.AtomToString(stream.Properties.Alias);
                if (alias != null && alias.Length > 0)
                {
                    _aliases[alias] = stream;
                }
            }
        }

        private static void RemoveFromOpenStreams(StreamTerm stream)
        {
            _streams.Remove(stream);
            if (stream.Properties.Alias != null)
            {
                string alias = Utils.Conversion.AtomToString(stream.Properties.Alias);
                if (alias != null && alias.Length > 0)
                {
                    _aliases.Remove(alias);
                }
            }
        }

        public static StreamTerm GetStreamFromTerm(Term _stream)
        {
            StreamTerm stream = null;
            if (_stream.IsAtom)
            {
                stream = StreamTerm.GetByAlias((Structure)_stream);
            }
            if (stream == null)
            {
                stream = _stream as StreamTerm;
            }
            return stream;
        }

        private static StreamTerm GetByAlias(Structure alias)
        {
            string key = Utils.Conversion.AtomToString(alias);
            if (_aliases.ContainsKey(key))
                return _aliases[key];
            else
                return null;
        }

        public static IEnumerator<StreamTerm> GetAllOpenStreams()
        {
            return _streams.GetEnumerator();
        }

        private static Structure EndOFile = new Structure("end_of_file");

        #endregion

        private bool _bOpen = true;
        private AtEofStream _ateof = AtEofStream.not;
        private StreamProperties _opts;

        private Stream _stream;

        internal Stream Stream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        internal StreamProperties Properties
        {
            get { return _opts; }
            set { _opts = value; }
        }

        protected StreamTerm(StreamProperties opts)
        {
            _opts = opts;
            AddToOpenStreams(this);
            _streamUID = IncrementOpened();
        }

        protected StreamTerm(Stream stream, StreamProperties opts)
            : this(opts)
        {
            _stream = stream;
        }

        internal static StreamTerm CurrentOutput
        {
            get { return StreamTerm.currentOutput; }
            set { StreamTerm.currentOutput = value; }
        }


        static StreamTerm()
        {
            Stream stdin = Console.OpenStandardInput();
            currentInput = new StreamReaderTerm(stdin);

            Stream stdout = Console.OpenStandardOutput();
            currentOutput = new StreamWriterTerm(stdout);
        }



        internal static StreamTerm CurrentInput
        {
            get { return StreamTerm.currentInput; }
            set { StreamTerm.currentInput = value; }
        }

        public void Close(bool force)
        {
            try
            {
                this.Close();
            }
            catch (Exception e)
            {
                //TODO: wrap in a PrologRuntimeException
                if (!force)
                    throw e;
            }
            finally
            {
                _bOpen = false;
                RemoveFromOpenStreams(this);
            }
        }

        internal abstract void Close();
        internal bool IsOpen
        {
            get { return _bOpen; }
        }

        public long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                if (_stream.CanSeek)
                {
                    _stream.Seek(value, SeekOrigin.Begin);
                }
                else
                {
                    throw new PermissionException(Operations.reposition, PermissionsTypes.stream, this, null);
                }
            }
        }

        public Structure GetChar()
        {
            int code = GetCode().Value;
            if (code > 0)
                return new Structure(((char)code).ToString());
            else
            {
                return StreamTerm.EndOFile;
            }
        }

        public Integer GetCode()
        {
            Integer icode = GetCodeInternal();
            int code = icode.Value;
            if (code < 0)
            {
                _ateof = (_ateof == AtEofStream.not ? AtEofStream.at : AtEofStream.past);
            }
            return icode;
        }

        public Integer GetByte()
        {
            Integer icode = GetByteInternal();
            int code = icode.Value;
            if (code < 0)
            {
                _ateof = (_ateof == AtEofStream.not ? AtEofStream.at : AtEofStream.past);
            }
            return icode;
        }

        protected abstract Integer GetByteInternal();

        protected abstract Integer GetCodeInternal();

        public abstract Integer PeekByte();

        public Structure PeekChar()
        {
            int code = PeekCode().Value;
            if (code > 0)
                return new Structure(((char)code).ToString());
            else
                return StreamTerm.EndOFile;
        }
        public abstract Integer PeekCode();

        public void PutCode(int code)
        {
            char c = (char)code;
            PutChar(c);
        }

        public abstract void Write(string text);

        public void Write(int i)
        {
            Write(i.ToString());
        }

        public void Write(double d)
        {
            Write(d.ToString());
        }

        public void Write(char c)
        {
            PutChar(c);
        }

        public abstract void PutChar(char c);

        public abstract void PutByte(Integer b);

        public abstract void Newline();

        public virtual void Flush()
        {
            _stream.Flush();
        }

        public IEnumerator<Structure> GetProperties()
        {
            if (Properties.Filename != null)
            {
                yield return new Structure("file_name", new Structure(Properties.Filename));
            }
            yield return new Structure("mode", new Structure(Properties.Mode.ToString()));
            if (Properties.Mode == IoMode.read)
            {
                yield return new Structure("input");
            }
            else
            {
                yield return new Structure("output");
            }
            if (Properties.Alias != null)
            {
                yield return new Structure("alias", Properties.Alias);
            }
            
            if (_stream!=null && _stream.CanSeek)
            {
                yield return new Structure("position", new Integer((int)Position));
            }
            yield return new Structure("end_of_stream", new Structure(_ateof.ToString()));
            yield return new Structure("eof_action", new Structure(Properties.EofAction.ToString()));
            yield return new Structure("reposition",Utils.Conversion.BoolToTerm(Properties.Reposition));
            yield return new Structure("type", new Structure(Properties.Type.ToString()));
        }


        public override object ObjectValue
        {
            get { return _stream; }
        }

        public override string ToString()
        {

            return string.Format("$stream({0})",_streamUID);
        }
}

    enum IoMode
    {
        read,
        write,
        append
    }

    enum StreamType
    {
        text,
        binary
    }

    enum EofAction
    {
        error,
        eof_code,
        reset
    }

    enum AtEofStream
    {
        at,
        past,
        not
    }

    public class StreamProperties
    {
        StreamType _Type = StreamType.text;

        internal StreamType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        bool _Reposition = true;

        public bool Reposition
        {
            get { return _Reposition; }
            set { _Reposition = value; }
        }
        Structure _Alias = null;

        public Structure Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }
        EofAction _EofAction = EofAction.error;

        internal EofAction EofAction
        {
            get { return _EofAction; }
            set { _EofAction = value; }
        }

        IoMode mode;

        internal IoMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        private void EvalOpt(Term t)
        {
            Structure str = t as Structure;
            if (str == null || str.Arity != 1)
            {
                throw new DomainException(ValidDomains.stream_option, t, this);
            }

            try
            {
                switch (str.Name)
                {
                    case "type":
                        EvalType(str[0]);
                        return;
                    case "reposition":
                        EvalReposition(str[0]);
                        return;
                    case "alias":
                        EvalAlias(str[0]);
                        return;
                    case "eof_action":
                        EvalEofAction(str[0]);
                        return;
                }
            }
            catch (PrologException)
            {
                throw new DomainException(ValidDomains.stream_option, str, this);
            }
        }


        private void EvalAlias(Term term)
        {
            Structure str = term as Structure;
            if (str == null || !str.IsAtom) throw new PrologException();
            Alias = str;

        }

        private void EvalReposition(Term term)
        {
            Reposition = Conversion.TermToBool(term);
        }

        private void EvalType(Term term)
        {
            Structure str = term as Structure;
            if (str == null) throw new PrologException();
            switch (str.Name)
            {
                case "binary":
                    Type = StreamType.binary;
                    break;
                case "text":
                    Type = StreamType.text;
                    break;
                default:
                    throw new PrologException();
            }
        }

        private void EvalEofAction(Term term)
        {
            Structure str = term as Structure;
            if (str == null) throw new PrologException();
            switch (str.Name)
            {
                case "error":
                    this.EofAction = EofAction.error;
                    break;
                case "eof_code":
                    this.EofAction = EofAction.eof_code;
                    break;
                case "reset":
                    this.EofAction = EofAction.reset;
                    break;
                default:
                    throw new PrologException();
            }
        }

        private void EvalIoMode(Structure iomode)
        {
            if (iomode != null)
            {
                switch (iomode.Name)
                {
                    case "read":
                        Mode = IoMode.read;
                        return;
                    case "write":
                        Mode = IoMode.write;
                        return;
                    case "append":
                        Mode = IoMode.append;
                        return;
                }
            }
            throw new DomainException(ValidDomains.io_mode, iomode, this);

        }

        public static StreamProperties Create(string filename, Term iomode, PrologList opts)
        {
            StreamProperties opt = new StreamProperties();
            foreach (Term t in opts)
            {
                opt.EvalOpt(t);
            }
            opt.Filename = filename;
            opt.EvalIoMode(iomode as Structure);
            return opt;
        }


       
    }

    public class BinaryReaderTerm : StreamTerm
    {

        public BinaryReaderTerm(string file, StreamProperties opts)
            : base(opts)
        {
            Stream = new FileStream(file, FileMode.Open, FileAccess.Read);
        }

        protected BinaryReaderTerm():base(new StreamProperties())
        {
            Stream = null;
        }

        

        internal override void Close()
        {
            Stream.Close();
        }



        protected override Integer GetCodeInternal()
        {
            throw new PermissionException(Operations.input, PermissionsTypes.binary_stream, this, null);
        }

        public override Integer PeekCode()
        {
            throw new PermissionException(Operations.input, PermissionsTypes.binary_stream, this, null);
        }

        public override void PutChar(char c)
        {
            throw new PermissionException(Operations.output, PermissionsTypes.stream, this, null);
        }

        public override void Newline()
        {
            throw new PermissionException(Operations.output, PermissionsTypes.stream, this, null);
        }

        public override void Flush()
        {
            throw new PermissionException(Operations.output, PermissionsTypes.stream, this, null);
        }


        protected override Integer GetByteInternal()
        {
            return new Integer(Stream.ReadByte());
        }

        public override Integer PeekByte()
        {
            if(Stream.CanSeek)
            {
                int cur = Stream.ReadByte();
                Stream.Seek(-1L, SeekOrigin.Current);
                return new Integer(cur);
            }
            
            else
            {
                throw new PermissionException(Operations.reposition, PermissionsTypes.stream,this,null);
            }
        }

        public override void PutByte(Integer b)
        {
            throw new PermissionException(Operations.output, PermissionsTypes.stream, this, null);
        }

        public override void Write(string text)
        {
            throw new PermissionException(Operations.output, PermissionsTypes.stream, this, null);
        }
    }

    public class StreamReaderTerm : BinaryReaderTerm
    {
        TextReader _tr;

        public StreamReaderTerm(string file, StreamProperties opts)
                        :base(file,opts)

        {
            _tr = new StreamReader(Stream);
        }

        internal StreamReaderTerm(Stream st)
        {
            _tr = new StreamReader(st);
            Stream = st;
            Properties.Mode = IoMode.read;
            Properties.Reposition = false;
            Properties.Type = StreamType.text;
        }

        internal override void Close()
        {
            _tr.Close();
        }

        protected override Integer GetCodeInternal()
        {
            int code = _tr.Read();
            return  new Integer(code);
        }

        public override Integer PeekCode()
        {
            return new Integer(_tr.Peek());
        }
}

    public class BinaryWriterTerm : StreamTerm
    {

        public BinaryWriterTerm(string file, StreamProperties opts, bool append)
            : base(opts)
        {
            FileMode mode = (append ? FileMode.Append : FileMode.OpenOrCreate);

            Stream = new FileStream(file, mode, FileAccess.Write);
        }

        protected BinaryWriterTerm():base(new StreamProperties())
        {
            Stream = null;
        }

        internal override void Close()
        {
            Stream.Close();
        }

        protected override Integer GetCodeInternal()
        {
            throw new PermissionException(Operations.input, PermissionsTypes.stream, this, null);
        }

        public override Integer PeekCode()
        {
            throw new PermissionException(Operations.input, PermissionsTypes.stream, this, null);
        }



        public override void PutChar(char c)
        {
            throw new PermissionException(Operations.output, PermissionsTypes.binary_stream, this, null);
        }

        public override void Newline()
        {
            throw new PermissionException(Operations.output, PermissionsTypes.binary_stream, this, null);
        }

        protected override Integer GetByteInternal()
        {
            throw new PermissionException(Operations.input, PermissionsTypes.stream, this, null);
        }

        public override Integer PeekByte()
        {
            throw new PermissionException(Operations.input, PermissionsTypes.stream, this, null);
        }

        public override void PutByte(Integer b)
        {
            Stream.WriteByte((byte)b.Value);
        }

        public override void Write(string text)
        {
            throw new PermissionException(Operations.output, PermissionsTypes.binary_stream, this, null);
        }
    }

    public class StreamWriterTerm : BinaryWriterTerm
    {
        TextWriter _tw;
        public StreamWriterTerm(string file, StreamProperties opts, bool append)
            :base(file,opts,append)
        {
            _tw = new StreamWriter(Stream);
            
        }

        internal StreamWriterTerm(Stream st)
        {
            _tw = new StreamWriter(st);
            Stream = st;
            Properties.Mode = IoMode.write;
            Properties.Reposition = false;
            Properties.Type = StreamType.text;
        }



        internal override void Close()
        {
            _tw.Close();
        }

        public override void PutChar(char c)
        {
            _tw.Write(c);

            _tw.Flush();
        }

        public override void Write(string text)
        {
            _tw.Write(text);

            _tw.Flush();
        }

        public override void Newline()
        {
            _tw.Write(_tw.NewLine);
            _tw.Flush();
        }
}


}
