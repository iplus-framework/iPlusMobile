// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class SumQuantityByBarcodeViewModel : BarcodeScanModelBase
    {
        public SumQuantityByBarcodeViewModel(string material)
        {
            SumItemQuantites = new ObservableCollection<SumItem>();
            Title = "Σ " + material;
        }

        public static GS1.AII NetWeightInKgAI = GS1.GetAII("310d");
        public static GS1.AII GrossWeightInKgAI = GS1.GetAII("330d");
        public static GS1.AII AmountInPartsAI = GS1.GetAII("30");
        public static GS1.AII ExternLotNoAI = GS1.GetAII("10");
        public static GS1.AII ExpDateAI = GS1.GetAII("15");
        public static GS1.AII ExpiryDateAI = GS1.GetAII("17");

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

            public double QuantityGross
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

        public SumItem? CurrentBarcodeInfo
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

        internal void ProcessBarcode(string currentBarcode, bool skipSameItemCheck = false)
        {
            currentBarcode = currentBarcode.TrimStart();
            currentBarcode = currentBarcode.TrimEnd();

            var parseResult = GS1.Parse(currentBarcode, false);

            if (parseResult != null && parseResult.Any())
            {
                GS1.ParseResult externLotNo, expDate, expDate1, netWeight, grossWeight, amountInParts;

                parseResult.TryGetValue(NetWeightInKgAI, out netWeight);
                parseResult.TryGetValue(ExpDateAI, out expDate);
                parseResult.TryGetValue(ExpiryDateAI, out expDate1);
                parseResult.TryGetValue(ExternLotNoAI, out externLotNo);
                parseResult.TryGetValue(GrossWeightInKgAI, out grossWeight);
                parseResult.TryGetValue(AmountInPartsAI, out amountInParts);

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
                        expirationDate = ParseExpirationDate(expDate.StringResult);

                    }
                    if (expirationDate == null && expDate1.StringResult != null)
                    {
                        expirationDate = ParseExpirationDate(expDate1.StringResult);
                    }

                    if(expirationDate != null)
                    {
                        if (SumItemsExpDate == null)
                        {
                            if (SumItemQuantites.Count > 0)
                                return;
                            SumItemsExpDate = expirationDate;
                        }
                        else if (SumItemsExpDate != expirationDate)
                        {
                            Message = new Msg(eMsgLevel.Warning, Strings.AppStrings.DifferentExpDateInList_Text);
                            return;
                        }
                    }

                    if ((expirationDate == null && externLotNo.StringResult == null && SumItemQuantites.Count > 0))
                        return;

                    SumItem sItem = new SumItem() { Barcode = currentBarcode, Quantity = netWeight.DoubleResult.Value, ExpDate = expirationDate, ExtLotNo = externLotNo.StringResult };
                    SumItemQuantites.Add(sItem);
                    SumQuantity = SumItemQuantites.Sum(x => x.Quantity);
                }
                else
                {
                    DateTime? expirationDate = null;
                    double quantityGross = 0;
                    double quantity = 0;
                    string extLotNo = null;

                    if (expDate.StringResult != null)
                    {
                        DateTime dt;
                        if (DateTime.TryParseExact(expDate.StringResult, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out dt))
                            expirationDate = dt;
                    }

                    if (grossWeight.DoubleResult.HasValue)
                        quantityGross = grossWeight.DoubleResult.Value;
                    else if (amountInParts.DoubleResult.HasValue)
                        quantity = amountInParts.DoubleResult.Value;

                    if (externLotNo.StringResult != null)
                        extLotNo = externLotNo.StringResult.Trim();

                    if (expirationDate.HasValue || !string.IsNullOrEmpty(extLotNo) || quantityGross > double.Epsilon || quantity > double.Epsilon)
                    {
                        SumItem sItem = new SumItem() { Barcode = currentBarcode, Quantity = quantity, QuantityGross = quantityGross, ExpDate = expirationDate, ExtLotNo = extLotNo };
                        SumItemQuantites.Add(sItem);
                        SumQuantity = SumItemQuantites.Sum(x => x.Quantity);
                    }
                }
            }
        }

        private DateTime? ParseExpirationDate(string dateStrValue)
        {
            DateTime? result = null;
            DateTime dt;
            if (DateTime.TryParseExact(dateStrValue, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out dt))
            {
                result = dt;
            }
            return result;
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
