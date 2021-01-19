using Xamarin.Forms;

namespace gip.vb.mobile.Views
{
    public class BSOTabbedPageBase : TabbedPage, IBSOPage
    {
        public BSOTabbedPageBase() : base()
        {
        }

        public NavParameter NavParam
        {
            get; set;
        }

        public virtual void OnNavigatedTo(NavigationEventArgs e, NavigationMode navigationMode)
        {
        }

        public virtual void OnNavigatedFrom(NavigationEventArgs e, NavigationMode navigationMode)
        {
        }


        public static readonly BindableProperty PageStateProperty = BindableProperty.Create("PageState", typeof(PageStateEnum), typeof(BSOTabbedPageBase), PageStateEnum.View);
        public PageStateEnum PageState
        {
            get
            {
                return (PageStateEnum)GetValue(PageStateProperty);
            }
            set
            {
                SetValue(PageStateProperty, value);
                OnPageStateChanged();
            }
        }

        protected virtual void OnPageStateChanged()
        {
        }
    }
}
