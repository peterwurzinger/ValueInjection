namespace ValueInjection.Test.Data
{
    public class ReadonlyDestinationPropertyTestData
    {
        public int ValueKey { get; set; }

        [ValueInjection(typeof(RemoteTestData), nameof(ValueKey), nameof(RemoteTestData.RemoteValue))]
        public string InjectedValue { get; }
    }
}
