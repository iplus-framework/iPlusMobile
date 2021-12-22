using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PrinterSettingsViewModel : BaseViewModel
    {
        public PrinterSettingsViewModel()
        {
            GetAssignedPrinter = new Command(async () => await ExecuteGetAssignedPrinter());
            ScanPrinterID = new Command(async () => await ExecuteScanPrinterID());
            AssignPrinter = new Command(async () => await ExecuteAssignPrinter());
        }


        public Command GetAssignedPrinter { get; set; }
        public Task<bool> ExecuteGetAssignedPrinter()
        {
            return null;
        }

        public Command ScanPrinterID { get; set; }
        public Task<bool> ExecuteScanPrinterID()
        {
            return null;
        }

        public Command AssignPrinter { get; set; }
        public Task<bool> ExecuteAssignPrinter()
        {
            return null;
        }

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }
    }
}
