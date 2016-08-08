namespace ValueInjection.Test.Data
{
    public class NullReturningObtainer : IValueObtainer<RemoteTestData>
    {
        public RemoteTestData ObtainValue(object key)
        {
            return null;
        }

        object IValueObtainer.ObtainValue(object key)
        {
            return ObtainValue(key);
        }
    }
}
