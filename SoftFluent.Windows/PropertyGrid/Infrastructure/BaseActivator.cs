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

            //PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();


            var property = (Property)await CreateInstance(parent, descriptor.Name, typeof(Property));
            property.Descriptor = descriptor;

            //property.Options = options;
            property.Data = data;
            //Describe(property, descriptor, gridOptions.DefaultCategoryName, gridOptions.DecamelizePropertiesDisplayNames);
            return property;
        }



        public  async Task<IProperty> CreateProperty2(Guid parent, string name, object data)
        {


            //PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();


            var property = (Property2)await CreateInstance(parent, name, typeof(Property2));

            property.Name = name;
            //property.Options = options;
            property.Data = data;
            //Describe(property, descriptor, gridOptions.DefaultCategoryName, gridOptions.DecamelizePropertiesDisplayNames);
            return property;
        }



    }
}
