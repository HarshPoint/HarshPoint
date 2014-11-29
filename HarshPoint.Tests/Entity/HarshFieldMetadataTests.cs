using HarshPoint.Entity;
using HarshPoint.Entity.Metadata;
using System;
using System.Reflection;
using Xunit;

namespace HarshPoint.Tests.Entity
{
    public class HarshFieldMetadataTests
    {
        private static readonly Type TestEntityType = typeof(TestEntity);
        private static readonly TypeInfo TestEntityTypeInfo = TestEntityType.GetTypeInfo();

        [Fact]
        public void FieldId_is_set()
        {
            var prop = TestEntityTypeInfo.GetProperty(TestEntity.SomeTextFieldPropertyName);
            var fieldMd = new HarshFieldMetadata(prop, prop.GetCustomAttribute<FieldAttribute>());
            Assert.Equal(new Guid(TestEntity.SomeTextFieldId), fieldMd.FieldId);
        }

        [Fact]
        public void InternalName_is_set()
        {
            var prop = TestEntityTypeInfo.GetProperty(TestEntity.SomeTextFieldPropertyName);
            var fieldMd = new HarshFieldMetadata(prop, prop.GetCustomAttribute<FieldAttribute>());
            Assert.Equal(TestEntity.SomeTextFieldPropertyName, fieldMd.InternalName);
        }

        [Fact]
        public void StaticName_is_set()
        {
            var prop = TestEntityTypeInfo.GetProperty(TestEntity.SomeTextFieldPropertyName);
            var fieldMd = new HarshFieldMetadata(prop, prop.GetCustomAttribute<FieldAttribute>());
            Assert.Equal(TestEntity.SomeTextFieldPropertyName, fieldMd.StaticName);
        }
    }
}
