namespace PropertyGrid.Abstractions
{
    public interface IPropertyGridOptions
    {
        object Data { get; set; }
        int InheritanceLevel { get; set; }
        bool IsReadOnly { get; set; }
        string DefaultCategoryName { get; }
        bool DecamelizePropertiesDisplayNames { get; }
    }
}
