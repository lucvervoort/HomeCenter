using System;
using System.Threading;
using Obvs.Types;
//using Obvs.Serialization.ProtoBuf;
//using ProtoBuf;
using Obvs.Serialization.MessagePack;

namespace Obvs.NetMQ.Tests.Console.Publisher
{
    class Program
	{
		static void Main(string[] args)
		{
			int max = 50;
			CountdownEvent cd = new(max);

			string endPoint = "tcp://localhost:5557";
			System.Console.WriteLine("Publishing on {0}\n", endPoint);

			const string topic = "TestTopicxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

			{
				var publisher = new MessagePublisher<IMessage>("tcp://localhost:5557",
					new /*ProtoBufMessageSerializer*/MessagePackCSharpMessageSerializer(),
					topic);

				for (int i = 0; i < max; i++)
				{
					publisher.PublishAsync(new Message1AndItIs32CharactersLongForSureDefinitionForSure()
					{
						Id = i
					});

					Thread.Sleep(TimeSpan.FromSeconds(0.5));
					System.Console.WriteLine("Published: {0}", i);
				}
			}

			System.Console.WriteLine("[Finished - any key to continue]");
			System.Console.ReadKey();
		}
	}
}
