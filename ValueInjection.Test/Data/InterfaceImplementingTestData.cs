namespace ValueInjection.Test.Data
{
    public class InterfaceImplementingTestData : IHasRemoteTestData
    {
        public int ValueKey { get; set; }
        public RemoteTestData RemoteTestData { get; set; }
    }
}
