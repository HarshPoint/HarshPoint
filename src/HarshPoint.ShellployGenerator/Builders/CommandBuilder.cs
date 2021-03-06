﻿using HarshPoint.ShellployGenerator.CodeGen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SMA = System.Management.Automation;
using static System.FormattableString;

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
            PropertyContainer = new PropertyModelContainer(this);

            Attributes.Add(_cmdletAttribute);
            BaseTypes.Add(typeof(SMA.PSCmdlet).FullName);
        }

        public HashSet<String> Aliases { get; }
            = new HashSet<String>(StringComparer.OrdinalIgnoreCase);

        public Collection<AttributeBuilder> Attributes { get; }
            = new Collection<AttributeBuilder>();

        public HashSet<String> BaseTypes { get; }
            = new HashSet<String>(StringComparer.Ordinal);

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
                    return Invariant($"{Verb}{Noun}Command");
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
                    return Invariant( $"{Verb}-{Noun}");
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

        public ParameterBuilder Parameter(String name)
            => PropertyContainer.GetParameterBuilder(name);

        public ParameterBuilder PositionalParameter(String name)
            => PropertyContainer.GetParameterBuilder(name, isPositional: true);

        public CommandModel ToCommand() => ToCommand(null);

        public virtual CommandModel ToCommand(
            IEnumerable<PropertyModel> properties
        )
        {
            properties = properties ?? CreateProperties();

            properties = RemoveIgnoredUnsynthesized.Visit(properties);
            properties = new ParameterPositionVisitor().Visit(properties);

            return new CommandModel(
                Aliases,
                Attributes.Select(a => a.ToModel()),
                BaseTypes,
                ClassName,
                ImportedNamespaces,
                Name,
                Namespace,
                properties
            );
        }

        public virtual CommandCodeGenerator ToCodeGenerator()
            => new CommandCodeGenerator(
                ToCommand()
            );

        protected virtual IEnumerable<PropertyModel> CreateProperties()
            => PropertyContainer;

        protected internal virtual void ValidatePropertyName(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }
        }

        private static readonly CommandBuilderContext EmptyContext
            = new CommandBuilderContext();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilder));

        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        protected static readonly PropertyModelVisitor RemoveIgnoredUnsynthesized
            = new RemoveIgnoredOrUnsynthesizedVisitor();
    }
}
