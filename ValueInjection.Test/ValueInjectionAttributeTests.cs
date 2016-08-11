using System;
using ValueInjection.Test.Data;
using Xunit;

namespace ValueInjection.Test
{
    public class ValueInjectionAttributeTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionIfSourceTypeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionAttribute(null, "test", "test"));
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionIfKeyPropertyNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionAttribute(typeof(TestData), null, "test"));
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionIfKeyPropertyNameIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionAttribute(typeof(TestData), string.Empty, "test"));
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionIfSourcePropertyNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionAttribute(typeof(TestData), "test", null));
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionIfSourcePropertyNameIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionAttribute(typeof(TestData), "test", string.Empty));
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionIfKeyPropertyNameIsNull2()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionAttribute(null));
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionIfKeyPropertyNameIsEmpty2()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueInjectionAttribute(string.Empty));
        }
    }
}
