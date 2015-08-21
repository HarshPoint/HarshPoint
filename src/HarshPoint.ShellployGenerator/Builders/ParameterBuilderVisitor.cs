using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class ParameterBuilderVisitor
    {
        public virtual ParameterBuilder Visit(ParameterBuilder builder)
        {
            if (builder != null)
            {
                return builder.Accept(this);
            }

            return null;
        }

        protected internal virtual ParameterBuilder VisitDefaultValue(
            ParameterBuilderDefaultValue defaultValueBuilder
        )
            => VisitNext(defaultValueBuilder);

        protected internal virtual ParameterBuilder VisitIgnored(
            ParameterBuilderIgnored ignoredBuilder
        )
            => VisitNext(ignoredBuilder);

        protected internal virtual ParameterBuilder VisitRenamed(
            ParameterBuilderRenamed renamedBuilder
        )
            => VisitNext(renamedBuilder);

        protected internal virtual ParameterBuilder VisitPositional(
            ParameterBuilderPositional positionalBuilder
        )
            => VisitNext(positionalBuilder);

        protected internal virtual ParameterBuilder VisitInputObject(
            ParameterBuilderInputObject inputObjectBuilder
        )
            => VisitNext(inputObjectBuilder);

        protected internal virtual ParameterBuilder VisitFixed(
            ParameterBuilderFixed fixedBuilder
        )
            => VisitNext(fixedBuilder);

        protected internal virtual ParameterBuilder VisitSynthesized(
            ParameterBuilderSynthesized synthesizedBuilder
        )
            => VisitNext(synthesizedBuilder);

        private ParameterBuilder VisitNext(ParameterBuilder builder)
        {
            if (builder?.NextElement != null)
            {
                return builder.WithNextElement(
                    Visit(builder.NextElement)
                );
            }

            return null;
        }
    }
}
