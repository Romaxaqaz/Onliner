using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onliner.Helpers
{
    public static class WebViewHelper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
        "Html", typeof(string), typeof(WebViewHelper), new PropertyMetadata("", OnHtmlStringChanged));

        public static string GetHtml(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(HtmlProperty, value);
        }

        private static void OnHtmlStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebView;

            if (browser == null)
                return;

            var html = e.NewValue.ToString();

            browser.NavigateToString(html);
        }
    }
}
