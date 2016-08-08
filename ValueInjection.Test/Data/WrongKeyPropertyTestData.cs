using System;

namespace ValueInjection.Test.Data
{
    [Serializable]
    public class WrongKeyPropertyTestData
    {
        [ValueInjection(typeof(RemoteTestData), "NotExistingProperty", nameof(RemoteTestData.RemoteValue))]
        public string InjectedValue { get; set; }
    }
}
