using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.webservices;
using gip.vb.mobile.Helpers;
using gip.vb.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace gip.vb.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOBarcodeTaskManuDetails : BSOPageBase
    {
        BarcodeScanManuModel _ViewModel;

        public BSOBarcodeTaskManuDetails(BarcodeScanManuModel viewModel)
        {
            _ViewModel = viewModel;
            InitializeComponent();
            BindingContext = _ViewModel;
            EnableButtons();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SendChangedACMethod();
        }

        #region Properties


        #endregion


        #region Methods

        private void BtnReleaseMachine_Clicked(object sender, EventArgs e)
        {
            if (!IsEnabledBtnReleaseMachine())
                return;

            _ViewModel.PropertyChanged += _ViewModel_PropertyChanged;
            _ViewModel.ReleaseMachine();
        }

        private async void _ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_ViewModel.ReleaseMachine) && _ViewModel.DialogOptions.RequestID == 10)
            {
                await Navigation.PopAsync();
                _ViewModel.PropertyChanged -= _ViewModel_PropertyChanged;
            }
        }

        private async void BtnOccupyMachine_Clicked(object sender, EventArgs e)
        {
            if (!IsEnabledBtnOccupyMachine())
                return;

            await _ViewModel.InvokeActionOnMachine();
            EnableButtons();
        }

        private async void BtnPauseOrderOnMachine_Clicked(object sender, EventArgs e)
        {
            if (!IsEnabledBtnPauseOrderOnMachine())
                return;

            await _ViewModel.PauseOrderOnMachine();
            EnableButtons();
        }

        private async void BtnDoBooking_Clicked(object sender, EventArgs e)
        {
            if (!IsEnabledBtnDoBooking())
                return;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo.IntermediateBatch != null)
            {
                short materialWFConnMode = wfInfo.MaterialWFConnectionMode;

                if (wfInfo.WFMethod != null)
                {
                    wfInfo.WFMethod.ParameterValueList.Add(new core.datamodel.ACValue() { ACIdentifier = nameof(wfInfo.PostingQSuggestionMode), Value = wfInfo.PostingQSuggestionMode });
                    wfInfo.WFMethod.ParameterValueList.Add(new core.datamodel.ACValue() { ACIdentifier = nameof(wfInfo.PostingQSuggestionMode2), Value = wfInfo.PostingQSuggestionMode2 });
                }

                //Intermediate selector with batch
                if (materialWFConnMode == 10)
                {
                    //TODO: filter intermediate products which are related to the workflow node
                    await Navigation.PushAsync(new BSOProdOrderIntermediate(wfInfo.ProdOrderPartslist, _ViewModel));
                }
                else if (!wfInfo.IntermediateBatch.IsFinalMixure)
                {
                    ACValue inwardPostSQ = wfInfo.WFMethod.ParameterValueList.GetACValue(GlobalApp.WFParam_InwardPostingSuggestionQ);

                    //await Navigation.PushAsync(new BSOProdOrderOutwardMatSel(wfInfo.IntermediateBatch, _ViewModel, new Helpers.PostingSuggestionMode(wfInfo.PostingQSuggestionMode, wfInfo.ValidSeqNoForPostingQSMode)));

                    if (inwardPostSQ != null && inwardPostSQ.Value != null)
                    {
                        await Navigation.PushAsync(new BSOProdOrderInOutSelector(wfInfo.IntermediateBatch, _ViewModel, wfInfo.WFMethod));
                    }
                    else
                    {
                        await Navigation.PushAsync(new BSOProdOrderOutwardMatSel(wfInfo.IntermediateBatch, _ViewModel, wfInfo.WFMethod));
                    }
                }
                else if (!wfInfo.IntermediateBatch.HasInputMaterials)
                {
                    //await Navigation.PushAsync(new BSOProdOrderInward(wfInfo.IntermediateBatch, _ViewModel, wfInfo.OrderQuantityOnInwardPosting));
                    await Navigation.PushAsync(new BSOProdOrderInward(wfInfo.IntermediateBatch, _ViewModel, wfInfo.WFMethod));
                }
                else
                {
                    //await Navigation.PushAsync(new BSOProdOrderInOutSelector(wfInfo.IntermediateBatch, _ViewModel, wfInfo.OrderQuantityOnInwardPosting, new Helpers.PostingSuggestionMode(wfInfo.PostingQSuggestionMode, wfInfo.ValidSeqNoForPostingQSMode)));
                    await Navigation.PushAsync(new BSOProdOrderInOutSelector(wfInfo.IntermediateBatch, _ViewModel, wfInfo.WFMethod));
                }
            }
            else if (wfInfo.Intermediate != null)
            {
                await Navigation.PushAsync(new BSOProdOrderBatch(wfInfo.Intermediate, _ViewModel));
            }
            else
            {
                await Navigation.PushAsync(new BSOProdOrderIntermediate(wfInfo.ProdOrderPartslist, _ViewModel));
            }
        }

        private async void BtnEnterACMParams_Clicked(object sender, EventArgs e)
        {
            if (!IsEnabledBtnEnterACMParams())
                return;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null || wfInfo.WFMethod == null)
                return;

            wfInfo.WFMethod.AutoRemove = false;
            _ViewModel.InitUserTime(wfInfo);
            await Navigation.PushAsync(new BSOACMethodEditor(wfInfo));
        }

        private async void SendChangedACMethod()
        {
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo != null && wfInfo.WFMethod != null && wfInfo.WFMethod.AutoRemove == true)
            {
                wfInfo.WFMethod.AutoRemove = false;
                BarcodeEntity entity = _ViewModel.Item.Sequence.LastOrDefault();
                if (entity == null)
                {
                    _ViewModel.ResetScanSequence();
                    return;
                }
                entity.SelectedOrderWF = wfInfo;
                entity.WFMethod = wfInfo.WFMethod;
                //_ViewModel.InvokeBarcodeCommand.Execute(null);
                await _ViewModel.InvokeActionOnMachine();
            }
        }

        private void EnableButtons()
        {
            BtnDoBooking.IsEnabled = IsEnabledBtnDoBooking();
            BtnOccupyMachine.IsVisible = IsEnabledBtnOccupyMachine();
            BtnReleaseMachine.IsVisible = IsEnabledBtnReleaseMachine();
            BtnPauseOrderOnMachine.IsEnabled = IsEnabledBtnPauseOrderOnMachine();
            BtnEnterACMParams.IsEnabled = IsEnabledBtnEnterACMParams();
        }

        private bool IsEnabledBtnDoBooking()
        {
            if (_ViewModel == null)
                return false;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null)
                return false;
            return wfInfo.InfoState > POPartslistInfoState.None;
        }

        private bool IsEnabledBtnOccupyMachine()
        {
            if (_ViewModel == null)
                return false;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null)
                return false;
            return wfInfo.InfoState != POPartslistInfoState.Release;
        }

        private bool IsEnabledBtnReleaseMachine()
        {
            if (_ViewModel == null)
                return false;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null)
                return false;
            return wfInfo.InfoState > POPartslistInfoState.None;
        }

        private bool IsEnabledBtnPauseOrderOnMachine()
        {
            if (_ViewModel == null)
                return false;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null)
                return false;
            return wfInfo.InfoState > POPartslistInfoState.None;
        }

        private bool IsEnabledBtnEnterACMParams()
        {
            if (_ViewModel == null)
                return false;
            ProdOrderPartslistWFInfo wfInfo = _ViewModel.SelectedSequence as ProdOrderPartslistWFInfo;
            if (wfInfo == null)
                return false;
            return wfInfo.WFMethod != null;
        }

        #endregion


    }
}