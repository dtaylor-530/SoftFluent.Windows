using System.Collections;

namespace PropertyGrid.Abstractions
{
    public interface IPropertyNode
    {
        public IEnumerable Children { get; }
    }
}

