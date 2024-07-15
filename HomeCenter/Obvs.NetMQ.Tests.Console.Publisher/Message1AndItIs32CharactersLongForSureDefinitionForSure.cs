using Obvs.Types;
//using Obvs.Serialization.ProtoBuf;
//using ProtoBuf;
using MessagePack;

namespace Obvs.NetMQ.Tests.Console.Publisher
{
    /*
	[ProtoContract]
	public class Message1AndItIs32CharactersLongForSureDefinitionForSure : IMessage
	{
		public Message1AndItIs32CharactersLongForSureDefinitionForSure()
		{

		}

		[ProtoMember(1)]
		public int Id { get; set; }

		public override string ToString()
		{
			return "Message1AndItIs32CharactersLongForSureDefinitionForSure-" + Id;
		}
	}
	*/

    [MessagePackObject]
    public class Message1AndItIs32CharactersLongForSureDefinitionForSure : IMessage
    {
        public Message1AndItIs32CharactersLongForSureDefinitionForSure()
        {

        }

		[MessagePack.Key(0)]
        public int Id { get; set; }

        public override string ToString()
        {
            return "Message1AndItIs32CharactersLongForSureDefinitionForSure-" + Id;
        }
    }

}
