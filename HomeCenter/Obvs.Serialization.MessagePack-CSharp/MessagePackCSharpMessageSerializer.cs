using System.IO;
using MessagePack;

namespace Obvs.Serialization.MessagePack
{
    public class MessagePackCSharpMessageSerializer : IMessageSerializer
    {
        private readonly /*IFormatterResolver*/MessagePackSerializerOptions _resolver;

        public MessagePackCSharpMessageSerializer()
            : this(null)
        {
        }

        public MessagePackCSharpMessageSerializer(/*IFormatterResolver*/MessagePackSerializerOptions resolver)
        {
            _resolver = resolver ?? MessagePackSerializer.DefaultOptions /*MessagePackSerializer.DefaultResolver*/;
        }

        public void Serialize(Stream destination, object message)
        {
            MessagePackSerializer./*NonGeneric.*/Serialize(message.GetType(), destination, message, _resolver);
        }
    }
}
