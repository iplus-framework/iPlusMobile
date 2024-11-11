// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.webservices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace gip.vbm.mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage
        {
            get
            {
                return Application.Current.MainPage  as MainPage;
            }
        }

        public MenuPage()
        {
            this.BindingContext = App.UserRights;
            InitializeComponent();

            //ListViewMenu.ItemsSource = RootPage.UserRights.Menu;
            //ListViewMenu.SelectedItem = RootPage.UserRights.Menu.FirstOrDefault();
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                VBMenuItem selectedMenu = (VBMenuItem)e.SelectedItem;
                await RootPage.NavigateFromMenu(selectedMenu);
            };
        }

        public void RefreshIcons()
        {
            //BuildNavList();
        }
    }
}