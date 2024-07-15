namespace SamplePeer
{
    public class Group
    {
        #region Properties
        public string GroupName { get; }
        #endregion

        #region Ctor
        public Group(string groupName)
        {
            GroupName = groupName;
        }
        #endregion
    }
}
