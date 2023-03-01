using Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Markup.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Utilities;

namespace SoftFluent.Windows.Utilities
{
   public static class VisualTreeExtensions
    {



        public static IEnumerable<DependencyProperty> EnumerateMarkupDependencyProperties(object element)
        {
            if (element != null)
            {
                MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
                if (markupObject != null)
                {
                    foreach (MarkupProperty mp in markupObject.Properties)
                    {
                        if (mp.DependencyProperty != null)
                        {
                            yield return mp.DependencyProperty;
                        }
                    }
                }
            }
        }

        //public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject obj)
        //{
        //    return obj.EnumerateVisualChildren(true);
        //}

        public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject obj, bool recursive)
        {
            return obj.EnumerateVisualChildren(recursive, true);
        }

        public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject obj, bool recursive, bool sameLevelFirst)
        {
            if (obj == null)
            {
                yield break;
            }

            if (sameLevelFirst)
            {
                int count = VisualTreeHelper.GetChildrenCount(obj);
                List<DependencyObject> list = new List<DependencyObject>(count);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child == null)
                    {
                        continue;
                    }

                    yield return child;
                    if (recursive)
                    {
                        list.Add(child);
                    }
                }

                foreach (DependencyObject child in list)
                {
                    foreach (DependencyObject grandChild in child.EnumerateVisualChildren(recursive, true))
                    {
                        yield return grandChild;
                    }
                }
            }
            else
            {
                int count = VisualTreeHelper.GetChildrenCount(obj);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child == null)
                    {
                        continue;
                    }

                    yield return child;
                    if (recursive)
                    {
                        foreach (DependencyObject dp in child.EnumerateVisualChildren(true, false))
                        {
                            yield return dp;
                        }
                    }
                }
            }
        }



    

        public static T FindFocusableVisualChild<T>(this DependencyObject obj, string name) where T : FrameworkElement
        {
            foreach (T item in obj.EnumerateVisualChildren(true, true).OfType<T>())
            {
                if (item.Focusable && (item.Name == name || name == null))
                {
                    return item;
                }
            }
            return null;
        }

        public static T FindVisualChild<T>(this DependencyObject obj, Func<T, bool> where) where T : FrameworkElement
        {
            if (where == null)
            {
                throw new ArgumentNullException("where");
            }

            foreach (T item in obj.EnumerateVisualChildren(true, true).OfType<T>())
            {
                if (where(item))
                {
                    return item;
                }
            }
            return null;
        }

        public static T FindVisualChild<T>(this DependencyObject obj, string name) where T : FrameworkElement
        {
            foreach (T item in obj.EnumerateVisualChildren(true, true).OfType<T>())
            {
                if (name == null)
                {
                    return item;
                }

                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public static IEnumerable<T> GetChildren<T>(this DependencyObject obj)
        {
            if (obj == null)
            {
                yield break;
            }

            foreach (object item in LogicalTreeHelper.GetChildren(obj))
            {
                if (item == null)
                {
                    continue;
                }

                if (item is T)
                {
                    yield return (T)item;
                }

                DependencyObject dep = item as DependencyObject;
                if (dep != null)
                {
                    foreach (T child in dep.GetChildren<T>())
                    {
                        yield return child;
                    }
                }
            }
        }
      
        public static T GetSelfOrParent<T>(this FrameworkElement source) where T : FrameworkElement
        {
            while (true)
            {
                if (source == null)
                {
                    return default(T);
                }

                if (source is T)
                {
                    return (T)source;
                }

                source = source.Parent as FrameworkElement;
            }
        }

        public static T GetVisualSelfOrParent<T>(this DependencyObject source) where T : DependencyObject
        {
            if (source == null)
            {
                return default(T);
            }

            if (source is T)
            {
                return (T)source;
            }

            if (!(source is Visual) && !(source is Visual3D))
            {
                return default(T);
            }

            return VisualTreeHelper.GetParent(source).GetVisualSelfOrParent<T>();
        }




        
    }
}