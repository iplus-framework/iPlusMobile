using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels.Inventory
{
    public class InventoryLinesModel : BaseViewModel
    {
        #region ctor's
        public InventoryLinesModel()
        {
            // Commands
            GetOpenLinesCommand = new Command(async () => await ExecuteGetOpenLines());
        }
        #endregion

        #region Overrides

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }
        #endregion

        #region Commands
        public Command GetOpenLinesCommand { get; private set; }

        #endregion

        #region Params
        private string _FacilityInventoryNo;
        public string FacilityInventoryNo
        {
            get
            {
                return _FacilityInventoryNo;
            }
            set
            {
                SetProperty(ref _FacilityInventoryNo, value);
            }
        }

        private Facility _SelectedStorageLocation;
        public Facility SelectedStorageLocation
        {
            get
            {
                return _SelectedStorageLocation;
            }
            set
            {
                SetProperty(ref _SelectedStorageLocation, value);
            }
        }

        private Facility _SelectedFacility;
        public Facility SelectedFacility
        {
            get
            {
                return _SelectedFacility;
            }
            set
            {
                SetProperty(ref _SelectedFacility, value);
            }
        }
        #endregion

        #region Data
        private List<FacilityInventoryPos> _OpenLines;
        public List<FacilityInventoryPos> OpenLines
        {
            get
            {
                return _OpenLines;
            }
            set
            {
                SetProperty(ref _OpenLines, value);
            }
        }

        private FacilityInventoryPos _SelectedLine;
        public FacilityInventoryPos SelectedLine
        {
            get
            {
                return _SelectedLine;
            }
            set
            {
                SetProperty(ref _SelectedLine, value);
            }
        }
        #endregion

        #region Tasks

        /// <summary>
        /// Calling GetFacilityInventoriesAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteGetOpenLines()
        {
            bool success = false;
            Message = null;
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    WSResponse<List<FacilityInventoryPos>> wSResponse =
                        await _WebService.GetFacilityInventoryPosesAsync(
                            FacilityInventoryNo,
                            "",
                            SelectedFacility?.FacilityNo,
                            "",
                            "",
                            "",
                            "",
                            "");
                    OpenLines = wSResponse.Data;
                    WSResponse = wSResponse;
                    success = wSResponse.Suceeded;
                }
                catch (Exception ex)
                {
                    Message = new Msg(eMsgLevel.Exception, ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
            return success;
        }
        #endregion
    }
}
