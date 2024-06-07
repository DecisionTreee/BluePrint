using System.Windows;
using System.Windows.Media;

namespace BluePrint.UI
{
    internal static class Extension
    {
        internal static T? FindParent<T>(this UIElement element) where T : UIElement
        {
            if (element is T value)
            {
                return value;
            }
            else
            {
                DependencyObject? parent = VisualTreeHelper.GetParent(element);
                if (parent is UIElement last)
                {
                    return last.FindParent<T>();
                }
            }

            return default;
        }
    }
}
