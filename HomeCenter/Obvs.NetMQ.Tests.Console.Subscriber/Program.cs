using System;
using Obvs.Serialization;
using Obvs.Types;
//using Obvs.Serialization.ProtoBuf;
//using ProtoBuf;
using Obvs.Serialization.MessagePack;

namespace Obvs.NetMQ.Tests.Console.Subscriber
{
    class Program
	{
		static void Main(string[] args)
		{
			string endPoint = "tcp://localhost:5557";
			System.Console.WriteLine("Listening on {0}\n", endPoint);

			const string topic = "TestTopicxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

			IDisposable sub;
			{
				var source = new MessageSource<IMessage>(endPoint,
					new IMessageDeserializer<IMessage>[]
					{
						new /*ProtoBufMessageDeserializer*/MessagePackCSharpMessageDeserializer<Message1AndItIs32CharactersLongForSureDefinitionForSure>(),
					},
					topic);

				sub = source.Messages.Subscribe(msg =>
					{
						System.Console.WriteLine("Received: " + msg);
					},
				   err => System.Console.WriteLine("Error: " + err));
			}

			System.Console.ReadKey();
		}
	}
}
