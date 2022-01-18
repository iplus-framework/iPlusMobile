using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class SumQuantityByBarcodeViewModel : BaseViewModel
    {
        public SumQuantityByBarcodeViewModel(string material)
        {
            SumItemQuantites = new ObservableCollection<SumItem>();
            Title = "Σ " + material;
        }

        public static Helpers.GS1.AII NetWeightInKgAI = Helpers.GS1.GetAII("310d");
        public static Helpers.GS1.AII ExternLotNoAI = Helpers.GS1.GetAII("10");
        public static Helpers.GS1.AII ExpDateAI = Helpers.GS1.GetAII("15");

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

        public async Task OnBarcodeScanned(string currentBarcode)
        {
            Message = null;

            if (string.IsNullOrEmpty(currentBarcode))
                return;

            try
            {
                await Task.Run(() =>
                {
                    currentBarcode = currentBarcode.TrimStart();
                    currentBarcode = currentBarcode.TrimEnd();

                    Helpers.GS1.HasCheckSum = false;
                    var parseResult = Helpers.GS1.Parse(currentBarcode);

                    if (parseResult != null && parseResult.Any())
                    {
                        Helpers.GS1.ParseResult externLotNo, expDate, netWeight;

                        parseResult.TryGetValue(NetWeightInKgAI, out netWeight);
                        parseResult.TryGetValue(ExpDateAI, out expDate);
                        parseResult.TryGetValue(ExternLotNoAI, out externLotNo);


                        if (netWeight.StringResult != null)
                        {
                            if (SumItemQuantites != null && SumItemQuantites.Any(c => c.Barcode == currentBarcode))
                            {
                                Message = new Msg(eMsgLevel.Warning, Strings.AppStrings.AlreadAddedItem_Text);
                                return;
                            }

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
                });
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
        }

        public void Clear()
        {
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

        }
    }
}
