using gip.vb.mobile.Helpers;
using System;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
{
    /// <summary>
    /// Class ImageButton.
    /// </summary>
    public class ImageButton : Button
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(ImageSource),
            typeof(ImageButton), null, BindingMode.OneWay, null, (bindable, oldvalue, newvalue) => ((VisualElement)bindable).ToString());

        public static readonly BindableProperty DisabledSourceProperty = BindableProperty.Create("DisabledSource", typeof(ImageSource),
            typeof(ImageButton), null, BindingMode.OneWay, null, (bindable, oldvalue, newvalue) => ((VisualElement)bindable).ToString());

        public static readonly BindableProperty ImageWidthRequestProperty = BindableProperty.Create("ImageWidthRequest", typeof(int), typeof(ImageButton), default(int));

        public static readonly BindableProperty ImageHeightRequestProperty = BindableProperty.Create("ImageHeightRequest", typeof(int), typeof(ImageButton), default(int));

        public static readonly BindableProperty ImagePaddingProperty = BindableProperty.Create("ImagePadding", typeof(Thickness), typeof(ImageButton), default(Thickness));

        public static readonly BindableProperty OrientationProperty = BindableProperty.Create("Orientation", typeof(ImageOrientation), typeof(ImageButton), ImageOrientation.ImageToLeft);

        public static readonly BindableProperty ImageTintColorProperty = BindableProperty.Create("ImageTintColor", typeof(Color), typeof(ImageButton), Color.Transparent);

        public static readonly BindableProperty DisabledImageTintColorProperty = BindableProperty.Create("DisabledImageTintColor", typeof(Color), typeof(ImageButton), Color.Transparent);

        public static readonly BindableProperty ToolTipProperty = BindableProperty.Create("ToolTip", typeof(string), typeof(ImageButton), default(string));

        /// <summary>
        /// Gets or sets the ImageSource to use with the control.
        /// </summary>
        /// <value>
        /// The Source property gets/sets the value of the backing field, SourceProperty.
        /// </value>
        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ImageSource to use with the control when the button is disabled.
        /// </summary>
        /// <value>
        /// The Source property gets/sets the value of the backing field, DisabledSourceProperty.
        /// </value>
        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource DisabledSource
        {
            get { return (ImageSource)GetValue(DisabledSourceProperty); }
            set { SetValue(DisabledSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets The orientation of the image relative to the text.
        /// </summary> 
        /// <value>
        /// The Orientation property gets/sets the value of the backing field, OrientationProperty.
        /// </value> 
        public ImageOrientation Orientation
        {
            get { return (ImageOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the requested height of the image. If less than or equal to zero than a 
        /// height of 50 will be used.
        /// </summary>
        /// <value>
        /// The ImageHeightRequest property gets/sets the value of the backing field, ImageHeightRequestProperty.
        /// </value> 
        public int ImageHeightRequest
        {
            get { return (int)GetValue(ImageHeightRequestProperty); }
            set { SetValue(ImageHeightRequestProperty, value); }
        }

        /// <summary>
        /// Gets or sets the requested width of the image. If less than or equal to zero than a 
        /// width of 50 will be used.
        /// </summary>
        /// <value>
        /// The ImageWidthRequest property gets/sets the value of the backing field, ImageWidthRequestProperty.
        /// </value> 
        public int ImageWidthRequest
        {
            get { return (int)GetValue(ImageWidthRequestProperty); }
            set { SetValue(ImageWidthRequestProperty, value); }
        }

        public Thickness ImagePadding
        {
            get { return (Thickness)GetValue(ImagePaddingProperty); }
            set { SetValue(ImagePaddingProperty, value); }
        }


        /// <summary>
        /// Gets or sets the tint color of the image 
        /// </summary>
        /// <value>
        /// The ImageTintColor property gets/sets the value of the backing field, ImageTintColorProperty.
        /// </value> 
        public Color ImageTintColor
        {
            get { return (Color)GetValue(ImageTintColorProperty); }
            set { SetValue(ImageTintColorProperty, value); }
        }


        /// <summary>
        /// Gets or sets the tint color of the image when the button is disabled
        /// </summary>
        /// <value>
        /// The DisabledImageTintColor property gets/sets the value of the backing field, DisabledImageTintColorProperty.
        /// </value> 
        public Color DisabledImageTintColor
        {
            get { return (Color)GetValue(DisabledImageTintColorProperty); }
            set { SetValue(DisabledImageTintColorProperty, value); }
        }

        public String ToolTip
        {
            get { return (String)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        public bool IsToggleable { get; set; }

    }
}