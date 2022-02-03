using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class TestCommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SettingsGUI settingsGUI = new SettingsGUI();

            if (settingsGUI.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return settingsGUI.settingsResult;
            }
            string errorText = "Что - то пошло не так";
            message = errorText;
            return Result.Failed;
            
        }

        
    }
}
