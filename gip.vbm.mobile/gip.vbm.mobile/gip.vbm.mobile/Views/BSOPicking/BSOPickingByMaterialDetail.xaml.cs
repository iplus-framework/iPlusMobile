// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.webservices;
using gip.vbm.mobile.barcode;
using gip.vbm.mobile.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOPickingByMaterialDetail : BSOTabbedPageBase
    {
        private PickingByMaterialViewModel _ViewModel;

        public BSOPickingByMaterialDetail()
        {
            BindingContext = _ViewModel = new PickingByMaterialViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitPageOnNavigation();
            _ViewModel.PropertyChanged += _ViewModel_PropertyChanged;
        }

        private void _ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_ViewModel != null && e.PropertyName == "MissingBookingQuantity")
            {
                if (_ViewModel.MissingBookingQuantity < 0)
                {
                    lblMissingQuantity.TextColor = Colors.Red;
                }
                else
                {
                    lblMissingQuantity.TextColor = Colors.Lime;
                }
            }
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
            _ViewModel.PropertyChanged -= _ViewModel_PropertyChanged;
        }

        private void InitPageOnNavigation()
        {
            barcodeScanner.OnAppearing();
            if (NavParam != null)
            {
                PickingMaterial material = NavParam.Arguments as PickingMaterial;
                if (material != null)
                {
                    _ViewModel.PickingMaterialItem = material;
                }
            }
            this.PageState = PageStateEnum.View;
        }

        private void PickingsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _ViewModel.ChangePostingQuantity();
        }

        #region Barcode

#if ZXING
        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
#else
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
#endif
        {
#if ZXING
            await barcodeScanner.OpenBarcodeCamera();
#endif
        }

#endregion

        private void ButtonDoBooking_Clicked(object sender, EventArgs e)
        {
            BarcodeEntity barcodeEntity = _ViewModel.WSBarcodeEntityResult;
            if (barcodeEntity == null)
            {
                _ViewModel.Message = new core.datamodel.Msg() { MessageLevel = core.datamodel.eMsgLevel.Error, 
                                                                Message =  Strings.AppStrings.EmptyBarcodeEntity_Text };
                return;
            }

            if (barcodeEntity.FacilityCharge != null)
            {
                if (barcodeEntity.FacilityCharge.Material.MaterialID != _ViewModel.PickingMaterialItem.Material.MaterialID)
                {
                    _ViewModel.Message = new core.datamodel.Msg() { MessageLevel = core.datamodel.eMsgLevel.Error, 
                                                                    Message = Strings.AppStrings.PickingWrongMaterial_Text };
                    return;
                }
            }
            else
            {
                _ViewModel.Message = new core.datamodel.Msg() { MessageLevel = core.datamodel.eMsgLevel.Error,
                                                                Message = Strings.AppStrings.PickingWrongBarcodeEntityQ_Text};
                return;
            }

            _ViewModel.BookFacilityCommand.Execute(null);
        }

        private async void BtnWarehouseInfo_Clicked(object sender, EventArgs e)
        {
            Material material = _ViewModel?.PickingMaterialItem?.Material;
            if (material == null)
            {
                _ViewModel.ShowDialog(new core.datamodel.Msg(core.datamodel.eMsgLevel.Error, "Material to overview is null!"));
                return;
            }

            await Navigation.PushAsync(new BSOFacilityMaterialOverview() { NavParam = new NavParameter(PageStateEnum.View) { Arguments = new Tuple<Material, string>(material, "QuantPivotItem") } });
        }
    }
}