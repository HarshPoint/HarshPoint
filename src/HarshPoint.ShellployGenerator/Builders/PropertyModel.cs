using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class PropertyModel : Chain<PropertyModel>
    {
        private readonly String _identifier;
        private readonly Int32? _sortOrder;

        protected PropertyModel() { }

        protected PropertyModel(PropertyModel next)
            : base(next)
        {
        }

        protected PropertyModel(Int32? sortOrder, PropertyModel next)
            : base(next)
        {
            _sortOrder = sortOrder;
        }

        protected PropertyModel(String identifier)
        {
            _identifier = identifier;
        }

        public String Identifier => _identifier ?? NextElement?.Identifier;

        public Int32? SortOrder => _sortOrder ?? NextElement?.SortOrder;

        public IEnumerable<T> ElementsOfType<T>()
            => Elements.OfType<T>();

        public T FirstElementOfType<T>()
            => ElementsOfType<T>().FirstOrDefault();

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public Boolean HasElementsOfType<T>()
            => ElementsOfType<T>().Any();

        public virtual PropertyModel InsertIntoContainer(
            PropertyModel existing
        )
            => existing == null ? this : Append(existing);

        protected internal abstract PropertyModel Accept(
            PropertyModelVisitor visitor
        );

        internal PropertyModel Append(PropertyModel other)
            => (PropertyModel)base.Append(other);

        internal PropertyModel WithNext(PropertyModel next)
            => (PropertyModel)base.WithNext(next);

        protected internal new PropertyModel NextElement
            => base.NextElement;
    }
}
