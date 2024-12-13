// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.webservices;
using gip.mes.webservices;
using gip.vbm.mobile.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : BSOPageBase
    {
        public LoginPage()
        {
            this.BindingContext = App.SettingsViewModel;
            InitializeComponent();

            usernameEntry.Text = App.SettingsViewModel.LastUser;
#if DEBUG
            passwordEntry.Text = App.SettingsViewModel.LastPassword;
#endif

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 2;
            tapGestureRecognizer.Tapped += (s, e) => {
                SettingsPanel.IsVisible = !SettingsPanel.IsVisible;
            };
            Image.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private Task<VBUserRights> _CurrentTask = null;
        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(passwordEntry.Text) || String.IsNullOrEmpty(usernameEntry.Text))
                return;
            if (_CurrentTask != null && !_CurrentTask.IsCompleted)
            {
                await DisplayAlert("Info", Strings.AppStrings.LoginRequestActive_Text, "OK");
                return;
            }

            var user = new VBUser(usernameEntry.Text, passwordEntry.Text);
            if (App.SettingsViewModel.UseMockDataStore)
                DependencyService.Register<VBMockService>();
            else
                DependencyService.Register<VBWebService>();

            IVBWebService webService = DependencyService.Get<IVBWebService>() as IVBWebService;
            ILoginService loginService = webService as ILoginService;
            if (webService == null || loginService == null)
            {
                await DisplayAlert("Error", "Service-Error", "OK");
                return;
            }

            VBUserRights userRights = null;
            try
            {
                loginService.ReloadSettings();
                _CurrentTask = loginService.Login(user);
                App.SettingsViewModel.IsBusy = true;
                userRights = await _CurrentTask;

                if (userRights != null)
                {
                    if (App.SettingsViewModel.LightTheme)
                        MyXResourceLoader.ChangeTheme(App.Current.Resources, App.SettingsViewModel.LightTheme);
                    App.SettingsViewModel.SelectedLanguage = userRights.Language;
                    App.SettingsViewModel.LastUser = usernameEntry.Text;
#if DEBUG
                    App.SettingsViewModel.LastPassword = passwordEntry.Text;
#endif
                    DependencyService.Get<ILocalize>().SetLocale(App.SettingsViewModel.SelectedLanguage);
                    userRights.Menu.ForEach(c => c.ResolveDestPage(this.GetType().Assembly, "gip.vbm.mobile.Views"));
                    userRights.Menu.RemoveAll(c => c.DestPage == null);
                    userRights.Menu.Add(new VBMenuItem() { PageClassName = typeof(AboutPage).Name, DestPage = typeof(AboutPage), Label = Strings.AppStrings.About_Label });
                    App.UserRights = userRights;
                    gip.core.datamodel.Translator.VBLanguageCode = userRights.Language;
                    gip.core.datamodel.Translator.DefaultVBLanguageCode = userRights.DefaultLanguage;
                    (App.Current as App).DisplayMainPage();
                }
                else
                {
                    await DisplayAlert("Error", "Login failed", "OK");
                    passwordEntry.Text = string.Empty;
                }
            }
            catch (OperationCanceledException)
            {
                await DisplayAlert("Error", Strings.AppStrings.LoginError_Text, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                passwordEntry.Text = string.Empty;
            }
            App.SettingsViewModel.IsBusy = false;
        }

        private double _LastWidth;
        private double _LastHeight;
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this._LastWidth || height != this._LastHeight)
            {
                _LastWidth = width;
                _LastHeight = height;
                MainGrid.RowDefinitions.Clear();
                MainGrid.ColumnDefinitions.Clear();
                if (width > height)
                {
                    MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    Grid.SetColumn(Image, 0);
                    Grid.SetRow(Image, 0);
                    Grid.SetColumn(Form, 1);
                    Grid.SetRow(Form, 0);
                    //outerStack.Orientation = StackOrientation.Horizontal;
                }
                else
                {
                    MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.SetColumn(Image, 0);
                    Grid.SetRow(Image, 0);
                    Grid.SetColumn(Form, 0);
                    Grid.SetRow(Form, 1);
                    //outerStack.Orientation = StackOrientation.Vertical;
                }
            }
        }

        private void UsernameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == "settings")
            {
                SettingsPanel.IsVisible = true;
            }
            else
            {
                SettingsPanel.IsVisible = false;
            }
        }
    }
}