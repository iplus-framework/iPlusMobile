using System;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
{
	/// <summary>
	/// The Nested listbox.
	/// </summary>
	public class NestedListView : ListView
	{
        public static readonly BindableProperty IsNestedScrollProperty = BindableProperty.Create(
            propertyName: "IsNestedScroll",
            returnType: typeof(bool),
            declaringType: typeof(NestedListView),
            defaultValue: false
            );

        public bool IsNestedScroll
        {
            get
            {
                return (bool)GetValue(IsNestedScrollProperty);
            }
            set
            {
                SetValue(IsNestedScrollProperty, value);
            }
        }
    }
}