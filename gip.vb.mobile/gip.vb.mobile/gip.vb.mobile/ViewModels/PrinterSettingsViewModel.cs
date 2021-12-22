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


        #region Properties

        private string _AssignedPrinter;
        public string AssignedPrinter
        {
            get => _AssignedPrinter;
            set
            {
                SetProperty(ref _AssignedPrinter, value);
            }
        }

        public string _CurrentBarcode;
        public string CurrentBarcode
        {
            get
            {
                return _CurrentBarcode;
            }
            set
            {
                SetProperty(ref _CurrentBarcode, value);
            }
        }

        private string _CurrentScannedPrinter;
        public string CurrentScannedPrinter
        {
            get => _CurrentScannedPrinter;
            set
            {
                SetProperty(ref _CurrentScannedPrinter, value);
            }
        }

        #endregion

        #region Commands

        public Command GetAssignedPrinter { get; set; }
        public async Task<bool> ExecuteGetAssignedPrinter()
        {
            try
            {
                var response = await _WebService.GetAssignedPrinterAsync();
                if (response.Suceeded)
                {
                    AssignedPrinter = response.Data;
                }
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }


            return true;
        }

        public Command ScanPrinterID { get; set; }
        public async Task<bool> ExecuteScanPrinterID()
        {
            Message = null;
            CurrentScannedPrinter = null;

            if (string.IsNullOrEmpty(CurrentBarcode))
            {
                Message = new Msg(eMsgLevel.Error, Strings.AppStrings.EmptyBarcodeEntity_Text);
                return false;
            }

            try
            {
                Guid printServerACClassID;
                if (Guid.TryParse(CurrentBarcode, out printServerACClassID))
                {
                    var result = await _WebService.GetACClassByBarcodeAsync(CurrentBarcode);
                    if (result.Suceeded)
                    {
                        CurrentScannedPrinter = result.Data.ACCaption + System.Environment.NewLine + result.Data.ACUrlComponent;
                    }
                    else if (result.Message != null)
                    {
                        Message = result.Message;
                        return false;
                    }
                }
                else
                {
                    var result = await _WebService.GetScannedPrinterAsync(CurrentBarcode);
                    if (result.Suceeded)
                    {
                        CurrentScannedPrinter = result.Data;
                    }
                    else if (result.Message != null)
                    {
                        Message = result.Message;
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
                return false;
            }

            return true;
        }

        public Command AssignPrinter { get; set; }
        public async Task<bool> ExecuteAssignPrinter()
        {
            return true;
        }

        #endregion

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }
    }
}
