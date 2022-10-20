using gip.core.webservices;

namespace gip.vbm.mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //this.Items
            foreach (VBMenuItem vbMenutItem in App.UserRights.Menu)
            {
            }
        }
    }
}