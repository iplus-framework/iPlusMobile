using System;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
{
	/// <summary>
	/// The check box.
	/// </summary>
	public class NumericEntry : Entry
    {
        public NumericEntry()
        {
            TextChanged += NumericEntry_TextChanged;
            Completed += NumericEntry_Completed;
        }

        public static readonly BindableProperty TransferValueAtEndProperty = BindableProperty.Create("TransferValueAtEnd", typeof(bool), typeof(NumericEntry), true, BindingMode.OneWay);

		public bool TransferValueAtEnd
        {
			get
			{
				return this.GetValue<bool>(TransferValueAtEndProperty);
			}

			set
			{
                this.SetValue(TransferValueAtEndProperty, value);
            }
        }


        public static readonly BindableProperty TextNumericProperty = BindableProperty.Create("TextNumeric", typeof(string), typeof(Entry), null, BindingMode.TwoWay, propertyChanged: OnTextNumericChanged);

        public string TextNumeric
        {
            get { return (string)GetValue(TextNumericProperty); }
            set { SetValue(TextNumericProperty, value); }
        }

        static void OnTextNumericChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var entry = (NumericEntry)bindable;
            entry.Text = newValue as String;
        }

        private void NumericEntry_Completed(object sender, EventArgs e)
        {
            TextNumeric = Text;
        }

        private void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!TransferValueAtEnd)
                TextNumeric = Text;
        }
    }
}