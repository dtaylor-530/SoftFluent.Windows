using SoftFluent.Windows;
using System.Collections;
using System.ComponentModel;
using Utilities;

namespace Abstractions
{
    public interface IProperty // : IExecute
    {
        string Name => Descriptor.Name;
        string DisplayName => Descriptor.DisplayName;

        //IPropertyGridOptionsAttribute Options { get; set; }
        PropertyDescriptor Descriptor { get; set; }
        Type CollectionItemPropertyType { get; }
        Type PropertyType => Descriptor.PropertyType;
        bool IsCollectionItemValueType => CollectionItemPropertyType.IsValueType;
        bool IsValueType => PropertyType.IsValueType;
        bool IsReadOnly => Descriptor.IsReadOnly;
        bool IsError { get; set; }
        bool IsValid { get; }
        bool IsFlagsEnum => Extensions.IsFlagsEnum(PropertyType);
        string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Extensions.EqualsIgnoreCase(Descriptor.Category, CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;
        bool IsEnum => PropertyType.IsEnum;
        object Value { get; set; }
        //bool IsDefaultValue => DefaultValue == Value;

        public string TemplateKey { get; }
        public string EditorTemplateKey { get;  }

        object? DefaultValue => Extensions.GetAttribute<DefaultValueAttribute>((MemberDescriptor)Descriptor).Value;
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
