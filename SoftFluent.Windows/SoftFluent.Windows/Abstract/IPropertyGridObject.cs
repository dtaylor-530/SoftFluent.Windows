using System.Collections.Generic;

namespace SoftFluent.Windows
{
    public interface IPropertyGridObject
    {
        void FinalizeProperties(PropertyGridListSource listSource, IList<PropertyGridProperty> properties);
        bool TryShowEditor(PropertyGridProperty property, object editor, out bool? result);
        void EditorClosed(PropertyGridProperty property, object editor);
    }
}