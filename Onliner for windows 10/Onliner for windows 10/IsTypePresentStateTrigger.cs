using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace Onliner_for_windows_10
{
    public class IsTypePresentStateTrigger : StateTriggerBase
    {
        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <remarks>
        /// Example: <c>Windows.Phone.UI.Input.HardwareButtons</c>
        /// </remarks>
        public string TypeName
        {
            get { return (string)GetValue(TypeNameProperty); }
            set { SetValue(TypeNameProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TypeName"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty TypeNameProperty =
            DependencyProperty.Register("TypeName", typeof(string), typeof(IsTypePresentStateTrigger),
            new PropertyMetadata("", OnTypeNamePropertyChanged));

        private static void OnTypeNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (IsTypePresentStateTrigger)d;
            var val = (string)e.NewValue;
            bool isTypePresent = ApiInformation.IsTypePresent(val);
            obj.SetActive(!string.IsNullOrWhiteSpace(val) && isTypePresent);
        }
    }
}
