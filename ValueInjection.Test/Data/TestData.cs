using System;

namespace ValueInjection.Test.Data
{
    [Serializable]
    public class TestData
    {
        public int? ValueKey { get; set; }

        [ValueInjection(typeof(RemoteTestData), nameof(ValueKey), nameof(RemoteTestData.RemoteValue))]
        public string Value { get; set; }

    }
}
