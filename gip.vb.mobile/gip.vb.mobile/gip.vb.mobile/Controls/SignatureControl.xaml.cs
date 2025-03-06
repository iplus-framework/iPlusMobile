using gip.vb.mobile.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
{
    [Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
    public partial class SignatureControl : ContentView
	{
		public SignatureControl ()
		{
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
            }
		}

        #region DependencyProperties
        public static readonly BindableProperty ImageBytesProperty = BindableProperty.Create("ImageBytes", typeof(byte[]), typeof(SignatureControl), null, BindingMode.TwoWay, null, ImageBytesChanged);

        public byte[] ImageBytes
        {
            get { return (byte[])GetValue(ImageBytesProperty); }
            set { SetValue(ImageBytesProperty, value); }
        }

        public static readonly BindableProperty IsEditingProperty = BindableProperty.Create("IsEditingProperty", typeof(bool), typeof(SignatureControl), false, BindingMode.OneWayToSource);

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public event EventHandler EditingStarted;
        public event EventHandler EditingEnded;
        #endregion

        private void ClearButton_Click(object sender, EventArgs e)
        {
            IsEditing = true;
            if (EditingStarted != null)
                EditingStarted.Invoke(this, new EventArgs());
            Clear();
        }

        private async void OkButton_Clicked(object sender, EventArgs e)
        {
            _WritingBytes = true;
            try
            {
                ImageBytes = await SaveAsync();
            }
            catch
            {
            }
            _WritingBytes = false;
            IsEditing = false;
            if (EditingEnded != null)
                EditingEnded.Invoke(this, new EventArgs());
        }


        private bool _WritingBytes = false;

        public void Clear()
        {
            _WritingBytes = true;
            image.IsVisible = false;
            inkCanvas.IsVisible = true;
            inkCanvas.Clear();
            ImageBytes = null;
            _WritingBytes = false;
        }

        public async Task<byte[]> SaveAsync()
        {
            // We don't want to save an empty file
            if (!inkCanvas.IsBlank)
            {
                try
                {
                    Stream stream = await inkCanvas.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
                    MemoryStream memStream = stream as MemoryStream;
                    if (memStream != null)
                    {
                        return memStream.ToArray();
                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
        }


        public async Task LoadAsync(byte[] serialized)
        {
            if (serialized != null && serialized.Any())
            {
                ImageSource imageSource = await serialized.AsBitmapImageAsync();
                image.Source = imageSource;
                image.IsVisible = true;
                inkCanvas.IsVisible = false;
            }
            else
            {
                image.IsVisible = false;
                inkCanvas.IsVisible = true;
                inkCanvas.Clear();
            }
        }


        private static async void ImageBytesChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var ctl = (SignatureControl)bindable;
            if (ctl != null && !ctl._WritingBytes)
            {
                await ctl.LoadAsync(ctl.ImageBytes);
            }
        }
    }
}
