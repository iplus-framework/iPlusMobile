using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.ViewModels
{
    public interface IMaterialUnitRecalcReceiver
    {
        void SetQuantityFromUnitRecalc(double newValue);
    }

    public class MaterialUnitCalcModel : BaseViewModel
    {
        #region ctor's
        public MaterialUnitCalcModel(IMaterialUnitRecalcReceiver receiver)
        {
            _UnitReceiver = receiver;
            GetMaterialUnitsCommand = new Command(async () => await GetMaterialUnits());
            MaterialConvertUnitCommand = new Command(async () => await MaterialConvertUnit());
            MaterialConvertAllUnitsCommand = new Command(async () => await MaterialConvertAllUnits());
        }
        #endregion

        #region Overrides

        public override void DialogResponse(Global.MsgResult result, string entredValue = null)
        {

        }
        #endregion

        #region Commands
        public Command GetMaterialUnitsCommand { get; private set; }
        public Command MaterialConvertUnitCommand { get; private set; }
        public Command MaterialConvertAllUnitsCommand { get; private set; }

        #endregion

        #region Params
        #endregion

        #region Data
        private Material _MaterialToCalc;
        public Material MaterialToCalc
        {
            get
            {
                return _MaterialToCalc;
            }
            set
            {
                SetProperty(ref _MaterialToCalc, value);
                Title = _MaterialToCalc != null ? _MaterialToCalc.MaterialName1 : "Unit";
            }
        }

        private List<MDUnit> _Units;
        public List<MDUnit> Units
        {
            get
            {
                return _Units;
            }
            set
            {
                SetProperty(ref _Units, value);
            }
        }

        private MDUnit _SelectedUnit;
        public MDUnit SelectedUnit
        {
            get
            {
                return _SelectedUnit;
            }
            set
            {
                SetProperty(ref _SelectedUnit, value);
            }
        }

        private List<MDUnitCalc> _UnitCalcs;
        public List<MDUnitCalc> UnitCalcs
        {
            get
            {
                return _UnitCalcs;
            }
            set
            {
                SetProperty(ref _UnitCalcs, value);
            }
        }

        private MDUnitCalc _SelectedUnitCalc;
        public MDUnitCalc SelectedUnitCalc
        {
            get
            {
                return _SelectedUnitCalc;
            }
            set
            {
                SetProperty(ref _SelectedUnitCalc, value);
            }
        }

        private MDUnitCalc _CalcObj;
        public MDUnitCalc CalcObj
        {
            get
            {
                return _CalcObj;
            }
            set
            {
                SetProperty(ref _CalcObj, value);
            }
        }

        private double _InputValue;
        public double InputValue
        {
            get
            {
                return _InputValue;
            }
            set
            {
                SetProperty(ref _InputValue, value);
            }
        }

        private IMaterialUnitRecalcReceiver _UnitReceiver;
        public IMaterialUnitRecalcReceiver UnitReceiver
        {
            get
            {
                return _UnitReceiver;
            }
        }
        #endregion

        #region Tasks
        public async Task GetMaterialUnits()
        {
            if (IsBusy || MaterialToCalc == null)
                return;

            IsBusy = true;

            try
            {
                var response = await _WebService.GetMaterialUnitsAsync(MaterialToCalc.MaterialID.ToString());
                this.WSResponse = response;
                if (response.Suceeded)
                    Units = response.Data;
                else
                    Units = new List<MDUnit>();
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task MaterialConvertUnit()
        {
            if (IsBusy || MaterialToCalc == null || SelectedUnit == null)
                return;

            IsBusy = true;

            try
            {
                CalcObj = new MDUnitCalc();
                CalcObj.MaterialID = MaterialToCalc.MaterialID;
                CalcObj.Unit = SelectedUnit;
                CalcObj.InputValue = InputValue;

                var response = await _WebService.MaterialConvertUnitAsync(CalcObj);
                this.WSResponse = response;
                if (response.Suceeded)
                    CalcObj = response.Data;
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }


        public async Task MaterialConvertAllUnits()
        {
            if (IsBusy || MaterialToCalc == null)
                return;

            IsBusy = true;

            try
            {
                CalcObj = new MDUnitCalc();
                CalcObj.MaterialID = MaterialToCalc.MaterialID;
                CalcObj.Unit = SelectedUnit;
                CalcObj.InputValue = InputValue;

                var response = await _WebService.MaterialConvertAllUnitsAsync(CalcObj);
                this.WSResponse = response;
                if (response.Suceeded)
                    UnitCalcs = response.Data;
            }
            catch (Exception ex)
            {
                Message = new core.datamodel.Msg(core.datamodel.eMsgLevel.Exception, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
