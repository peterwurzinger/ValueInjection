using System;

namespace ValueInjection.Test.Data
{
    [Serializable]
    public class WrongSourcePropertyTestData
    {
        public int ValueKey { get; set; }

        [ValueInjection(typeof(RemoteTestData), nameof(ValueKey), "NotExistingProperty")]
        public string InjectedValue { get; set; }
    }
}
