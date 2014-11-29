using HarshPoint.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Tests.Entity
{
    internal sealed class TestEntity
    {
        [Field(SomeTextFieldId)]
        public String SomeTextField
        {
            get;
            set;
        }

        public const String SomeTextFieldId = "d3f85bfe-e873-43be-a06f-77f41e13c69b";
        public const String SomeTextFieldPropertyName = "SomeTextField";
    }
}
