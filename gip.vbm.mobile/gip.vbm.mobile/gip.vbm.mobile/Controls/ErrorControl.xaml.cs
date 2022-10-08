using HandballReporter.shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace gip.vbm.mobile.Controls
{
    //[Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
    public partial class ErrorControl : ContentView
	{
		public ErrorControl ()
		{
			InitializeComponent();
            SetupPlacement();
		}


        #region DependencyProperties

        public static readonly BindableProperty ValidateEntityProperty = BindableProperty.Create("ValidateEntity", typeof(IValidateEntity), typeof(ErrorControl), null, BindingMode.Default, null, ValidateEntityChanged);
        public IValidateEntity ValidateEntity
        {
            get { return (IValidateEntity)GetValue(ValidateEntityProperty); }
            set { SetValue(ValidateEntityProperty, value); }
        }


        public static readonly BindableProperty InnerContentProperty = BindableProperty.Create("InnerContent", typeof(View), typeof(ErrorControl), null, BindingMode.Default, null, InnerContentChanged);
        public View InnerContent
        {
            get { return (View)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }


        public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create("IsReadOnly", typeof(bool), typeof(ErrorControl), false, BindingMode.Default, null, IsReadOnlyChanged);
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }


        public enum Placements { Left, Right, LeftAndValid, RightAndValid }
        public static readonly BindableProperty PlacementProperty = BindableProperty.Create("Placement", typeof(Placements), typeof(ErrorControl), Placements.Right, BindingMode.Default, null, PlacementChanged);
        public Placements Placement
        {
            get { return (Placements)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly BindableProperty PropertyNameProperty = BindableProperty.Create("PropertyName", typeof(string), typeof(ErrorControl), String.Empty, BindingMode.Default);
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        #endregion

        #region Methods

        private static void ValidateEntityChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var control = bindable as ErrorControl;
            var validateEntity = control.ValidateEntity as IValidateEntity;
            if (validateEntity == null)
                validateEntity = control.BindingContext as IValidateEntity;
            if (validateEntity == null)
                return;
            var dictionary = validateEntity.Properties;
            var property = dictionary[control.PropertyName];
            control.LeftButton.BindingContext = property;
            control.RightButton.BindingContext = property;
            control.LeftButtonValid.BindingContext = property;
            control.RightButtonValid.BindingContext = property;
        }


        private static void InnerContentChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            (bindable as ErrorControl).MyPresenter.Content = newvalue as View;
        }


        private static void IsReadOnlyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var c = bindable as ErrorControl;
            var u = c.MyPresenter.Content as View;
            u.IsEnabled = (bool)newvalue;
        }


        private static void PlacementChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var c = bindable as ErrorControl;
            c.SetupPlacement();
        }


        private void SetupPlacement()
        {
            switch (this.Placement)
            {
                case Placements.Left:
                    LeftButtonValid.IsVisible = false;
                    LeftButton.IsVisible = true;
                    RightButtonValid.IsVisible = false;
                    RightButton.IsVisible = false;
                    break;
                case Placements.LeftAndValid:
                    LeftButtonValid.IsVisible = true;
                    LeftButton.IsVisible = false;
                    RightButtonValid.IsVisible = false;
                    RightButton.IsVisible = false;
                    break;
                case Placements.Right:
                    LeftButtonValid.IsVisible = false;
                    LeftButton.IsVisible = false;
                    RightButtonValid.IsVisible = false;
                    RightButton.IsVisible = true;
                    break;
                case Placements.RightAndValid:
                    LeftButtonValid.IsVisible = false;
                    LeftButton.IsVisible = false;
                    RightButtonValid.IsVisible = true;
                    RightButton.IsVisible = false;
                    break;
            }
        }
        #endregion

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var validateEntity = ValidateEntity as IValidateEntity;
            if (validateEntity == null)
                validateEntity = BindingContext as IValidateEntity;
            if (validateEntity == null)
                return;
            var dictionary = validateEntity.Properties;
            var property = dictionary[PropertyName];
            if (property != null)
            {
                StringBuilder sb = new StringBuilder();
                if (property.Errors != null && property.Errors.Any())
                {
                    foreach (var s in property.Errors)
                        sb.AppendLine(s);
                    await App.Current.MainPage.DisplayAlert("", sb.ToString(), "OK");
                }
            }
        }
    }
}
