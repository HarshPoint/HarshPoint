using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public abstract class CommandBuilder
    {
        private readonly AttributeBuilder _cmdletAttribute
            = new AttributeBuilder(typeof(SMA.CmdletAttribute))
            {
                Arguments = { null, null }
            };

        private CommandBuilderContext _context = EmptyContext;
        private String _className;
        private String _name;

        protected CommandBuilder()
        {
            Attributes.Add(_cmdletAttribute);
        }

        public HashSet<String> Aliases { get; }
            = new HashSet<String>(StringComparer.OrdinalIgnoreCase);

        public Collection<AttributeBuilder> Attributes { get; }
            = new Collection<AttributeBuilder>();

        public String ClassName
        {
            get
            {
                if (_className != null)
                {
                    return _className;
                }

                if (Verb != null && Noun != null)
                {
                    return $"{Verb}{Noun}Command";
                }

                return null;
            }
            set { _className = value; }
        }

        public String DefaultParameterSetName
        {
            get
            {
                return (String)_cmdletAttribute.Properties
                    .GetValueOrDefault("DefaultParameterSetName");
            }
            set
            {
                _cmdletAttribute.Properties["DefaultParameterSetName"]
                    = value;
            }
        }

        public Boolean HasInputObject { get; set; }

        public HashSet<String> ImportedNamespaces { get; }
            = new HashSet<String>(StringComparer.Ordinal);

        public String Name
        {
            get
            {
                if (_name != null)
                {
                    return _name;
                }

                if (Verb != null && Noun != null)
                {
                    return $"{Verb}-{Noun}";
                }

                return null;
            }
            set { _name = value; }
        }

        public String Namespace { get; set; }

        public String Noun
        {
            get { return (String)_cmdletAttribute.Arguments[1]; }
            set { _cmdletAttribute.Arguments[1] = value; }
        }

        public String Verb
        {
            get { return (String)_cmdletAttribute.Arguments[0]; }
            set { _cmdletAttribute.Arguments[0] = value; }
        }

        public CommandBuilderContext Context
        {
            get
            {
                if (_context == null)
                {
                    throw Logger.Fatal.InvalidOperation(
                        SR.CommandBuilder_NoContext
                    );
                }

                return _context;
            }
            internal set { _context = value; }
        }

        internal PropertyModelContainer PropertyContainer { get; }
            = new PropertyModelContainer();

        public ParameterBuilder Parameter(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            return PropertyContainer.GetParameterBuilder(name);
        }

        public ParameterBuilder PositionalParameter(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            return PropertyContainer.GetParameterBuilder(name, isPositional: true);
        }

        public virtual CommandModel ToCommand()
        {
            var properties = CreateProperties();

            if (HasInputObject)
            {
                properties = properties.Concat(
                    InputObjectProperty
                );
            }

            properties = new ParameterPositionVisitor().Visit(properties);

            return new CommandModel(
                Aliases,
                Attributes.Select(a => a.ToModel()),
                ClassName,
                ImportedNamespaces,
                Name,
                Namespace,
                properties
            );
        }

        protected virtual IEnumerable<PropertyModel> CreateProperties()
            => PropertyContainer;

        internal static void ValidateParameterName(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (ReservedNames.Contains(name))
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(name),
                    SR.CommandBuilder_ReservedName,
                    name
                );
            }
        }

        public const String InputObjectIdentifier = "InputObject";

        private static readonly CommandBuilderContext EmptyContext
            = new CommandBuilderContext();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilder));

        private static readonly ImmutableHashSet<String> ReservedNames
            = ImmutableHashSet.Create(
                StringComparer.OrdinalIgnoreCase,
                InputObjectIdentifier
            );

        private static readonly AttributeModel ValueFromPipeline
            = new AttributeModel(typeof(SMA.ParameterAttribute))
                .SetProperty("ValueFromPipeline", true);

        private static readonly PropertyModel InputObjectProperty
            = new PropertyModelPositional(
                Int32.MaxValue,
                new PropertyModelInputObject(
                    new PropertyModelSynthesized(
                        InputObjectIdentifier,
                        typeof(Object),
                        ValueFromPipeline
                    )
                )
            );
    }
}
