// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public class PickingsWorkplaceViewModel : PickingsViewModel
    {
        public PickingsWorkplaceViewModel() : base ()
        {
        }

        private List<Picking> _PickingItems;
        public List<Picking> PickingItems
        {
            get => _PickingItems;
            set
            {
                SetProperty(ref _PickingItems, value);
            }
        }

        private List<string> _GroupItems;
        public List<string> GroupItems
        {
            get => _GroupItems;
            set
            {
                SetProperty(ref _GroupItems, value);
            }
        }

        public ACClass RegisteredWorkplace
        {
            get;
            set;
        }

        private string _SelectedGroupItem;
        public string SelectedGroupItem
        {
            get => _SelectedGroupItem;
            set
            {
                SetProperty(ref _SelectedGroupItem, value);
                if (_SelectedGroupItem != null)
                {
                    PickingItems = Pickings.Where(c => c.GroupItem == _SelectedGroupItem && c.PickingType.PickingType == mes.datamodel.GlobalApp.PickingType.Issue).ToList();
                }
                else
                {
                    PickingItems = Pickings.Where(c => c.PickingType.PickingType == mes.datamodel.GlobalApp.PickingType.Issue).OrderBy(x => x.GroupItem).ToList();
                }
            }
        }

        public override void OnExecutedLoadPickings()
        {
            string temp = SelectedGroupItem;

            IEnumerable<string> groups = Pickings?.Where(x => x.GroupItem != null).Select(c => c.GroupItem).Distinct();
            if (groups != null && groups.Any())
            {
                GroupItems = groups.ToList();
            }

            PickingItems = Pickings.Where(c => c.PickingType.PickingType == mes.datamodel.GlobalApp.PickingType.Issue).OrderBy(x => x.GroupItem).ToList();

            SelectedGroupItem = temp;
        }
    }
}
