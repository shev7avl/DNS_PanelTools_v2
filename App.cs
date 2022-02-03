using Autodesk.Revit.UI;
using DSKPrim.PanelTools.GUI;

namespace DSKPrim.PanelTools
{
    class App : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {

            ButtonBuilder lod100Buttons = new PrecastLod100Builder(application);
            lod100Buttons.BuildRibbon();
            lod100Buttons.BuildButtons();

            ButtonBuilder lod400Buttons = new PrecastLod400Builder(application);
            lod400Buttons.BuildRibbon();
            lod400Buttons.BuildButtons();

            ButtonBuilder archButtons = new PrecastArchBuilder(application);
            archButtons.BuildRibbon();
            archButtons.BuildButtons();

            ButtonBuilder testButtons = new TestBuilder(application);
            testButtons.BuildRibbon();
            testButtons.BuildButtons();


            return Result.Succeeded;
        }

    }
}
