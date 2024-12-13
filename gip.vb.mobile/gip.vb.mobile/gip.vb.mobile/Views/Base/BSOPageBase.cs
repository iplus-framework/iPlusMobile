// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.vb.mobile.DependencyServices;
using gip.vb.mobile.Strings;
using System;
using Xamarin.Forms;

namespace gip.vb.mobile.Views
{
    public class BSOPageBase : ContentPage, IBSOPage
    {
        public BSOPageBase()
        {
            IconImageSource = null;
        }

        ViewModels.BaseViewModel _BaseViewModel = null;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext != null)
            {
                _BaseViewModel = BindingContext as ViewModels.BaseViewModel;
                if (_BaseViewModel != null)
                    _BaseViewModel.DialogEvent += _BaseViewModel_DialogEvent;
            }
        }

        private async void _BaseViewModel_DialogEvent(object sender, Controls.EventArgs<core.datamodel.Msg> e)
        {
            core.datamodel.Msg msg = e.Value;

            if (msg != null)
            {
                string message = msg.Message;
                core.datamodel.MsgWithDetails msgDetail = msg as core.datamodel.MsgWithDetails;

                if (msgDetail != null)
                {
                    foreach (var m in msgDetail.MsgDetails)
                    {
                        message += System.Environment.NewLine;
                        message += m.Message;
                    }
                }

                if (msg.MessageLevel == core.datamodel.eMsgLevel.Question)
                {
                    core.datamodel.Global.MsgResult result = core.datamodel.Global.MsgResult.Cancel;

                    switch (msg.MessageButton)
                    {
                        case core.datamodel.eMsgButton.Cancel:
                            {
                                await DisplayAlert(_BaseViewModel.DialogOptions.DialogTitle, message, AppStrings.ButtonCancel);
                                break;
                            }
                        case core.datamodel.eMsgButton.OK:
                            {
                                await DisplayAlert(_BaseViewModel.DialogOptions.DialogTitle, message, "OK");
                                break;
                            }
                        case core.datamodel.eMsgButton.OKCancel:
                            {
                                bool res = await DisplayAlert(_BaseViewModel.DialogOptions.DialogTitle, message, "OK", AppStrings.ButtonCancel);
                                result = res ? core.datamodel.Global.MsgResult.OK : core.datamodel.Global.MsgResult.Cancel;
                                break;
                            }
                        case core.datamodel.eMsgButton.YesNo:
                            {
                                bool res = await DisplayAlert(_BaseViewModel.DialogOptions.DialogTitle, message, AppStrings.ButtonYes, AppStrings.ButtonNo);
                                result = res ? core.datamodel.Global.MsgResult.Yes : core.datamodel.Global.MsgResult.No;
                                break;
                            }
                        case core.datamodel.eMsgButton.YesNoCancel:
                            {
                                string res = await DisplayActionSheet(message, AppStrings.ButtonCancel, null, AppStrings.ButtonYes, AppStrings.ButtonNo);
                                if (res == AppStrings.ButtonCancel)
                                    result = core.datamodel.Global.MsgResult.Cancel;
                                else if (res == AppStrings.ButtonYes)
                                    result = core.datamodel.Global.MsgResult.Yes;
                                else
                                    result = core.datamodel.Global.MsgResult.No;

                                break;
                            }
                    }

                    if (_BaseViewModel != null)
                        _BaseViewModel.DialogResponse(result);
                    else
                    {
                        //TODO:Error
                    }
                }
                else if (msg.MessageLevel == core.datamodel.eMsgLevel.QuestionPrompt)
                {
                    if (_BaseViewModel != null)
                    {
                        string result = await DisplayPromptAsync(_BaseViewModel.DialogOptions.DialogTitle, message, "OK", AppStrings.ButtonCancel, null, -1, 
                                                                 _BaseViewModel.DialogOptions.DialogPrompKeyboard, _BaseViewModel.DialogOptions.DialogPromptInitialValue);
                        if (_BaseViewModel != null)
                            _BaseViewModel.DialogResponse(result == null ? core.datamodel.Global.MsgResult.Cancel : core.datamodel.Global.MsgResult.OK, result);
                    }
                    else
                    {
                        //TODO:Error
                    }
                }
                else
                {
                    await DisplayAlert("", message, "OK");
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_BaseViewModel != null)
            {
                _BaseViewModel.DialogEvent -= _BaseViewModel_DialogEvent;
                _BaseViewModel = null;
            }
            else
            {
                ViewModels.BaseViewModel baseModel = BindingContext as ViewModels.BaseViewModel;
                if (baseModel != null)
                    baseModel.DialogEvent -= _BaseViewModel_DialogEvent;
            }
        }

        public virtual void OnNavigatedTo(NavigationEventArgs e, NavigationMode navigationMode)
        {
        }

        public virtual void OnNavigatedFrom(NavigationEventArgs e, NavigationMode navigationMode)
        {
        }


        public NavParameter NavParam
        {
            get; set;
        }

        public static readonly BindableProperty PageStateProperty = BindableProperty.Create("PageState", typeof(PageStateEnum), typeof(BSOPageBase), PageStateEnum.View);
        public PageStateEnum PageState
        {
            get
            {
                return (PageStateEnum)GetValue(PageStateProperty);
            }
            set
            {
                SetValue(PageStateProperty, value);
                OnPageStateChanged();
            }
        }

        protected virtual void OnPageStateChanged()
        {
        }

        private DateTime _LastExitTap;
        private TimeSpan _ExitTapTimeout = TimeSpan.FromSeconds(2.5);

        protected virtual void ExitOnBackButtonPressed()
        {
            DateTime tap = DateTime.Now;
            TimeSpan elapsedTime = tap - _LastExitTap;
            _LastExitTap = tap;
            IUtility utilityService = DependencyService.Get<IUtility>();
            if (utilityService != null)
            {
                if (elapsedTime > _ExitTapTimeout)
                {
                    utilityService.ShortAlert(Strings.AppStrings.ExitHint_Text);
                }
                else
                {
                    utilityService.CloseApp();
                }
            }
        }
    }
}
