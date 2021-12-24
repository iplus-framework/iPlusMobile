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

        public Guid? CurrentScannedPrinterACClassID
        {
            get;
            set;
        }


        #endregion

        #region Commands

        public Command GetAssignedPrinter { get; set; }
        public async Task<bool> ExecuteGetAssignedPrinter()
        {
            if (IsBusy)
                return false;

            try
            {
                IsBusy = true;

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
            finally
            {
                IsBusy = false;
            }


            return true;
        }

        public Command ScanPrinterID { get; set; }
        public async Task<bool> ExecuteScanPrinterID()
        {
            Message = null;
            CurrentScannedPrinter = null;
            CurrentScannedPrinterACClassID = null;

            if (IsBusy)
                return false;

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
                        CurrentScannedPrinterACClassID = printServerACClassID;
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
            finally
            {
                IsBusy = false;
            }

            return true;
        }

        public Command AssignPrinter { get; set; }
        public async Task<bool> ExecuteAssignPrinter()
        {
            if (IsBusy)
                return false;

            if (string.IsNullOrEmpty(CurrentScannedPrinter))
                return false;

            IsBusy = true;

            try
            {
                string printerID = CurrentScannedPrinterACClassID.HasValue ? CurrentScannedPrinterACClassID.Value.ToString() : CurrentScannedPrinter;

                var result = await _WebService.AssignPrinterAsync(printerID);
                if (result.Suceeded)
                {
                    AssignedPrinter = result.Data;
                    Message = new Msg(eMsgLevel.Info, "Printer was sucessfully assigned!");
                }
                else if (result.Message != null)
                {
                    Message = result.Message;
                    return false;
                }
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
            return true;
        }

        #endregion

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
        }
    }
}
