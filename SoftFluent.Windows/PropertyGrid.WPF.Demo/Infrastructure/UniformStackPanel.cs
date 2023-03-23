using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Panels
{
    /// <summary>
    /// Helps boxing Booolean values.
    /// https://xstatic2.wordpress.com/2011/10/21/tip-improving-boolean-dependency-properties-performance/
    /// </summary>
    public static class BooleanBoxes
    {
        /// <summary>
        /// Gets a boxed representation for Boolean's "true" value.
        /// </summary>
        public static readonly object TrueBox;

        /// <summary>
        /// Gets a boxed representation for Boolean's "false" value.
        /// </summary>
        public static readonly object FalseBox;

        /// <summary>
        /// Initializes the <see cref="BooleanBoxes"/> class.
        /// </summary>
        static BooleanBoxes()
        {
            TrueBox = true;
            FalseBox = false;
        }

        /// <summary>
        /// Returns a boxed representation for the specified Boolean value.
        /// </summary>
        /// <param name="value">The value to box.</param>
        /// <returns></returns>
        public static object Box(bool value)
        {
            if (value)
            {
                return TrueBox;
            }

            return FalseBox;
        }
    }


    /// <summary>
    /// 
    /// UniformStackPanel
    ///   Filed under: WPF — Leave a comment
    ///   September 30, 2012

    ///  For a personal application I’m writing, I needed a panel that stacks my items, but unlike the regular StackPanel, which doesn’t limit the size of the items, I need mine to divide its items evenly among its available space.
    ///   Also, I needed some of the items to be able to receive their desired size and not the size like the rest of the items, so I can use an Expander control, make them collapsed and make them using less space.
    ///  This panel is usually useful when creating navigation buttons like Outlook has(the buttons to switch between Mail, Calendar, Notes and so on).
    ///   For example, if this UniformStackPanel has 300 units to display 3 items, it will give each one 100 units.
    ///  If I set the second one as UniformStackPanel.IsAutoSized=”True” and it only needs 20 units, then the first and third items will have a size of 140 units, and the second item will have a size of 20 units.
    ///  Notice that “size” could be either height or width, depends on the orientation of the panel.
    /// Also, notice that I’m using the Boolean boxes from my previous post: https://xstatic2.wordpress.com/2011/10/21/tip-improving-boolean-dependency-properties-performance/.
    /// https://xstatic2.wordpress.com/2012/09/30/uniformstackpanel/
    /// </summary>
    public class UniformStackPanel : Panel
    {
        public static bool GetIsAutoSized(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(IsAutoSizedProperty);
        }

        public static void SetIsAutoSized(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(IsAutoSizedProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty IsAutoSizedProperty =
            DependencyProperty.RegisterAttached("IsAutoSized", typeof(bool), typeof(UniformStackPanel), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformStackPanel), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        protected override Size MeasureOverride(Size availableSize)
        {
            _autoSizeSum = 0d;
            double maxElementWidth = 0d;
            int count = InternalChildren.Count;
            Orientation orientation = Orientation;
            List<UIElement> fixedSizedElements = new List<UIElement>(InternalChildren.Count);

            for (int i = 0; i < count; i++)
            {
                UIElement element = InternalChildren[i];

                if (element.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                bool isElementAutoSized = GetIsAutoSized(element);

                if (isElementAutoSized)
                {
                    element.Measure(availableSize);
                    double autoSizedValue = element.DesiredSize.Height;

                    if (orientation == Orientation.Horizontal)
                    {
                        autoSizedValue = element.DesiredSize.Width;
                    }

                    _autoSizeSum += autoSizedValue;
                }
                else
                {
                    fixedSizedElements.Add(element);
                }

                double elementWidth = element.DesiredSize.Width;

                if (orientation == Orientation.Horizontal)
                {
                    elementWidth = element.DesiredSize.Height;
                }

                maxElementWidth = Math.Max(maxElementWidth, elementWidth);
            }

            _fixedSizedElementsCount = fixedSizedElements.Count;

            double fixedSizeAvailableSpace = availableSize.Height - _autoSizeSum;

            if (orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                fixedSizeAvailableSpace = availableSize.Width - _autoSizeSum;
            }

            double singleFixedSizeElementHeight = fixedSizeAvailableSpace / (double)_fixedSizedElementsCount;

            Size singleFixedSizeElementSize = Size.Empty;

            if (orientation == Orientation.Vertical)
            {
                singleFixedSizeElementSize = new Size(availableSize.Width, singleFixedSizeElementHeight);
            }
            else
            {
                singleFixedSizeElementSize = new Size(singleFixedSizeElementHeight, availableSize.Height);
            }

            for (int i = 0; i < fixedSizedElements.Count; i++)
            {
                UIElement element = fixedSizedElements[i];
                element.Measure(singleFixedSizeElementSize);
            }

            return new Size(maxElementWidth, singleFixedSizeElementHeight + _autoSizeSum);
        }

        private double _autoSizeSum;
        private int _fixedSizedElementsCount;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0d;
            double y = 0d;
            Orientation orientation = Orientation;

            Size availableSize = finalSize;

            if (orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                availableSize = new Size(finalSize.Height, finalSize.Width);
            }

            double fixedSizeAvailableSpace = Math.Max(0d, availableSize.Height - _autoSizeSum);

            foreach (UIElement element in InternalChildren)
            {
                Size elementSize = element.DesiredSize;

                bool isElementAutoSized = GetIsAutoSized(element);

                if (!isElementAutoSized)
                {
                    if (orientation == Orientation.Vertical)
                    {
                        elementSize = new Size(elementSize.Width, fixedSizeAvailableSpace / (double)_fixedSizedElementsCount);
                    }
                    else
                    {
                        elementSize = new Size(fixedSizeAvailableSpace / (double)_fixedSizedElementsCount, elementSize.Height);
                    }
                }

                Rect finalRect = new Rect(x, y, Math.Max(0d, finalSize.Width - x), Math.Max(0d, finalSize.Height - y));

                if (element.Visibility != Visibility.Collapsed)
                {
                    if (orientation == System.Windows.Controls.Orientation.Vertical)
                    {
                        y += elementSize.Height;
                        finalRect.Height = elementSize.Height;
                    }
                    else
                    {
                        x += elementSize.Width;
                        finalRect.Width = elementSize.Width;
                    }
                }

                element.Arrange(finalRect);
            }

            return finalSize;
        }
    }
}