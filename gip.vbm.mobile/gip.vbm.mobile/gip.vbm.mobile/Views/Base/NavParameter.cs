namespace gip.vbm.mobile.Views
{
    public class NavParameter
    {
        public NavParameter(PageStateEnum pageMode)
        {
            PageMode = pageMode;
        }
        public PageStateEnum PageMode { get; set; }
        public object Arguments { get; set; }
    }
}
