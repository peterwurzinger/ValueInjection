using System;

namespace ValueInjection.Test.Data
{
    [Serializable]
    public class RecursiveTestData
    {
        public RecursiveTestData TestData { get; set; }

        public int RemoteValueKey { get; set; }

        private bool _remoteValueAccessed;
        private string _remoteValue;
        public string RemoteValue
        {
            get
            {
                return _remoteValue;
            }
            set
            {
                if (!_remoteValueAccessed)
                {
                    _remoteValue = value;
                    _remoteValueAccessed = true;
                }
                else
                    throw new InvalidOperationException("Second access of Property-Setter!");
            }
        }
    }
}
