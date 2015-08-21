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
        private readonly AttributeData _cmdletAttribute
            = new AttributeData(typeof(SMA.CmdletAttribute))
            {
                ConstructorArguments = { null, null }
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

        public Collection<AttributeData> Attributes { get; }
            = new Collection<AttributeData>();

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
                    return $"{Verb}${Noun}Command";
                }

                return null;
            }
            set { _className = value; }
        }

        public String DefaultParameterSetName
        {
            get
            {
                return (String)_cmdletAttribute.NamedArguments
                    .GetValueOrDefault("DefaultParameterSetName");
            }
            set
            {
                _cmdletAttribute.NamedArguments["DefaultParameterSetName"]
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
            get { return (String)_cmdletAttribute.ConstructorArguments[1]; }
            set { _cmdletAttribute.ConstructorArguments[1] = value; }
        }

        public String Verb
        {
            get { return (String)_cmdletAttribute.ConstructorArguments[0]; }
            set { _cmdletAttribute.ConstructorArguments[0] = value; }
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

        internal ParameterBuilderContainer ParameterBuilders { get; }
            = new ParameterBuilderContainer();

        public ParameterBuilderFactory Parameter(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            return ParameterBuilders.GetFactory(name);
        }

        public ParameterBuilderFactory PositionalParameter(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            return ParameterBuilders.GetFactory(name, isPositional: true);
        }

        public virtual ShellployCommand ToCommand()
        {
            var properties = CreateProperties();

            if (HasInputObject)
            {
                properties = properties.Concat(
                    InputObjectParameter.Value.Synthesize()
                );
            }

            var propertyArray = properties.ToImmutableArray();
            AssignParameterPositions(propertyArray);

            return new ShellployCommand
            {
                Attributes = Attributes.ToImmutableArray(),
                Aliases = Aliases.ToImmutableArray(),
                ClassName = ClassName,
                HasInputObject = HasInputObject,
                Name = Name,
                Namespace = Namespace,
                Properties = propertyArray,
                Usings = ImportedNamespaces.ToImmutableArray(),
            };
        }

        protected virtual IEnumerable<ShellployCommandProperty> CreateProperties()
            => Enumerable.Empty<ShellployCommandProperty>();

        private static void AssignParameterPositions(
            IEnumerable<ShellployCommandProperty> properties
        )
        {
            var currentPosition = 0;

            foreach (var prop in properties.Where(p => p.IsPositional))
            {
                foreach (var attr in prop.ParameterAttributes)
                {
                    attr.NamedArguments["Position"] = currentPosition;
                }

                currentPosition++;
            }
        }

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

        private static readonly KeyValuePair<String, ParameterBuilder> InputObjectParameter
            = new KeyValuePair<String, ParameterBuilder>(
                InputObjectPropertyName,
                new ParameterBuilderInputObject(
                    new ParameterBuilderAttributeNamedArgument(
                        typeof(SMA.ParameterAttribute),
                        "ValueFromPipeline",
                        true,
                        new ParameterBuilderSynthesized(
                            InputObjectPropertyName,
                            typeof(Object)
                        )
                    )
                )
            );

        private static readonly CommandBuilderContext EmptyContext
            = new CommandBuilderContext();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilder));

        private static readonly ImmutableHashSet<String> ReservedNames
            = ImmutableHashSet.Create(
                StringComparer.OrdinalIgnoreCase,
                InputObjectPropertyName
            );

        public const String InputObjectPropertyName = "InputObject";
    }
}
