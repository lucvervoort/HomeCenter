namespace SamplePeer
{
    public class Peer
    {
        #region Properties
        public Guid SenderUuid { get;  }
        public string Address { get; set; }
        public string SenderName { get; }
        #endregion

        #region Ctor
        public Peer(string senderName, Guid senderUuid, string address)
        {
            SenderName = senderName;
            SenderUuid = senderUuid;
            Address = address;
        }
        #endregion
    }
}
