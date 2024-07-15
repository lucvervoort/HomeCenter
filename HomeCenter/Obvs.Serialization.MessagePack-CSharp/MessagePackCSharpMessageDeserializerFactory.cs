using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MessagePack;
using MessagePack.Resolvers;
using Obvs.Configuration;

namespace Obvs.Serialization.MessagePack
{
    public class MessagePackCSharpMessageDeserializerFactory : IMessageDeserializerFactory
    {
        private readonly /*IFormatterResolver*/MessagePackSerializerOptions _resolver;

        public MessagePackCSharpMessageDeserializerFactory()
            : this(null)
        {
        }

        public MessagePackCSharpMessageDeserializerFactory(/*IFormatterResolver */MessagePackSerializerOptions resolver)
        {
            _resolver = resolver ?? MessagePackSerializer.DefaultOptions /*MessagePackSerializer.DefaultResolver*/;
        }

        public IEnumerable<IMessageDeserializer<TMessage>> Create<TMessage, TServiceMessage>(Func<Assembly, bool> assemblyFilter = null, Func<Type, bool> typeFilter = null)
            where TMessage : class
            where TServiceMessage : class
        {
            return MessageTypes.Get<TMessage, TServiceMessage>(assemblyFilter, typeFilter)
                .Select(type => typeof(MessagePackCSharpMessageDeserializer<>).MakeGenericType(type))
                .Select(deserializerGeneric => Activator.CreateInstance(deserializerGeneric, _resolver) as IMessageDeserializer<TMessage>)
                .ToArray();
        }
    }
}