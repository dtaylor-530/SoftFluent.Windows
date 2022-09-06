using System.Collections.Generic;
using Abstractions;

namespace SoftFluent.Windows
{
    public interface IPropertyGridObject
    {
        void FinalizeProperties(IPropertyGridListSource listSource, IList<IPropertyGridProperty> properties);
        bool TryShowEditor(IPropertyGridProperty property, object editor, out bool? result);
        void EditorClosed(IPropertyGridProperty property, object editor);
    }

    public interface IPropertyGridListSource
    {
    }
}