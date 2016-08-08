using System;
using System.Collections.Generic;

namespace ValueInjection.Test.Data
{
    [Serializable]
    public class EnumerableChildPropertyTestData
    {
        public IEnumerable<TestData> TestDatas { get; set; }
    }
}
