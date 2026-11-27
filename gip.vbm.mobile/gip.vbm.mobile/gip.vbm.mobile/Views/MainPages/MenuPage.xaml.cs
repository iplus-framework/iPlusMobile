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
            // ItemTapped (not ItemSelected): taps fire on every selection, including
            // re-tapping the already-selected item. With ItemSelected, tapping the
            // item whose page is currently displayed (e.g. "About" right after login,
            // when AboutPage is the initial Detail) did nothing - the selection didn't
            // change and the flyout stayed open. Resetting SelectedItem allows the
            // same item to be tapped again later.
            ListViewMenu.ItemTapped += async (sender, e) =>
            {
                if (e.Item == null)
                    return;

                ListViewMenu.SelectedItem = null;
                VBMenuItem selectedMenu = (VBMenuItem)e.Item;
                await RootPage.NavigateFromMenu(selectedMenu);
            };
        }

        public void RefreshIcons()
        {
            //BuildNavList();
        }
    }
}