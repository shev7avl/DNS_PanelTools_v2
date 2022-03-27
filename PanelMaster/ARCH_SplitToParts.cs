using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.Facade;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ARCH_SplitToParts : IExternalCommand
    {
        Document Document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            Document = commandData.Application.ActiveUIDocument.Document;

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }
            AddinSettings settings = AddinSettings.GetSettings();
            Selector selector = new Selector();
            ICollection<Element> els = selector.CollectElements(commandData, new FacadeSelectionFilter(), BuiltInCategory.OST_Walls);

            try
            {
                foreach (var el in els)
                {
                    TileAlgorythm tileAlgorythm;
                    if (settings.GetTileSectionType() == TileSectionType.TILE_LAYOUT_STRAIGHT)
                    {
                        tileAlgorythm = new StraightAlgorythm(Document, el.Id);
                    }
                    else
                    {
                        tileAlgorythm = new BrickAlgorytm(Document, el.Id);
                    }

                    tileAlgorythm.Execute(Document);

                    Debug.WriteLine(el.Name);
                }
            }
            catch (Exception e)
            {
                message = $"Ошибка {e.Message} \n" +
                    $"{e.InnerException}" +
                    $"{e.Source}";
                Debug.WriteLine($"Ошибка {e.Message} \n" +
                    $"{e.InnerException}" +
                    $"{e.Source}" +
                    $"{e.StackTrace}");
                return Result.Failed;
            }

                         
            return Result.Succeeded;
        }
    }

    
}
