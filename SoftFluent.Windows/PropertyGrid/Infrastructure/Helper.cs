//using SoftFluent.Windows.Utilities;
//using SoftFluent.Windows;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PropertyGrid.Infrastructure
//{
//    public static class Helper
//    {
//        public static void AddDynamicProperties(this DynamicObject dynamicObject, ICollection<PropertyGridAttribute> attributes)
//        {
//            if (attributes == null || dynamicObject == null)
//            {
//                return;
//            }

//            foreach (PropertyGridAttribute pga in attributes)
//            {
//                if (string.IsNullOrWhiteSpace(pga.Name))
//                {
//                    continue;
//                }

//                DynamicObjectProperty prop = dynamicObject.AddProperty(pga.Name, pga.Type, null);
//                prop.SetValue(dynamicObject, pga.Value);
//            }
//        }
//    }
//}
