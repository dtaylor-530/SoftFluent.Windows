using Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyGrid.Abstractions
{
    public record Key(Guid Guid, string Name, Type Type) : IKey
    {
        public bool Equals(IKey? other)
        {
            return Equals(other as Key);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}
