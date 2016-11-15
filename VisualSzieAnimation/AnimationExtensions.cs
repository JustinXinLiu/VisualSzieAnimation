using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace VisualSzieAnimation
{
    public static class AnimationExtensions
    {
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
            var root = hostVisual.Compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(element, root);
            return root;
        }

        public static void SetChildVisual(this UIElement element, Visual childVisual)
        {
            ElementCompositionPreview.SetElementChildVisual(element, childVisual);
        }

        #region Composition

        public static void EnableImplicitAnimation(this UIElement element, VisualPropertyType typeToAnimate,
            double duration = 800, double delay = 0, CompositionEasingFunction easing = null)
        {
            var visual = element.Visual();
            var compositor = visual.Compositor;

            var animationCollection = compositor.CreateImplicitAnimationCollection();

            foreach (var type in EnumExtensions.GetValues<VisualPropertyType>())
            {
                if (!typeToAnimate.HasFlag(type)) continue;

                var animation = CreateAnimationByType(compositor, type, duration, delay, easing);

                if (animation != null)
                {
                    animationCollection[type.ToString()] = animation;
                }
            }

            visual.ImplicitAnimations = animationCollection;
        }

        public static void EnableImplicitAnimation(this Visual visual, VisualPropertyType typeToAnimate,
            double duration = 800, double delay = 0, CompositionEasingFunction easing = null)
        {
            var compositor = visual.Compositor;

            var animationCollection = compositor.CreateImplicitAnimationCollection();

            foreach (var type in EnumExtensions.GetValues<VisualPropertyType>())
            {
                if (!typeToAnimate.HasFlag(type)) continue;

                var animation = CreateAnimationByType(compositor, type, duration, delay, easing);

                if (animation != null)
                {
                    animationCollection[type.ToString()] = animation;
                }
            }

            visual.ImplicitAnimations = animationCollection;
        }

        private static KeyFrameAnimation CreateAnimationByType(Compositor compositor, VisualPropertyType type,
            double duration = 800, double delay = 0, CompositionEasingFunction easing = null)
        {
            KeyFrameAnimation animation;

            switch (type)
            {
                case VisualPropertyType.Offset:
                case VisualPropertyType.Scale:
                    animation = compositor.CreateVector3KeyFrameAnimation();
                    break;
                case VisualPropertyType.Size:
                    animation = compositor.CreateVector2KeyFrameAnimation();
                    break;
                case VisualPropertyType.Opacity:
                case VisualPropertyType.RotationAngleInDegrees:
                    animation = compositor.CreateScalarKeyFrameAnimation();
                    break;
                default:
                    return null;
            }

            animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", easing);
            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            animation.Target = type.ToString();

            return animation;
        }

        #region Cubic Easing

        public static CubicBezierEasingFunction EaseInCubic(this Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.55f, 0.055f), new Vector2(0.675f, 0.19f));
        }

        public static CubicBezierEasingFunction EaseOutCubic(this Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.215f, 0.61f), new Vector2(0.355f, 1.0f));
        }

        public static CubicBezierEasingFunction EaseInOutCubic(this Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.645f, 0.045f), new Vector2(0.355f, 1.0f));
        }

        #endregion

        #endregion
    }

    [Flags]
    public enum VisualPropertyType
    {
        None = 0,
        Opacity = 1 << 0,
        Offset = 1 << 1,
        Scale = 1 << 2,
        Size = 1 << 3,
        RotationAngleInDegrees = 1 << 4,
        All = ~0
    }
}
