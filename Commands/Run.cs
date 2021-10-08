using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Threading;

namespace DSKPrim.PanelTools_v2.Commands
{


    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class Run : IExternalCommand
    {
        private Routine RoutineBehaviour;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            SetRoutineBehaviour();

                RoutineBehaviour.ExecuteRoutine(commandData);
                return Result.Succeeded;

                //return Result.Cancelled;

            

            

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
