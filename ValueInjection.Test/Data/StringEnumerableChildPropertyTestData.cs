using System;
using System.Collections.Generic;

namespace ValueInjection.Test.Data
{
    public class StringEnumerableChildPropertyTestData
    {
        public IEnumerable<string> StringEnumerable { get { throw new InvalidOperationException();} set { throw new InvalidOperationException();} } 
    }
}
