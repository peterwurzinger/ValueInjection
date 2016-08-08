namespace ValueInjection.Test.Data
{
    public class ReadonlyKeyPropertyTestData
    {
        private int KeyValue { get; set; }

        [ValueInjection(typeof(RemoteTestData), nameof(KeyValue), nameof(RemoteTestData.RemoteValue))]
        public string InjectedValue { get; set; }

    }
}
