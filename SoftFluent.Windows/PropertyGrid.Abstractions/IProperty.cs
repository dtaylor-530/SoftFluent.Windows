using SoftFluent.Windows;
using System.Collections;
using System.ComponentModel;
using Utilities;

namespace Abstractions
{
    public interface IProperty
    {
        string Name { get; }
    
        Type PropertyType { get; }
        bool IsCollectionItemValueType { get; }
        bool IsValueType { get; }
        bool IsReadOnly { get; }
        bool IsError { get; set; }
        bool IsValid { get; }
        bool IsFlagsEnum { get; }
        //string? Category { get; }
        bool IsEnum => PropertyType.IsEnum;
        object Value { get; set; }
        //bool IsDefaultValue => DefaultValue == Value;

        //public string TemplateKey { get; }
        //public string EditorTemplateKey { get; }

        //object? DefaultValue => Extensions.GetAttribute<DefaultValueAttribute>((MemberDescriptor)Descriptor).Value;
        //bool HasDefaultValue => DefaultValue != default;
        //string DefaultEditorResourceKey { get; }

        public virtual bool IsCollection => PropertyType != null ? PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType) : false;

        bool IsString { get; }

        //  public int SortOrder => Options.SortOrder != 0 ? Options.SortOrder : default;
    }
}
