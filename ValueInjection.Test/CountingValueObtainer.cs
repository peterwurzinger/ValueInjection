using ValueInjection.Test.Data;

namespace ValueInjection.Test
{
    public class CountingValueObtainer : IValueObtainer<RemoteTestData>
    {
        public int ObtainerCallCounter { get; private set; }

        public CountingValueObtainer()
        {
            ObtainerCallCounter = 0;
        }

        public RemoteTestData ObtainValue(object key)
        {
            ObtainerCallCounter++;
            return new RemoteTestData
            {
                RemoteValue = "My Remote Value"
            };
        }

        object IValueObtainer.ObtainValue(object key)
        {
            return ObtainValue(key);
        }
    }
}
