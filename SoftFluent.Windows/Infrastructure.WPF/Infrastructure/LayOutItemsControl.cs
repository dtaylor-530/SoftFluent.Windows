using PixelLab.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static BOT.WPF.TemplateGenerator;

namespace PixelLab.Wpf
{
    public enum Arrangement
    {
        TreeStack, Uniform, Stack, Wrap, TreeMap
    }

    public class LayOutItemsControl : ItemsControl
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation?), typeof(LayOutItemsControl), new PropertyMetadata(OrientationChanged));

        public static readonly DependencyProperty ArrangementProperty =
            DependencyProperty.Register("Arrangement", typeof(object), typeof(LayOutItemsControl), new PropertyMetadata(ArrangementChanged));

        public Orientation? Orientation
        {
            get => (Orientation?)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public object Arrangement
        {
            get { return (object)GetValue(ArrangementProperty); }
            set { SetValue(ArrangementProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            var arrangement = (Arrangement)Enum.Parse(typeof(Arrangement), Arrangement.ToString());
            LayOutHelper.Changed(this, Orientation, arrangement);
            base.OnApplyTemplate();
        }

        public static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl)
                if (e.NewValue is Orientation orientation)
                {
                    var arrangement = (Arrangement)d.GetValue(ArrangementProperty);
                    LayOutHelper.Changed(itemsControl, orientation, arrangement);
                }
        }

        public static void ArrangementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LayOutItemsControl itemsControl)
            {
                if (Enum.TryParse(e.NewValue.ToString(), out Arrangement arr))
                {
                    var orientation = itemsControl.Orientation;
                    LayOutHelper.Changed(itemsControl, orientation, arr);
                }
                if (e.NewValue is Arrangement arrangement)
                {
                    var orientation = itemsControl.Orientation;
                    LayOutHelper.Changed(itemsControl, orientation, arrangement);
                }
            }
        }
    }


    public class LayOutHelper
    {
        public static void Changed(ItemsControl itemsControl, Orientation? orientation, Arrangement arrangement)
        {
            itemsControl.ItemsPanel = ItemsPanelTemplate();

            ItemsPanelTemplate ItemsPanelTemplate()
            {
                return (arrangement, orientation) switch
                {
                    (Arrangement.Stack, _) => BasicItemsPanelTemplate(),
                    (Arrangement.Wrap, _) => BasicItemsPanelTemplate(),
                    (Arrangement.Uniform, _) => BasicItemsPanelTemplate(),
                    //(Arrangement.TreeMap, _) => CreateItemsPanelTemplate<TreeMapPanel>(f => { }),
                    //(Arrangement.TreeStack, _) => CreateItemsPanelTemplate<TreeStackPanel>(f => { }),
                    _ => throw new Exception("WGE vgfd vvf")
                };
            }

            ItemsPanelTemplate BasicItemsPanelTemplate()
            {
                return LayOutHelper_.ItemsPanelTemplate(itemsControl.Items.Count, orientation, arrangement);
            }
        }
    }

    public class LayOutHelper_
    {
        public static void Changed(ItemsControl itemsControl, Orientation orientation, Arrangement arrangement)
        {
            itemsControl.ItemsPanel = ItemsPanelTemplate(itemsControl.Items.Count, orientation, arrangement);
        }

        public static ItemsPanelTemplate ItemsPanelTemplate(int count, Orientation? orientation, Arrangement arrangement)
        {
            return (arrangement, orientation) switch
            {
                (Arrangement.Stack, _) =>
                CreateItemsPanelTemplate<StackPanel>(SetStackPanelOrientation),
                (Arrangement.Wrap, _) =>
                CreateItemsPanelTemplate<WrapPanel>(SetWrapPanelOrientation),
                (Arrangement.Uniform, Orientation.Vertical) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.ColumnsProperty, 1);
                    factory.SetValue(UniformGrid.RowsProperty, count);
                }),
                (Arrangement.Uniform, Orientation.Horizontal) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.RowsProperty, 1);
                    factory.SetValue(UniformGrid.ColumnsProperty, count);
                }),
                (Arrangement.Uniform, _) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                }),


                _ => throw new Exception("WGE vgfd vvf")
            };

            void SetStackPanelOrientation(FrameworkElementFactory factory)
            {
                orientation ??= (Orientation)WrapPanel.OrientationProperty.DefaultMetadata.DefaultValue;
                factory.SetValue(StackPanel.OrientationProperty, orientation);
            }
            void SetWrapPanelOrientation(FrameworkElementFactory factory)
            {
                orientation ??= (Orientation)WrapPanel.OrientationProperty.DefaultMetadata.DefaultValue;
                factory.SetValue(WrapPanel.OrientationProperty, orientation);
            }
            void SetTreeMapOrientation(FrameworkElementFactory factory)
            {
                factory.SetValue(WrapPanel.OrientationProperty, orientation);
            }
            void SetTreeStackOrientation(FrameworkElementFactory factory)
            {
                factory.SetValue(WrapPanel.OrientationProperty, orientation);
            }
        }
    }
}
