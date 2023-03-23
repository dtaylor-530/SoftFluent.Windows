using SoftFluent.Windows.Utilities;
using System;
//using PropertyGrid.Infrastructure;
using PropertyGrid.WPF.Demo.Infrastructure;
using System.Threading.Tasks;

namespace SoftFluent.Windows
{
    public class BaseActivator : IActivator
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
    }
}
