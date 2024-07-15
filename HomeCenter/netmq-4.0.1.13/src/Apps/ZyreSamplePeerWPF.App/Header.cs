namespace SamplePeer
{
    public class Header
    {
        #region Properties
        public string Key { get; private set; }
        public string Value { get; private set; }
        #endregion

        #region Ctor
        public Header(string key, string value)
        {
            Key = key;
            Value = value;
        }
        #endregion
    }
}
