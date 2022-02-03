using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Diagnostics;

namespace DSKPrim.PanelTools.PanelMaster
{


    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class Run : Autodesk.Revit.UI.IExternalCommand
    {
        private Routine RoutineBehaviour;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            
            SetRoutineBehaviour();
            Debug.WriteLine($"Выбрана задача {RoutineBehaviour}");
            Debug.WriteLine("Старт задачи");

            RoutineBehaviour.ExecuteRoutine(commandData);
            return Result.Succeeded;
        }

        private void SetRoutineBehaviour()
        {
            GUI.PanelWizard landing = new GUI.PanelWizard();
            if (landing.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RoutineBehaviour = landing.GetRoutine();
            }
            //Some code here that sets behaviour

        }
    }


}
