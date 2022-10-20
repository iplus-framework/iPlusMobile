using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.ViewModels
{
    public class FacilityChargeMakerViewModel : BaseViewModel, IBarcodeScanFacilityHost
    {
        #region c'tors

        public FacilityChargeMakerViewModel(Material material) : base()
        {
            SearchMaterialCommand = new Command(async () => await ExecuteSearchMaterial());
            CreateNewFacilityChargeCommand = new Command(async () => await ExecuteCreateNewFacilityCharge());

            FacilityScanViewModel = new BarcodeScanFacilityModel(this);

            FacilityChargeItem = new FacilityCharge();
            FacilityChargeItem.FacilityLot = new FacilityLot();
            FacilityChargeItem.Material = material;
            Title = Strings.AppStrings.NewQuant_Text;
        }

        #endregion

        #region Properties

        public FacilitySelectorViewModel FacilitySelector
        {
            get;
            set;
        }

        private FacilityCharge _TempFacilityChargeForPrint;

        public FacilityCharge _FacilityChargeItem;
        public FacilityCharge FacilityChargeItem
        {
            get => _FacilityChargeItem;
            set
            {
                SetProperty(ref _FacilityChargeItem, value);
            }
        }

        private string _MaterialSearchText;
        public string MaterialSearchText
        {
            get => _MaterialSearchText;
            set
            {
                SetProperty(ref _MaterialSearchText, value);
            }
        }

        private Material _SelectedMaterial;
        public Material SelectedMaterial
        {
            get => _SelectedMaterial;
            set
            {
                SetProperty(ref _SelectedMaterial, value);
                IsSelectMaterialVisible = false;
            }
        }

        private List<Material> _MaterialList;
        public List<Material> MaterialList
        {
            get => _MaterialList;
            set
            {
                SetProperty(ref _MaterialList, value);
            }
        }

        private bool _IsSelectMaterialVisible;
        public bool IsSelectMaterialVisible
        {
            get => _IsSelectMaterialVisible;
            set => SetProperty(ref _IsSelectMaterialVisible, value);
        }

        public BarcodeScanFacilityModel FacilityScanViewModel;

        public Guid? FacilityInventoryID
        {
            get;
            set;
        }

        #endregion

        #region Methods

        #region Methods => Commands

        public Command CreateNewFacilityChargeCommand { get; set; }
        public async Task ExecuteCreateNewFacilityCharge()
        {
            if (IsBusy || FacilityChargeItem == null)
                return;

            try
            {
                IsBusy = true;

                Message = null;

                if (FacilityChargeItem.Material == null)
                {
                    Message = new Msg(eMsgLevel.Warning, Strings.AppStrings.MaterialNotSelected_Text);
                    return;
                }

                if (FacilityChargeItem.Facility == null)
                {
                    Message = new Msg(eMsgLevel.Warning, Strings.AppStrings.FacilityNotSelected_Text);
                    return;
                }

                if (FacilityChargeItem.FacilityLot == null)
                {
                    Message = new Msg(eMsgLevel.Error, "FacilityLot is null.");
                    return;
                }

                FacilityChargeParamItem fcParam = new FacilityChargeParamItem();
                fcParam.Material = FacilityChargeItem.Material;
                fcParam.Facility = FacilityChargeItem.Facility;
                fcParam.FacilityLot = FacilityChargeItem.FacilityLot;
                fcParam.StockQuantity = FacilityChargeItem.StockQuantity;

                if (FacilityInventoryID.HasValue)
                {
                    fcParam.ParamID = FacilityInventoryID.Value;
                }

                var response = await _WebService.CreateFacilityChargeAsync(fcParam);
                if (response.Suceeded)
                {
                    if (response.Message != null)
                    {
                        Message = response.Message;
                        return;
                    }

                    if (response.Data == null)
                    {
                        Message = new Msg(eMsgLevel.Error, "Posting error!");
                        return;
                    }

                    _TempFacilityChargeForPrint = response.Data;
                    if (_TempFacilityChargeForPrint != null)
                    {
                        ShowDialog(new Msg(eMsgLevel.QuestionPrompt, Strings.AppStrings.PickingBookSuccAndPrint_Question), "", Keyboard.Numeric, "1", 1);
                    }
                }
                else if (response.Message != null)
                {
                    Message = response.Message;
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
        }

        public Command SearchMaterialCommand { get; set; }
        public async Task ExecuteSearchMaterial()
        {
            if (IsBusy || string.IsNullOrEmpty(MaterialSearchText))
                return;

            try
            {
                IsBusy = true;

                var response = await _WebService.SearchMaterialAsync(MaterialSearchText);
                if (response.Suceeded)
                {
                    MaterialList = response.Data;
                }
                else if (response.Message != null)
                {
                    Message = response.Message;
                }

                IsSelectMaterialVisible = true;
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecutePrintNewFacilityCharge(int copies = 1)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                PrintEntity printEntity = new PrintEntity();
                printEntity.CopyCount = copies;
                printEntity.Sequence = new List<BarcodeEntity>();
                printEntity.Sequence.Add(new BarcodeEntity() { FacilityCharge = _TempFacilityChargeForPrint });

                var response = await _WebService.Print(printEntity);

                if (response.Suceeded)
                {
                    if (response.Message != null)
                    {
                        Message = response.Message;
                    }
                }
                else if (response.Message != null)
                {
                    Message = response.Message;
                }
                _TempFacilityChargeForPrint = null;
            }
            catch (Exception e)
            {
                Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public void OnAppear()
        {
            if (FacilitySelector != null && FacilityChargeItem != null)
            {
                FacilityChargeItem.Facility = FacilitySelector.SelectedStorageLocation;
                FacilityChargeItem.OnPropChanged(nameof(FacilityChargeItem.Facility));
                FacilitySelector = null;
            }
        }

        public async override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {
            if (DialogOptions.RequestID == 1)
            {
                if (result == Global.MsgResult.OK)
                {
                    int copies = 0;
                    if (int.TryParse(entredValue, out copies))
                    {
                        if (copies > 0)
                        {
                            await ExecutePrintNewFacilityCharge(copies);
                        }
                    }
                }
            }
        }

        public void OnFacilityScanned(Facility facility)
        {
            if (FacilityChargeItem != null)
            {
                FacilityChargeItem.Facility = facility;
                FacilityChargeItem.OnPropChanged(nameof(FacilityChargeItem.Facility));
            }
        }

        public void OnSearchFacility(string searchText)
        {
        }

        #endregion
    }
}
