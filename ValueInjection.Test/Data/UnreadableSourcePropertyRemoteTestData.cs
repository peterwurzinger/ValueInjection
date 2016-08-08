namespace ValueInjection.Test.Data
{
    public class UnreadableSourcePropertyRemoteTestData
    {
        private string _remoteValue;
        public string RemoteValue
        {
            set { _remoteValue = value; }
        }
    }
}
