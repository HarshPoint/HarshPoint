using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
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

        protected PropertyModel(
            PropertyModel next = null,
            String identifier = null,
            Int32? sortOrder = null
        )
            : base(next)
        {
            _identifier = identifier;
            _sortOrder = sortOrder;
        }

        public String Identifier => _identifier ?? NextElement?.Identifier;

        public Int32? SortOrder => _sortOrder ?? NextElement?.SortOrder;

        public IEnumerable<T> ElementsOfType<T>() where T : PropertyModel
            => Elements.OfType<T>();

        public T FirstElementOfType<T>() where T : PropertyModel
            => ElementsOfType<T>().FirstOrDefault();

        public Boolean HasElementsOfType<T>() where T : PropertyModel
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

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModel));
    }
}
