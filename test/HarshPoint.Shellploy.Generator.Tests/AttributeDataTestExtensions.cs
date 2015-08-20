using HarshPoint;
using HarshPoint.ShellployGenerator;
using System;

internal static class AttributeDataTestExtensions
{
    public static Boolean Mandatory(this AttributeData data)
        => Equals(true, data.NamedArguments.GetValueOrDefault("Mandatory"));

    public static String ParameterSetName(this AttributeData data)
        => (String)data.NamedArguments.GetValueOrDefault("ParameterSetName");

    public static Int32? Position(this AttributeData data)
        => (Int32?)data.NamedArguments.GetValueOrDefault("Position");

    public static Boolean ValueFromPipeline(this AttributeData data)
        => Equals(true, data.NamedArguments.GetValueOrDefault("ValueFromPipeline"));
}
