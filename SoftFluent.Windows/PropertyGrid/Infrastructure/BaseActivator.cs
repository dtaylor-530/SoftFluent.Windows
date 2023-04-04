using SoftFluent.Windows.Utilities;
using System;
//using PropertyGrid.Infrastructure;
using PropertyGrid.WPF.Demo.Infrastructure;
using System.Threading.Tasks;
using Abstractions;
using System.ComponentModel;

namespace SoftFluent.Windows
{
    public class BaseActivator 
    {



        public virtual async Task<object?> CreateInstance(Guid parent, string name, System.Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var cx = await PropertyStore2.Instance.GetGuidByParent(parent, name, type);

            var args = new object[]
            {
               cx
            };

            return Activator.CreateInstance(type, args);
        }



        public async Task<IProperty> CreateProperty(Guid parent, PropertyDescriptor descriptor, object data)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }
 
            var property = (Property)await CreateInstance(parent, descriptor.Name, typeof(Property));

            property.Descriptor = descriptor;
            property.Data = data;

            return property;
        }



        public  async Task<IProperty> CreateCollectionProperty(Guid parent, int index, object data)
        {
            var property = (CollectionProperty)await CreateInstance(parent, index.ToString(), typeof(CollectionProperty));

            property.Index = index;
            property.Data = data;

            return property;
        }



    }
}
