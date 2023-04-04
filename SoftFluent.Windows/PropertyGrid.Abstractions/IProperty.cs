using SoftFluent.Windows;
using System.Collections;
using System.ComponentModel;
using Utilities;

namespace Abstractions
{
    public interface IProperty // : IExecute
    {
        string Name { get; }
        string DisplayName { get; }

        //IPropertyGridOptionsAttribute Options { get; set; }
        //PropertyDescriptor Descriptor { get; set; }
        Type CollectionItemPropertyType { get; }
        Type PropertyType { get; }
        bool IsCollectionItemValueType { get; }
        bool IsValueType { get; }
        bool IsReadOnly { get; }
        bool IsError { get; set; }
        bool IsValid { get; }
        bool IsFlagsEnum { get; }
        string? Category { get; }
        bool IsEnum => PropertyType.IsEnum;
        object Value { get; set; }
        //bool IsDefaultValue => DefaultValue == Value;

        //public string TemplateKey { get; }
        //public string EditorTemplateKey { get; }

        //object? DefaultValue => Extensions.GetAttribute<DefaultValueAttribute>((MemberDescriptor)Descriptor).Value;
        //bool HasDefaultValue => DefaultValue != default;
        //string DefaultEditorResourceKey { get; }

        public virtual bool IsCollection => PropertyType != null ? PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType) : false;

        //  public int SortOrder => Options.SortOrder != 0 ? Options.SortOrder : default;


    }

    public interface IPropertyGridOptionsAttribute
    {
        bool ForcePropertyChanged { get; set; }
        //string[] EnumNames { get; set; }
        //object[] EnumValues { get; set; }
        //string EnumSeparator { get; set; }
        //int EnumMaxPower { get; set; }
        //bool IsEnum { get; set; }
        //bool IsFlagsEnum { get; set; }
        object EditorDataTemplateResourceKey { get; set; }
        Type EditorType { get; set; }
        string EditorDataTemplateSelectorPropertyPath { get; set; }
        string EditorDataTemplatePropertyPath { get; set; }
        bool CollectionEditorHasOnlyOneColumn { get; set; }
        string EditorResourceKey { get; }
        bool HasDefaultValue { get; set; }
        object DefaultValue { get; set; }
        int SortOrder { get; set; }
    }
}
