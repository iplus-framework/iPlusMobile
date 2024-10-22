using gip.mes.webservices;
using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.core.datamodel;

namespace gip.vbm.mobile.Helpers
{
    public class BarcodeEntityTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Unknown { get; set; }
        public DataTemplate ACClass { get; set; }
        public DataTemplate Material { get; set; }
        public DataTemplate FacilityLot { get; set; }
        public DataTemplate Facility { get; set; }
        public DataTemplate FacilityCharge { get; set; }
        public DataTemplate Picking { get; set; }
        public DataTemplate PickingPos { get; set; }
        public DataTemplate ProdOrderPartslist { get; set; }
        public DataTemplate ProdOrderPartslistPos{ get; set; }
        public DataTemplate ProdOrderPartslistWFInfo { get; set; }
        public DataTemplate WFMethod { get; set; }
        public DataTemplate Command { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            BarcodeEntity entity = item as BarcodeEntity;
            if (entity != null)
            {
                if (entity.ACClass != null)
                    return ACClass;
                else if (entity.Material != null)
                    return Material;
                else if (entity.FacilityLot != null)
                    return FacilityLot;
                else if (entity.Facility != null)
                    return Facility;
                else if (entity.FacilityCharge != null)
                    return FacilityCharge;
                else if (entity.Picking != null)
                    return Picking;
                else if (entity.PickingPos != null)
                    return PickingPos;
                else if (entity.SelectedOrderWF != null)
                    return ProdOrderPartslistWFInfo;
                else if (entity.WFMethod != null)
                    return WFMethod;
                else if (entity.Command != null)
                    return Command;
            }
            else if (item is core.webservices.ACClass)
                return ACClass;
            else if (item is Material)
                return Material;
            else if (item is FacilityLot)
                return FacilityLot;
            else if (item is Facility)
                return Facility;
            else if (item is FacilityCharge)
                return FacilityCharge;
            else if (item is Picking)
                return Picking;
            else if (item is PickingPos)
                return PickingPos;
            else if (item is ProdOrderPartslist)
                return ProdOrderPartslist;
            else if (item is ProdOrderPartslistPos)
                return ProdOrderPartslistPos;
            else if (item is ProdOrderPartslistWFInfo)
                return ProdOrderPartslistWFInfo;
            else if (item is ACMethod)
                return WFMethod;
            else if (item is BarcodeEntityCommand)
                return Command;

            return Unknown;
        }
    }
}
