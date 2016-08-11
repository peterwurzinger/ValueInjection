namespace ValueInjection.Test.Data
{
    public interface IHasRemoteTestData
    {
        int ValueKey { get; set; }
        
        RemoteTestData RemoteTestData { get; set; }
    }
}
