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

        public ARCH_SplitToParts()
        { }

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

            //List<Element> list_Walls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Where(o => o.Name.Contains("DNS_")).ToList();

            //Selection selection = commandData.Application.ActiveUIDocument.Selection;
            //IList<Reference> list_Walls = selection.PickObjects(ObjectType.Element, new FacadeSelectionFilter(), "Выберите стены DNS_Фасад или DNS_Фасад2");

            

            Debug.WriteLine(Document.PathName);
            try
            {
                foreach (var el in els)
                {
                    //Element item = Document.GetElement(reference.ElementId);



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
