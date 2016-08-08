using System;

namespace ValueInjection.Test.Data
{
    public class TestValueObtainer : IValueObtainer<RemoteTestData>
    {
        private readonly Func<object, RemoteTestData> _obtainerFunc;

        public TestValueObtainer(Func<object, RemoteTestData> obtainerFunc )
        {
            _obtainerFunc = obtainerFunc;
        }

        public RemoteTestData ObtainValue(object key)
        {
            return _obtainerFunc(key);
        }

        object IValueObtainer.ObtainValue(object key)
        {
            return ObtainValue(key);
        }
    }
}
