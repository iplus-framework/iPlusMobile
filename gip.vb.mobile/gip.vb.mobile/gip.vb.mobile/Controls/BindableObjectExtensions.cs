using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
{
    public static class BindableObjectExtensions
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindableObject">The bindable object.</param>
        /// <param name="property">The property.</param>
        /// <returns>T.</returns>
        public static T GetValue<T>(this BindableObject bindableObject, BindableProperty property)
        {
            return (T)bindableObject.GetValue(property);
        }
    }
}
