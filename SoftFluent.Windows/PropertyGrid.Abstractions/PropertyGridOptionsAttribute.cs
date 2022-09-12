using System;
using System.Windows;
using Abstractions;

namespace SoftFluent.Windows
{
    public class PropertyGridOptionsAttribute : Attribute, IPropertyGridOptionsAttribute {
        public PropertyGridOptionsAttribute()
        {
            EnumSeparator = ", ";
        }

        public string[] EnumNames { get; set; }
        public object[] EnumValues { get; set; }
        public bool IsEnum { get; set; }
        public bool IsFlagsEnum { get; set; }
        public int EnumMaxPower { get; set; }
        public bool CollectionEditorHasOnlyOneColumn { get; set; }
        public int SortOrder { get; set; }
        public string EditorDataTemplatePropertyPath { get; set; }
        public string EditorDataTemplateSelectorPropertyPath { get; set; }
        public Type EditorType { get; set; }
        public string EditorResourceKey { get; set; }
        public object EditorDataTemplateResourceKey { get; set; }
        public Type PropertyType { get; set; }
        public bool ForceReadWrite { get; set; }
        public bool HasDefaultValue { get; set; }
        public bool ForcePropertyChanged { get; set; }
        public object DefaultValue { get; set; }
        public string EnumSeparator { get; set; }
    }
}