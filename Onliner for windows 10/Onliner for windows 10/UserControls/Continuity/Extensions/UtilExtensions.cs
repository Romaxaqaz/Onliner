﻿using Continuity;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Continuity.Extensions
{
    public static partial class UtilExtensions
    {
        public static float ToFloat(this double value)
        {
            return (float)value;
        }

        public static List<FrameworkElement> Children(this DependencyObject parent)
        {
            var list = new List<FrameworkElement>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is FrameworkElement)
                {
                    list.Add(child as FrameworkElement);
                }

                list.AddRange(Children(child));
            }

            return list;
        }

        public static T GetChildByName<T>(this DependencyObject parent, string name)
        {
            var childControls = Children(parent);
            var control = childControls.OfType<FrameworkElement>().Where(x => x.Name.Equals(name)).Cast<T>().First();

            return control;
        }

        public static void ScrollToElement(this ScrollViewer scrollViewer, UIElement element,
            bool isVerticalScrolling = true, bool smoothScrolling = true, float? zoomFactor = null)
        {
            var transform = element.TransformToVisual((UIElement)scrollViewer.Content);
            var position = transform.TransformPoint(new Point(0, 0));

            if (isVerticalScrolling)
            {
                scrollViewer.ChangeView(null, position.Y, zoomFactor, !smoothScrolling);
            }
            else
            {
                scrollViewer.ChangeView(position.X, null, zoomFactor, !smoothScrolling);
            }
        }

        //public static void Seek(this ScalarKeyFrameAnimation animation, float from, float to, double duration)
        //{
        //    CompositionScopedBatch batch = null;
        //    batch.
        //    batch = animation.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        //    batch.Completed += (s, e) => completed();

        //    visual.StartAnimation($"Offset.{orientation.ToString()}", compositor.CreateScalarKeyFrameAnimation(to, duration, easing));

        //    batch.End();
        //}

        public static CompositionPropertySet ScrollProperties(this ScrollViewer scrollViewer)
        {
            return ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
        }

        public static Visual Visual(this UIElement element)
        {
            return ElementCompositionPreview.GetElementVisual(element);
        }

        public static ContainerVisual ContainerVisual(this UIElement element)
        {
            var hostVisual = ElementCompositionPreview.GetElementVisual(element);
            ContainerVisual root = hostVisual.Compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(element, root);
            return root;
        }

        public static FlickDirection FlickDirection(this ManipulationCompletedRoutedEventArgs e)
        {
            if (!e.IsInertial)
            {
                return global::Continuity.FlickDirection.None;
            }

            var x = e.Cumulative.Translation.X;
            var y = e.Cumulative.Translation.Y;

            if (Math.Abs(x) > Math.Abs(y))
            {
                return x > 0 ? global::Continuity.FlickDirection.Right : global::Continuity.FlickDirection.Left;
            }
            else
            {
                return y > 0 ? global::Continuity.FlickDirection.Down : global::Continuity.FlickDirection.Up;
            }
        }

        public static void FillAnimation(this ManipulationCompletedRoutedEventArgs e, double fullDimension,
            Action forward, Action backward,
            AnimationAxis orientation = AnimationAxis.Y, double ratio = 0.5)
        {
            var translation = e.Cumulative.Translation;
            var distance = orientation == AnimationAxis.X ? translation.X : translation.Y;

            if (distance >= fullDimension * ratio)
            {
                forward();
            }
            else
            {
                backward();
            }
        }

        public static float MovementRatioOnYAxis(this Point translation, double height)
        {
            var ratio = translation.Y / height;
            ratio = ratio < 0 ? 0 : ratio;

            return ratio.ToFloat();
        }

        public static Point RelativePosition(this UIElement element, UIElement other)
        {
            return element.TransformToVisual(other).TransformPoint(new Point(0, 0));
        }

        public static float OffsetX(this UIElement element, UIElement other)
        {
            var position = element.RelativePosition(other);
            return position.X.ToFloat();
        }
    }
}
