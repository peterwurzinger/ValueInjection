using System;

namespace ValueInjection.Test.Data
{
    public class TestDataWithoutInjection
    {
        public int Property
        {
            get
            {
                throw new InvalidOperationException();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }
    }
}
