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
    class ARCH_SplitToParts : IExternalCommand
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
            //List<Element> list_Walls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Where(o => o.Name.Contains("DNS_")).ToList();

            Selection selection = commandData.Application.ActiveUIDocument.Selection;

            IList<Reference> list_Walls = selection.PickObjects(ObjectType.Element, new FacadeSelectionFilter(), "Выберите стены DNS_Фасад или DNS_Фасад2");

            AddinSettings settings = AddinSettings.GetSettings();

            Debug.WriteLine(Document.PathName);

            foreach (var reference in list_Walls)
            {
                Element item = Document.GetElement(reference.ElementId);
                try
                {
                    TileAlgorythm tileAlgorythm;
                    if (settings.GetTileSectionType() == TileSectionType.TILE_LAYOUT_STRAIGHT)
                    {
                        tileAlgorythm = new StraightAlgorythm(Document, reference.ElementId);
                    }
                    else
                    {
                         tileAlgorythm = new BrickAlgorytm(Document, reference.ElementId);
                    }
                    
                    tileAlgorythm.Execute(Document);

                    Utility.Parts.ExcludeStitches(Document, item);
                    Debug.WriteLine(item.Name);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Ошибка{e.Message} : {item.Name} - {item.Id}");
                }
            }
                         
            return Result.Succeeded;
        }
    }

    internal class FacadeSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {

            if (elem.GetType() == typeof(Wall) && elem.Name.Contains("DNS_Фасад"))
            {
                return true;
            }

            else return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
