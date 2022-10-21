using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Controls
{
	/// <summary>
	/// The check box.
	/// </summary>
	/*public class CheckBox : View
	{
        /// <summary>
        /// The checked state property.
        /// </summary>
        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create("IsChecked", typeof(bool),
            typeof(CheckBox), false, BindingMode.TwoWay, null, OnCheckedPropertyChanged);

        /// <summary>
        /// The checked text property.
        /// </summary>
        public static readonly BindableProperty CheckedTextProperty = BindableProperty.Create("CheckedText", typeof(string), typeof(CheckBox), string.Empty, BindingMode.Default);

        /// <summary>
        /// The unchecked text property.
        /// </summary>
        public static readonly BindableProperty UncheckedTextProperty = BindableProperty.Create("UncheckedText", typeof(string), typeof(CheckBox), string.Empty, BindingMode.Default);

        /// <summary>
        /// The default text property.
        /// </summary>
        public static readonly BindableProperty DefaultTextProperty = BindableProperty.Create("Text", typeof(string), typeof(CheckBox), string.Empty, BindingMode.Default);

        /// <summary>
        /// Identifies the TextColor bindable property.
        /// </summary>
        /// 
        /// <remarks/>
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(CheckBox), Color.FromRgba(-1,-1,-1,-1), BindingMode.Default);

        /// <summary>
        /// The font size property
        /// </summary>
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(CheckBox), -1.0, BindingMode.Default);

        /// <summary>
        /// The font name property.
        /// </summary>
        public static readonly BindableProperty FontNameProperty = BindableProperty.Create("FontName", typeof(string), typeof(CheckBox), string.Empty, BindingMode.Default);


		/// <summary>
		/// The checked changed event.
		/// </summary>
		public event EventHandler<EventArgs<bool>> CheckedChanged;

		/// <summary>
		/// Gets or sets a value indicating whether the control is checked.
		/// </summary>
		/// <value>The checked state.</value>
		public bool IsChecked
        {
			get
			{
				return this.GetValue<bool>(IsCheckedProperty);
			}

			set
			{
				if (this.IsChecked != value)
                {
					this.SetValue(IsCheckedProperty, value);
                    if (this.CheckedChanged != null)
					    this.CheckedChanged.Invoke(this, new EventArgs<bool>(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating the checked text.
		/// </summary>
		/// <value>The checked state.</value>
		/// <remarks>
		/// Overwrites the default text property if set when checkbox is checked.
		/// </remarks>
		public string CheckedText
		{
			get
			{
				return this.GetValue<string>(CheckedTextProperty);
			}

			set
			{
				this.SetValue(CheckedTextProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control is checked.
		/// </summary>
		/// <value>The checked state.</value>
		/// <remarks>
		/// Overwrites the default text property if set when checkbox is checked.
		/// </remarks>
		public string UncheckedText
		{
			get
			{
				return this.GetValue<string>(UncheckedTextProperty);
			}

			set
			{
				this.SetValue(UncheckedTextProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		public string DefaultText
		{
			get
			{
				return this.GetValue<string>(DefaultTextProperty);
			}

			set
			{
				this.SetValue(DefaultTextProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <value>The color of the text.</value>
		public Color TextColor
		{
			get
			{
				return this.GetValue<Color>(TextColorProperty);
			}

			set
			{
				this.SetValue(TextColorProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the size of the font.
		/// </summary>
		/// <value>The size of the font.</value>
		public double FontSize
		{
			get
			{
				return (double) GetValue(FontSizeProperty);
			}
			set
			{
				SetValue(FontSizeProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the name of the font.
		/// </summary>
		/// <value>The name of the font.</value>
		public string FontName
		{
			get
			{
				return (string) GetValue(FontNameProperty);
			}
			set
			{
				SetValue(FontNameProperty, value);
			}
		}
		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text
		{
			get
			{
				return this.IsChecked
					? (string.IsNullOrEmpty(this.CheckedText) ? this.DefaultText : this.CheckedText)
						: (string.IsNullOrEmpty(this.UncheckedText) ? this.DefaultText : this.UncheckedText);
			}
		}

		/// <summary>
		/// Called when [checked property changed].
		/// </summary>
		/// <param name="bindable">The bindable.</param>
		/// <param name="oldvalue">if set to <c>true</c> [oldvalue].</param>
		/// <param name="newvalue">if set to <c>true</c> [newvalue].</param>
		private static void OnCheckedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			var checkBox = (CheckBox) bindable;
			checkBox.IsChecked = (bool) newvalue;
		}
	}*/
}