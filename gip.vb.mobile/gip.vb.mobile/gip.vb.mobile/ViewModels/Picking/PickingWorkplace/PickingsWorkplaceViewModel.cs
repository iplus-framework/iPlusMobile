using gip.core.webservices;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    PickingItems = Pickings.Where(c => c.GroupItem == _SelectedGroupItem).ToList();
                }
                else
                {
                    PickingItems = Pickings.OrderBy(x => x.GroupItem).ToList();
                }
            }
        }

        public override void OnExecutedLoadPickings()
        {
            IEnumerable<string> groups = Pickings?.Where(x => x.GroupItem != null).Select(c => c.GroupItem).Distinct();
            if (groups != null && groups.Any())
            {
                GroupItems = groups.ToList();
            }

            PickingItems = Pickings.OrderBy(x => x.GroupItem).ToList();
        }
    }
}
