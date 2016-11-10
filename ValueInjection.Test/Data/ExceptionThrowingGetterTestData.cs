using System;
using System.Collections;

namespace ValueInjection.Test.Data
{
    [Serializable]
    public class TestDataWithExceptionThrowingGetter
    {
        /// <summary>
        /// This Property will throw an exception if the getter is accessed although the property type is not relevant for injection
        /// </summary>
        public RemoteTestData ExceptionThrowingGetterTestData
        {
            get { throw new Exception("Thou shall not access this getter!"); }
        }
    }

    [Serializable]
    public class TestDataWithExceptionThrowingEnumerableGetter
    {
        /// <summary>
        /// This Property will throw an exception if the getter is accessed although the property type is not relevant for injection
        /// </summary>
        public IEnumerable ExceptionThrowingGetterTestData
        {
            get { throw new Exception("Thou shall not access this getter!"); }
        }
    }
}
