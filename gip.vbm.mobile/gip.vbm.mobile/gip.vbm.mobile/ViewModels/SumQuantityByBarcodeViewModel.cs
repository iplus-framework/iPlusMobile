using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class SumQuantityByBarcodeViewModel : BarcodeScanModelBase
    {
        public SumQuantityByBarcodeViewModel(string material)
        {
            SumItemQuantites = new ObservableCollection<SumItem>();
            Title = "Σ " + material;
        }

        public static GS1.AII NetWeightInKgAI = GS1.GetAII("310d");
        public static GS1.AII ExternLotNoAI = GS1.GetAII("10");
        public static GS1.AII ExpDateAI = GS1.GetAII("15");

        public struct SumItem
        {
            public string Barcode
            {
                get;
                set;
            }

            public double Quantity
            {
                get;
                set;
            }

            public DateTime? ExpDate
            {
                get;
                set;
            }

            public string ExtLotNo
            {
                get;
                set;
            }
        }


        public DateTime? SumItemsExpDate
        {
            get;
            set;
        }

        public string SumItemsLotNo
        {
            get;
            set;
        }

        private double _SumQuantity;
        public double SumQuantity
        {
            get => _SumQuantity;
            set
            {
                SetProperty(ref _SumQuantity, value);
            }
        }

        public ObservableCollection<SumItem> SumItemQuantites
        {
            get;
            set;
        }

        private string _TempCurrentBarcode;

        public async Task OnBarcodeScanned(string currentBarcode)
        {
            Message = null;

            if (string.IsNullOrEmpty(currentBarcode) || !string.IsNullOrEmpty(_TempCurrentBarcode))
                return;

            try
            {
                await Task.Run(() =>
                {
                    ProcessBarcode(currentBarcode);
                });
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
        }

        private void ProcessBarcode(string currentBarcode, bool skipSameItemCheck = false)
        {
            currentBarcode = currentBarcode.TrimStart();
            currentBarcode = currentBarcode.TrimEnd();

            var parseResult = GS1.Parse(currentBarcode, false);

            if (parseResult != null && parseResult.Any())
            {
                GS1.ParseResult externLotNo, expDate, netWeight;

                parseResult.TryGetValue(NetWeightInKgAI, out netWeight);
                parseResult.TryGetValue(ExpDateAI, out expDate);
                parseResult.TryGetValue(ExternLotNoAI, out externLotNo);


                if (netWeight.StringResult != null)
                {
                    if (!skipSameItemCheck && SumItemQuantites != null && SumItemQuantites.Any(c => c.Barcode == currentBarcode))
                    {
                        var question = new Msg(eMsgLevel.Question, Strings.AppStrings.AlreadAddedItem_Text) { MessageButton = eMsgButton.YesNo };
                        _TempCurrentBarcode = currentBarcode;
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            ShowDialog(question, "", null, "", 1);
                        });
                        return;
                    }

                    _TempCurrentBarcode = null;

                    if (!netWeight.DoubleResult.HasValue)
                    {
                        Message = new Msg(eMsgLevel.Warning, Strings.AppStrings.NetWeightParseError_Text);
                        return;
                    }

                    if (externLotNo.StringResult != null)
                    {
                        if (SumItemsLotNo == null)
                        {
                            if (SumItemQuantites.Count > 0)
                                return;

                            SumItemsLotNo = externLotNo.StringResult;
                        }
                        else if (SumItemsLotNo != externLotNo.StringResult)
                        {
                            Message = new Msg(eMsgLevel.Warning, Strings.AppStrings.DifferentLotInList_Text);
                            return;
                        }
                    }

                    DateTime? expirationDate = null;
                    if (expDate.StringResult != null)
                    {
                        DateTime dt;
                        if (DateTime.TryParseExact(expDate.StringResult, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out dt))
                        {
                            expirationDate = dt;
                            if (SumItemsExpDate == null)
                            {
                                if (SumItemQuantites.Count > 0)
                                    return;

                                SumItemsExpDate = dt;
                            }
                            else if (dt != SumItemsExpDate)
                            {
                                Message = new Msg(eMsgLevel.Warning, Strings.AppStrings.DifferentExpDateInList_Text);
                                return;
                            }
                        }
                    }

                    if (expirationDate == null && externLotNo.StringResult == null && SumItemQuantites.Count > 0)
                        return;

                    SumItem sItem = new SumItem() { Barcode = currentBarcode, Quantity = netWeight.DoubleResult.Value, ExpDate = expirationDate, ExtLotNo = externLotNo.StringResult };
                    SumItemQuantites.Add(sItem);
                    SumQuantity = SumItemQuantites.Sum(x => x.Quantity);
                }
            }
        }

        public override void Clear()
        {
            base.Clear();
            if (SumItemQuantites != null)
            {
                SumItemQuantites.Clear();
            }
            SumQuantity = 0;
            SumItemsLotNo = null;
            SumItemsExpDate = null;
            Message = null;
        }

        public override void DialogResponse(Global.MsgResult result, string enteredValue = null)
        {
            if (DialogOptions.RequestID == 1 && result == Global.MsgResult.Yes && !string.IsNullOrEmpty(_TempCurrentBarcode))
            {
                ProcessBarcode(_TempCurrentBarcode, true);
            }
            _TempCurrentBarcode = null;
        }
    }
}
