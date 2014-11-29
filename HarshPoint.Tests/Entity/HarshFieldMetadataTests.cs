using HarshPoint.Entity;
using HarshPoint.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Entity
{
    public class HarshFieldMetadataTests
    {
        private const String SomeTextFieldId = "d3f85bfe-e873-43be-a06f-77f41e13c69b";
        private const String SomeTextField = "SomeTextField";

        private static readonly Type DummyEntityType = typeof(DummyEntity);
        private static readonly TypeInfo DummyEntityTypeInfo = DummyEntityType.GetTypeInfo();

        [Fact]
        public void FieldId_is_set()
        {
            var prop = DummyEntityTypeInfo.GetProperty(SomeTextField);
            var fieldMd = new HarshFieldMetadata(prop, prop.GetCustomAttribute<FieldAttribute>());
            Assert.Equal(new Guid(SomeTextFieldId), fieldMd.FieldId);
        }

        [Fact]
        public void InternalName_is_set()
        {
            var prop = DummyEntityTypeInfo.GetProperty(SomeTextField);
            var fieldMd = new HarshFieldMetadata(prop, prop.GetCustomAttribute<FieldAttribute>());
            Assert.Equal(SomeTextField, fieldMd.InternalName);
        }

        [Fact]
        public void StaticName_is_set()
        {
            var prop = DummyEntityTypeInfo.GetProperty(SomeTextField);
            var fieldMd = new HarshFieldMetadata(prop, prop.GetCustomAttribute<FieldAttribute>());
            Assert.Equal(SomeTextField, fieldMd.StaticName);
        }
        private sealed class DummyEntity
        {
            [Field(SomeTextFieldId)]
            public String SomeTextField
            {
                get;
                set;
            }
        }
    }
}
