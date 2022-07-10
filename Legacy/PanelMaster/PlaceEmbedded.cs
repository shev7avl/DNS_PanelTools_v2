using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSKPrim.PanelTools.GUI;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    [Regeneration(RegenerationOption.Manual)]
    internal class PlaceEmbedded : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            PlaceEmbeddedUI embeddedUI = new PlaceEmbeddedUI();

            if (embeddedUI.DialogResult == true)
            {
                return Result.Succeeded;
            }
            else
            {
                return Result.Cancelled;
            }
        }
    }
}
