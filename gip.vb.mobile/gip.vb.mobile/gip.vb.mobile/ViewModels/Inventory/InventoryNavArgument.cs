using gip.mes.webservices;

namespace gip.vb.mobile.ViewModels.Inventory
{
    public class InventoryNavArgument
    {

        #region ctor's
        public InventoryNavArgument()
        {

        }


        #endregion

        #region Properties

        public string FacilityInventoryNo { get; set; }

        public EditModeEnum EditMode { get; set; }

        public bool IsValidateAndComplete { get; set; }

        public Facility SelectedStorageLocation { get; set; }

        public Facility SelectedFacility { get; set; }

        public FacilityInventoryPos SelectedInventoryLine { get; set; }
        #endregion

    }
}
