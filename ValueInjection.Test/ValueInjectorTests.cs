using System;
using System.Collections.Generic;
using System.Linq;
using ValueInjection.Test.Data;
using Xunit;

namespace ValueInjection.Test
{
    public class ValueInjectorTests : IDisposable
    {
        private readonly IValueObtainer<RemoteTestData> _remoteValueObtainer;

        public ValueInjectorTests()
        {
            _remoteValueObtainer = new TestValueObtainer(f => new RemoteTestData { RemoteValue = f.ToString() });
        }

        [Fact]
        public void ShouldInjectRemoteValueQueryable()
        {
            ValueInjector.UseValueObtainer(_remoteValueObtainer);
            var injected = new List<TestData>
            {
                new TestData
                {
                    ValueKey = 1
                }
            }.AsQueryable().ToValueInjection();

            foreach (var element in injected)
            {
                Assert.NotNull(element);
                Assert.Equal(element.ValueKey.ToString(), element.Value);
            }
        }

        [Fact]
        public void ShouldInjectRemoteValueEnumerable()
        {
            ValueInjector.UseValueObtainer(_remoteValueObtainer);
            var injected = new List<TestData>
            {
                new TestData
                {
                    ValueKey = 1
                }
            }.ToValueInjection();

            foreach (var element in injected)
            {
                Assert.NotNull(element);
                Assert.Equal(element.ValueKey.ToString(), element.Value);
            }
        }

        [Fact]
        public void ShouldHandleReferenceTypes()
        {
            ValueInjector.UseValueObtainer(_remoteValueObtainer);
            var injected = new List<ReferenceTestData>
            {
                new ReferenceTestData {
                    ReferencedData = new TestData
                    {
                        ValueKey = 1
                    }
                }
            }.ToValueInjection();

            foreach (var element in injected)
            {
                Assert.NotNull(element);
                Assert.Equal(element.ReferencedData.ValueKey.ToString(), element.ReferencedData.Value);
            }
        }

        [Fact]
        public void ShouldHandleNullValuesInReferenceTypes()
        {
            var injected = new List<ReferenceTestData>
            {
                new ReferenceTestData {
                   ReferencedData = null
                }
            }.ToValueInjection();

            foreach (var element in injected)
            {
                Assert.NotNull(element);
            }
        }

        [Fact]
        public void ShouldNotTouchPropertiesIfNoInjectionIntended()
        {
            var testData = new TestDataWithoutInjection();

            ValueInjector.InjectValues(testData);
        }

        [Fact]
        public void ShouldHandleNullValues()
        {
            ValueInjector.InjectValues(null);
        }

        [Fact]
        public void ShouldThrowExceptionIfKeyPropertyIsNull()
        {
            ValueInjector.UseValueObtainer(_remoteValueObtainer);
            var testData = new TestData
            {
                ValueKey = null
            };

            Assert.Throws<InvalidOperationException>(() => ValueInjector.InjectValues(testData));
        }

        [Fact]
        public void ShouldThrowExceptionIfKeyPropertyNotFound()
        {
            var testData = new WrongKeyPropertyTestData();

            Assert.Throws<InvalidOperationException>(() => ValueInjector.InjectValues(testData));
        }

        [Fact]
        public void ShouldThrowExceptionIfValueObtainerReturnsNull()
        {
            ValueInjector.UseValueObtainer(new NullReturningObtainer());

            var testData = new TestData { ValueKey = 1 };
            Assert.Throws<InvalidOperationException>(() => ValueInjector.InjectValues(testData));
        }

        [Fact]
        public void ShouldThrowExceptionIfDestinationPropertyCannotBeWritten()
        {
            ValueInjector.UseValueObtainer(_remoteValueObtainer);
            var testData = new ReadonlyDestinationPropertyTestData();

            Assert.Throws<InvalidOperationException>(() => ValueInjector.InjectValues(testData));
        }

        [Fact]
        public void ShouldThrowExceptionIfNoValueObtainerFoundForType()
        {
            ValueInjector.Clear();
            var testData = new TestData { ValueKey = 1 };

            Assert.Throws<NotSupportedException>(() => ValueInjector.InjectValues(testData));
        }

        [Fact]
        public void ShouldThrowExceptionIfSourcePropertyNotFound()
        {
            var testData = new WrongSourcePropertyTestData();

            Assert.Throws<InvalidOperationException>(() => ValueInjector.InjectValues(testData));
        }

        [Fact]
        public void ShouldThrowExceptionIfSourcePropertyCannotBeRead()
        {
            var testData = new UnreadableSourcePropertyTestData { ValueKey = 1 };

            Assert.Throws<InvalidOperationException>(() => ValueInjector.InjectValues(testData));
        }

        [Fact]
        public void ShouldCacheObtainedValuesForSameKey()
        {
            var valueObtainer = new CountingValueObtainer();
            ValueInjector.UseValueObtainer(valueObtainer);
            var testData1 = new TestData { ValueKey = 1 };
            var testData2 = new TestData { ValueKey = 1 };

            ValueInjector.InjectValues(testData1);
            ValueInjector.InjectValues(testData2);

            Assert.Equal(1, valueObtainer.ObtainerCallCounter);
        }

        [Fact]
        public void ShouldNotCacheObtainedValuesForDifferentKeys()
        {
            var valueObtainer = new CountingValueObtainer();
            ValueInjector.UseValueObtainer(valueObtainer);
            var testData1 = new TestData { ValueKey = 1 };
            var testData2 = new TestData { ValueKey = 2 };

            ValueInjector.InjectValues(testData1);
            ValueInjector.InjectValues(testData2);

            Assert.Equal(2, valueObtainer.ObtainerCallCounter);
        }

        [Fact]
        public void ShouldInjectValuesInChildEnumerable()
        {
            ValueInjector.UseValueObtainer(_remoteValueObtainer);
            var testData = new EnumerableChildPropertyTestData
            {
                TestDatas = new List<TestData>
                {
                    new TestData {ValueKey = 1},
                    new TestData {ValueKey = 2}
                }
            };

            ValueInjector.InjectValues(testData);

            foreach (var element in testData.TestDatas)
            {
                Assert.NotNull(element.Value);
                Assert.Equal(element.ValueKey.ToString(), element.Value);
            }
        }

        [Fact]
        public void ShouldNotInjectValuesIfChildEnumerableIsNull()
        {
            ValueInjector.UseValueObtainer(_remoteValueObtainer);
            var testData = new EnumerableChildPropertyTestData
            {
                TestDatas = null
            };

            ValueInjector.InjectValues(testData);

            Assert.Null(testData.TestDatas);
        }

        [Fact]
        public void ShouldIgnoreEnumerableGenericTypesLikeString()
        {
            var stringEnumerable = new StringEnumerableChildPropertyTestData();

            ValueInjector.InjectValues(stringEnumerable);
        }

        [Fact]
        public void ShouldInjectWholeSourceInstance()
        {
            ValueInjector.UseValueObtainer(_remoteValueObtainer);
            var testData = new ReferenceObjectInjectionTestData {ValueKey = 1};

            ValueInjector.InjectValues(testData);

            Assert.NotNull(testData.RemoteTestData);
        }

        [Fact]
        public void ShouldInjectInheritedProperties()
        {
            ValueInjectionMetadataBuilder.ConfigureReplacement<IHasRemoteTestData>(f => f.Of(p => p.RemoteTestData)
                                                                                        .With<RemoteTestData>()
                                                                                        .FromKey(td => td.ValueKey));

            ValueInjector.UseValueObtainer(_remoteValueObtainer);
            var testData = new InterfaceImplementingTestData {ValueKey = 1};

            ValueInjector.InjectValues(testData);

            Assert.NotNull(testData.RemoteTestData);
        }

        [Fact]
        public void ShouldNotAccessGetterOfIrrelevantReferencePropertyType()
        {
            var testData = new TestDataWithExceptionThrowingGetter();

            ValueInjector.InjectValues(testData);
        }

        [Fact]
        public void ShouldNotAccessGetterOfEnumerable()
        {
            var testData = new TestDataWithExceptionThrowingEnumerableGetter();

            ValueInjector.InjectValues(testData);
        }

        public void Dispose()
        {
            ValueInjector.Clear();
        }
    }
}
