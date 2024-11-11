// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.vbm.mobile.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOFacilitySelector : BSOPageBase
    {
        FacilitySelectorViewModel _ViewModel;

        public BSOFacilitySelector(FacilitySelectorViewModel viewModel)
        {
            BindingContext = _ViewModel = viewModel;
            InitializeComponent();

            Binding binding = new Binding("StorageLocationPath");
            binding.Source = BindingContext;
            SetBinding(FlexLayoutItemsSourceProperty, binding);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            barcodeScanner.OnAppearing();
            _ViewModel.LoadStorageLocationsCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            barcodeScanner.OnDisappearing();
            base.OnDisappearing();
        }

        private async void btnCancel_Clicked(object sender, EventArgs e)
        {
            _ViewModel.SelectedStorageLocation = null;
            _ViewModel = null;
            await Navigation.PopAsync(true);
        }
         
        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PopAsync(true);
        }

#if ZXING
        private async void CameraScanTBItem_Clicked(object sender, EventArgs e)
#else
        private void CameraScanTBItem_Clicked(object sender, EventArgs e)
#endif

        {
            _ViewModel.FacilityScanViewModel.Clear();
            #if ZXING
            await barcodeScanner.OpenBarcodeCamera();
            #endif
        }

        public static readonly BindableProperty FlexLayoutItemsSourceProperty = BindableProperty.Create("FlexLayoutItemsSource", typeof(IEnumerable<object>), typeof(BSOFacilitySelector), propertyChanged: OnEventNameChanged);

        static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            BSOFacilitySelector thisObject = bindable as BSOFacilitySelector;
            if (thisObject != null)
            {
                thisObject.storageLocationPathLayout.Children.Clear();

                IEnumerable<object> items = newValue as IEnumerable<object>;
                if (items != null)
                {
                    foreach (object item in items)
                    {
                        Label entry = new Label();
                        entry.Text = item.ToString() + " > ";
                        entry.TextColor = Colors.White;
                        thisObject.storageLocationPathLayout.Children.Add(entry);
                    }
                }
            }
        }

        public IEnumerable<object> FlexLayoutItemsSource
        {
            get;
            set;
        }

        private void FacilitiesList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemsSource")
            {
                if (_ViewModel != null && _ViewModel.StorageLocations != null)
                {
                    object firstItem = _ViewModel.StorageLocations.FirstOrDefault();
                    if (firstItem != null)
                        FacilitiesList.ScrollTo(firstItem, ScrollToPosition.MakeVisible, false);
                }
            }
        }
    }
}