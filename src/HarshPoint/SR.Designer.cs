﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HarshPoint {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HarshPoint.SR", typeof(SR).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified argument contains elements already contained in this chain..
        /// </summary>
        internal static string Chain_ElementAlreadyContained {
            get {
                return ResourceManager.GetString("Chain_ElementAlreadyContained", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified object hasn&apos;t been loaded yet, cannot determine if it is null..
        /// </summary>
        internal static string ClientObject_IsNullNotLoaded {
            get {
                return ResourceManager.GetString("ClientObject_IsNullNotLoaded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0}.{1} method may not return any objects of type {2}.
        ///
        ///All queries must be results of Include() calls. Otherwise, CSOM automatically loads default properties which might be missing on an on-premises SharePoint 2013..
        /// </summary>
        internal static string ClientObjectQueryResolveBuilder_CollectionQuery {
            get {
                return ResourceManager.GetString("ClientObjectQueryResolveBuilder_CollectionQuery", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &quot;{0}&quot; implementation of CreateQueries resulted in a null value..
        /// </summary>
        internal static string ClientObjectResolveBuilder_CreateQueriesReturnedNull {
            get {
                return ResourceManager.GetString("ClientObjectResolveBuilder_CreateQueriesReturnedNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The array of state objects must have the same length as the number of this resolver&apos;s elements.
        ///
        ///Number of state objects: {0}
        ///Number of resolver elements {1}.
        /// </summary>
        internal static string ClientObjectResolveBuilder_StateCountNotEqualToElementCount {
            get {
                return ResourceManager.GetString("ClientObjectResolveBuilder_StateCountNotEqualToElementCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified sub-expression doesn&apos;t pass a new array expression as the second argument to the Include call:
        ///
        ///{0}.
        /// </summary>
        internal static string ClientObjectResolveQueryProcessor_IncludeArgNotArray {
            get {
                return ResourceManager.GetString("ClientObjectResolveQueryProcessor_IncludeArgNotArray", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MaxRecursionDepth cannot be a negative number..
        /// </summary>
        internal static string ClientObjectResolveQueryProcessor_MaxRecursionDepthNegative {
            get {
                return ResourceManager.GetString("ClientObjectResolveQueryProcessor_MaxRecursionDepthNegative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The content type ID could not be determined because no parent of the specified type &apos;{0}&apos; defines an absolute content type ID..
        /// </summary>
        internal static string ContentTypeIdBuilder_NoAbsoluteIDInHierarchy {
            get {
                return ResourceManager.GetString("ContentTypeIdBuilder_NoAbsoluteIDInHierarchy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The content type ID could not be determined because the type &apos;{0}&apos; has no ContentTypeAttribute..
        /// </summary>
        internal static string ContentTypeIdBuilder_NoContentTypeAttribute {
            get {
                return ResourceManager.GetString("ContentTypeIdBuilder_NoContentTypeAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The content type ID cannot be determined because the type &apos;{0}&apos;, from which the specified type &apos;{1}&apos; inherits, has no ContentTypeAttribute..
        /// </summary>
        internal static string ContentTypeIdBuilder_NoContentTypeAttributeBaseClass {
            get {
                return ResourceManager.GetString("ContentTypeIdBuilder_NoContentTypeAttributeBaseClass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The DeferredResolveBuilder is not initialized. You must first call InitializeContext method..
        /// </summary>
        internal static string DeferredResolveBuilder_InnerNotInitialized {
            get {
                return ResourceManager.GetString("DeferredResolveBuilder_InnerNotInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sequence doesn&apos;t contain more elements..
        /// </summary>
        internal static string EnumeratorExtensions_EnumerationEnded {
            get {
                return ResourceManager.GetString("EnumeratorExtensions_EnumerationEnded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value cannot be null or empty string..
        /// </summary>
        internal static string Error_ArgumentNullOrEmpty {
            get {
                return ResourceManager.GetString("Error_ArgumentNullOrEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value cannot be null or whitespace-only string..
        /// </summary>
        internal static string Error_ArgumentNullOrWhitespace {
            get {
                return ResourceManager.GetString("Error_ArgumentNullOrWhitespace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified value &quot;{0}&quot; is not a valid {1} enumerated value..
        /// </summary>
        internal static string Error_InvalidEnum {
            get {
                return ResourceManager.GetString("Error_InvalidEnum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified object &apos;{0}&apos; is not a subtype of any of the following types: {1}..
        /// </summary>
        internal static string Error_ObjectNotAssignableToMany {
            get {
                return ResourceManager.GetString("Error_ObjectNotAssignableToMany", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified object &apos;{0}&apos; is not a subtype of &apos;{1}&apos;..
        /// </summary>
        internal static string Error_ObjectNotAssignableToOne {
            get {
                return ResourceManager.GetString("Error_ObjectNotAssignableToOne", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The property {0} cannot be null..
        /// </summary>
        internal static string Error_PropertyNull {
            get {
                return ResourceManager.GetString("Error_PropertyNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value cannot be an empty sequence..
        /// </summary>
        internal static string Error_SequenceEmpty {
            get {
                return ResourceManager.GetString("Error_SequenceEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified type &apos;{0}&apos; is not assignable from &apos;{1}&apos;..
        /// </summary>
        internal static string Error_TypeNotAssignableFrom {
            get {
                return ResourceManager.GetString("Error_TypeNotAssignableFrom", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A MemberExpression could not be found in the specified Expression..
        /// </summary>
        internal static string ExpressionExtensions_MemberExpressionNotFound {
            get {
                return ResourceManager.GetString("ExpressionExtensions_MemberExpressionNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Field ID cannot be an empty Guid..
        /// </summary>
        internal static string FieldAttribute_EmptyGuid {
            get {
                return ResourceManager.GetString("FieldAttribute_EmptyGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified string is not a valid Guid..
        /// </summary>
        internal static string FieldAttribute_InvalidGuid {
            get {
                return ResourceManager.GetString("FieldAttribute_InvalidGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Feature ID &apos;{0}&apos; not found..
        /// </summary>
        internal static string HarshActivateFeature_FeatureNotFound {
            get {
                return ResourceManager.GetString("HarshActivateFeature_FeatureNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified expression &apos;{0}&apos; is not a property or field access expression..
        /// </summary>
        internal static string HarshCloneable_ExpressionNotFieldOrProperty {
            get {
                return ResourceManager.GetString("HarshCloneable_ExpressionNotFieldOrProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified sequence cannot be empty..
        /// </summary>
        internal static string HarshCompositeProvisioner_SequenceEmpty {
            get {
                return ResourceManager.GetString("HarshCompositeProvisioner_SequenceEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Id and ParentContentType properties cannot be both set..
        /// </summary>
        internal static string HarshContentType_BothIdAndParentContentTypeCannotBeSet {
            get {
                return ResourceManager.GetString("HarshContentType_BothIdAndParentContentTypeCannotBeSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified content type ID &apos;{0}&apos; cannot be appended to &apos;{1}&apos;, because it is already an absolute content type ID...
        /// </summary>
        internal static string HarshContentTypeId_CannotAppendAbsoluteCTId {
            get {
                return ResourceManager.GetString("HarshContentTypeId_CannotAppendAbsoluteCTId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified content type ID &apos;{0}&apos; is not absolute..
        /// </summary>
        internal static string HarshContentTypeId_CannotIsChildOfRelative {
            get {
                return ResourceManager.GetString("HarshContentTypeId_CannotIsChildOfRelative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified string &apos;{0}&apos; is not a valid content type ID. Expected a 32-character ID after the &quot;00&quot; separator..
        /// </summary>
        internal static string HarshContentTypeId_Expected_32chars_ID_after_00 {
            get {
                return ResourceManager.GetString("HarshContentTypeId_Expected_32chars_ID_after_00", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified string &apos;{0}&apos; contains characters illegal in a content type ID..
        /// </summary>
        internal static string HarshContentTypeId_IllegalCharacters {
            get {
                return ResourceManager.GetString("HarshContentTypeId_IllegalCharacters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified string &apos;{0}&apos; is not a valid content type ID, its length isn&apos;t an even number..
        /// </summary>
        internal static string HarshContentTypeId_NotEven {
            get {
                return ResourceManager.GetString("HarshContentTypeId_NotEven", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified string &quot;00&quot; is used to separate content type ID parts and therefore cannot be used as a valid relative content type ID..
        /// </summary>
        internal static string HarshContentTypeId_RelCTId_00_Invalid {
            get {
                return ResourceManager.GetString("HarshContentTypeId_RelCTId_00_Invalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified relative content type ID &apos;{0}&apos; is invalid. A relative content type ID is either a 2 or 32 character long hexadecimal string..
        /// </summary>
        internal static string HarshContentTypeId_RelCTId_Invalid {
            get {
                return ResourceManager.GetString("HarshContentTypeId_RelCTId_Invalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified edge from &quot;{0}&quot; to &quot;{1}&quot; is invalid..
        /// </summary>
        internal static string HarshDependencyGraph_EdgeFromToEqual {
            get {
                return ResourceManager.GetString("HarshDependencyGraph_EdgeFromToEqual", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This HarshEntityMetadata instance doesn&apos;t belong to a HarshEntityMetadataRepository..
        /// </summary>
        internal static string HarshEntityMetadata_DoesntBelongToARepo {
            get {
                return ResourceManager.GetString("HarshEntityMetadata_DoesntBelongToARepo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified type &apos;{0}&apos; is not a subclass of HarshEntity..
        /// </summary>
        internal static string HarshEntityMetadata_TypeNotAnEntity {
            get {
                return ResourceManager.GetString("HarshEntityMetadata_TypeNotAnEntity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified type &apos;{0}&apos; has no ContentTypeAttribute..
        /// </summary>
        internal static string HarshEntityMetadataContentType_NoContentTypeAttribute {
            get {
                return ResourceManager.GetString("HarshEntityMetadataContentType_NoContentTypeAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The field cannot be added because FieldSchemaXml is null..
        /// </summary>
        internal static string HarshFieldProvisioner_SchemaXmlNotSet {
            get {
                return ResourceManager.GetString("HarshFieldProvisioner_SchemaXmlNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HarshFieldSchemaXmlTransformer.Transform cannot be null..
        /// </summary>
        internal static string HarshFieldProvisioner_TransformNull {
            get {
                return ResourceManager.GetString("HarshFieldProvisioner_TransformNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The FieldId cannot be an empty Guid..
        /// </summary>
        internal static string HarshFieldProvisionerBase_FieldIdEmpty {
            get {
                return ResourceManager.GetString("HarshFieldProvisionerBase_FieldIdEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} cannot be an empty Guid..
        /// </summary>
        internal static string HarshFieldSchemaXmlProvisioner_PropertyEmptyGuid {
            get {
                return ResourceManager.GetString("HarshFieldSchemaXmlProvisioner_PropertyEmptyGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} cannot be null or a whitespace-only string..
        /// </summary>
        internal static string HarshFieldSchemaXmlProvisioner_PropertyWhiteSpace {
            get {
                return ResourceManager.GetString("HarshFieldSchemaXmlProvisioner_PropertyWhiteSpace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation is valid only during a Provision/Unprovision call..
        /// </summary>
        internal static string HarshProvisionerBase_NoContext {
            get {
                return ResourceManager.GetString("HarshProvisionerBase_NoContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The mandatory parameter &apos;{0}&apos; must be set..
        /// </summary>
        internal static string HarshProvisionerBase_ParameterMandatory {
            get {
                return ResourceManager.GetString("HarshProvisionerBase_ParameterMandatory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Default parameter set &apos;{0}&apos; could not be found on type &apos;{1}&apos;..
        /// </summary>
        internal static string HarshProvisionerMetadata_DefaultParameterSetNotFound {
            get {
                return ResourceManager.GetString("HarshProvisionerMetadata_DefaultParameterSetNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified property &apos;{0} {1}.{2}&apos; is not a valid parameter. Parameters must be read-write, public, instance, non-abstract, non-indexer properties..
        /// </summary>
        internal static string HarshProvisionerMetadata_InvalidParameterProperty {
            get {
                return ResourceManager.GetString("HarshProvisionerMetadata_InvalidParameterProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The mandatory parameter &apos;{0}.{1}&apos; is of type &apos;{2}&apos;, which is not supported by the DefaultValuePolicy &apos;{3}&apos;..
        /// </summary>
        internal static string HarshProvisionerMetadata_MandatoryTypeNotSupportedByDefaultValuePolicy {
            get {
                return ResourceManager.GetString("HarshProvisionerMetadata_MandatoryTypeNotSupportedByDefaultValuePolicy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameters cannot be members of the same parameter set multiple times.
        ///
        ///Property: {0}.{1}
        ///Duplicate parameter sets: {2}.
        /// </summary>
        internal static string HarshProvisionerMetadata_MoreParametersWithSameSet {
            get {
                return ResourceManager.GetString("HarshProvisionerMetadata_MoreParametersWithSameSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified property &apos;{0}.{1}&apos; is of type IResolve or IResolveSingle and cannot have a TagType set..
        /// </summary>
        internal static string HarshProvisionerMetadata_NoTagTypesOnResolvers {
            get {
                return ResourceManager.GetString("HarshProvisionerMetadata_NoTagTypesOnResolvers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DefaultFromContext properties must be reference types. The specified property &apos;{0}.{1}&apos; is of type &apos;{2}&apos;. .
        /// </summary>
        internal static string HarshProvisionerMetadata_NoValueTypeDefaultFromContext {
            get {
                return ResourceManager.GetString("HarshProvisionerMetadata_NoValueTypeDefaultFromContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameters can either be shared by all parameter sets by having ParameterSetName equal to null, or belong to one or more named parameter sets.
        ///
        ///Property: {0}.{1}.
        /// </summary>
        internal static string HarshProvisionerMetadata_ParameterBothCommonAndInSet {
            get {
                return ResourceManager.GetString("HarshProvisionerMetadata_ParameterBothCommonAndInSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified property &apos;{1}.{2}&apos; has DefaultFromContext TagType of &apos;{0}&apos; that does not implement the IDefaultFromContextTag interface..
        /// </summary>
        internal static string HarshProvisionerMetadata_TagTypeNotAssignableFromIDefaultFromContextTag {
            get {
                return ResourceManager.GetString("HarshProvisionerMetadata_TagTypeNotAssignableFromIDefaultFromContextTag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified item array length {0} doesn&apos;t match the arity of &apos;{1}&apos;..
        /// </summary>
        internal static string HarshTuple_CreateTupleItemsCountDoesNotMatchType {
            get {
                return ResourceManager.GetString("HarshTuple_CreateTupleItemsCountDoesNotMatchType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create a tuple with more than {0} components..
        /// </summary>
        internal static string HarshTuple_TooManyComponentTypes {
            get {
                return ResourceManager.GetString("HarshTuple_TooManyComponentTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified type &apos;{0}&apos; is not a constructed generic type..
        /// </summary>
        internal static string HarshTuple_TypeIsNotAConstructedGenericType {
            get {
                return ResourceManager.GetString("HarshTuple_TypeIsNotAConstructedGenericType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified type &apos;{0}&apos; is not a Tuple..
        /// </summary>
        internal static string HarshTuple_TypeIsNotATuple {
            get {
                return ResourceManager.GetString("HarshTuple_TypeIsNotATuple", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified url &apos;{0}&apos; is not relative to &apos;{1}&apos;..
        /// </summary>
        internal static string HarshUrl_UrlNotRelativeTo {
            get {
                return ResourceManager.GetString("HarshUrl_UrlNotRelativeTo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Last element of the sequence cannot be null..
        /// </summary>
        internal static string NestedResoveResult_LastComponentCannotBeNull {
            get {
                return ResourceManager.GetString("NestedResoveResult_LastComponentCannotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The last specified component type &apos;{0}&apos; must be assignable from &apos;{1}&apos;..
        /// </summary>
        internal static string NestedResoveResult_LastComponentTypeMustBeAssignableFromT {
            get {
                return ResourceManager.GetString("NestedResoveResult_LastComponentTypeMustBeAssignableFromT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter set cannot be resolved using the specified parameters..
        /// </summary>
        internal static string ParameterSetResolver_Ambiguous {
            get {
                return ResourceManager.GetString("ParameterSetResolver_Ambiguous", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} resolved in more than one result..
        /// </summary>
        internal static string Resolvable_ManyResults {
            get {
                return ResourceManager.GetString("Resolvable_ManyResults", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} resolved in no results..
        /// </summary>
        internal static string Resolvable_NoResult {
            get {
                return ResourceManager.GetString("Resolvable_NoResult", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The resolver &apos;{0}&apos; must be resolved via the IResolver interface to retrieve results..
        /// </summary>
        internal static string Resolvable_ResultsNotAvailableNotResolved {
            get {
                return ResourceManager.GetString("Resolvable_ResultsNotAvailableNotResolved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This method cannot be called on ResolveBuilders..
        /// </summary>
        internal static string ResolveBuilder_CannotCallThisMethod {
            get {
                return ResourceManager.GetString("ResolveBuilder_CannotCallThisMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resolve context factory returned null..
        /// </summary>
        internal static string ResolveRegistrar_ContextFactoryReturnedNull {
            get {
                return ResourceManager.GetString("ResolveRegistrar_ContextFactoryReturnedNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified type &apos;{0}&apos; is not an IGrouping..
        /// </summary>
        internal static string ResolveResultConverterStrategyGrouping_NotAGrouping {
            get {
                return ResourceManager.GetString("ResolveResultConverterStrategyGrouping_NotAGrouping", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tuples of IGroupings are not supported: {0}..
        /// </summary>
        internal static string ResolveResultConverterStrategyTuple_NestedGroupingNotAllowed {
            get {
                return ResourceManager.GetString("ResolveResultConverterStrategyTuple_NestedGroupingNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified object &apos;{0}&apos; cannot be converted to &apos;{1}&apos;..
        /// </summary>
        internal static string ResolveResultFactory_ObjectNotConvertable {
            get {
                return ResourceManager.GetString("ResolveResultFactory_ObjectNotConvertable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified property type &apos;{0}&apos; is not a generic type..
        /// </summary>
        internal static string ResolveResultFactory_PropertyTypeNotGeneric {
            get {
                return ResourceManager.GetString("ResolveResultFactory_PropertyTypeNotGeneric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified property type generic definition &apos;{0}&apos; is not one of the known interface types: {1}..
        /// </summary>
        internal static string ResolveResultFactory_PropertyTypeUnknownInterface {
            get {
                return ResourceManager.GetString("ResolveResultFactory_PropertyTypeUnknownInterface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified type &apos;{0}&apos; is not a generic interface definition..
        /// </summary>
        internal static string TypeInfoExtension_ArgNotInterfaceDefinition {
            get {
                return ResourceManager.GetString("TypeInfoExtension_ArgNotInterfaceDefinition", resourceCulture);
            }
        }
    }
}
