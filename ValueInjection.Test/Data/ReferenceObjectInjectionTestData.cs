namespace ValueInjection.Test.Data
{
    public class ReferenceObjectInjectionTestData
    {
        public int ValueKey { get; set; }

        [ValueInjection(nameof(ValueKey))]
        public RemoteTestData RemoteTestData { get; set; }
    }
}
