namespace ValueInjection.Test.Data
{
    public class UnreadableSourcePropertyTestData
    {
        public int ValueKey { get; set; }

        [ValueInjection(typeof(UnreadableSourcePropertyRemoteTestData), nameof(ValueKey), "RemoteValue")]
        public string InjectedValue { get; set; }
    }
}
