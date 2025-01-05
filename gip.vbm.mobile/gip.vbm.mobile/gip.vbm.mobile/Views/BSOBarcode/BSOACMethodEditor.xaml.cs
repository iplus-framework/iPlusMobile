using gip.core.datamodel;
using gip.vbm.mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.mes.webservices;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BSOACMethodEditor : BSOTabbedPageBase
    {
        ProdOrderPartslistWFInfo _ViewModel;

        public BSOACMethodEditor(ProdOrderPartslistWFInfo viewModel)
        {
            BindingContext = _ViewModel = viewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_ViewModel != null && _ViewModel.UserTime != null)
                _ViewModel.UserTime.UpdateTime();

            _UserChangedEndTime = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private bool _UserChangedEndTime = false;

        public short UserTimeMode
        {
            get
            {
                if (_ViewModel.WFMethod != null)
                {
                    ACValue userTimeModeParam = _ViewModel.WFMethod.ParameterValueList.GetACValue("AllowEditProgramLogTime");
                    if (userTimeModeParam != null)
                        return userTimeModeParam.ParamAsInt16;
                }
                return 0;
            }
        }

        private async void BtnApply_Clicked(object sender, EventArgs e)
        {
            if (_ViewModel != null && _ViewModel.UserTime != null)
            {
                _ViewModel.UserTime.UpdateDate(_UserChangedEndTime);
            }
            _ViewModel.WFMethod.AutoRemove = true;
            _ = await Navigation.PopAsync();
        }

        private void endDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            _UserChangedEndTime = true;
        }

        private void endTimePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TimePicker.Time))
            {
                _UserChangedEndTime = true;
            }
        }
    }
}