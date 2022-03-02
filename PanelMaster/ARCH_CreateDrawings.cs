using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ARCH_CreateDrawings : IExternalCommand
    {
        public Document Document { get; set; }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            CommonProjectEnvironment environment = CommonProjectEnvironment.GetInstance(Document);

            Selector selector = new Selector();

            ICollection<Element> wallsCollection = selector.CollectElements(commandData, new FacadeSelectionFilter(), BuiltInCategory.OST_Walls);
            Transaction transaction = new Transaction(Document, "Создаем сборки");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            List<BasePanel> panels = new List<BasePanel>();
            foreach (var item in wallsCollection)
            {
                panels.Add(new Facade_Panel(Document, item));
            }

            CreateAssemblyIfMissing(panels, transaction);
            CreateDrawingForSelectedPanels(panels);

            return Result.Succeeded;
        }

        private void CreateDrawingForSelectedPanels(List<BasePanel> list_Panels)
        {
            foreach (BasePanel item in list_Panels)
            {
                BasePanelWrapper panelWrapper = new DrawingWrapper(item);
                panelWrapper.Execute(Document);
            }
        }

        private void CreateAssemblyIfMissing(List<BasePanel> list_Panels, Transaction transaction)
        {
            foreach (var item in list_Panels)
            {
                if (item.AssemblyInstance is null)
                {
                    item.AssemblyInstance = CreatePartAssembly(transaction, item.ActiveElement);
                }
            }
            //Пересохраняем коллектор
            if (Document.IsModified && Document.IsModifiable)
            {
                SubTransaction regeneration = new SubTransaction(Document);
                regeneration.Start();
                Document.Regenerate();
                regeneration.Commit();
            }
        }

        private void CreateFacadeAssembly(IList<Reference> list_Walls, out List<AssemblyInstance> assemblies, out Transaction transaction)
        {
            assemblies = new List<AssemblyInstance>();
            transaction = new Transaction(Document, "creating an assembly");
            TransactionSettings.SetFailuresPreprocessor(transaction);
            
        }



        private AssemblyInstance CreatePartAssembly(Transaction transaction, Element item)
        {

            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(new Outline(item.get_Geometry(new Options()).GetBoundingBox().Min, item.get_Geometry(new Options()).GetBoundingBox().Max));

            ICollection<ElementId> ids = new FilteredElementCollector(Document).OfClass(typeof(Part)).WhereElementIsNotElementType().WherePasses(boundingBoxIntersectsFilter).ToElementIds();

            List<Element> els = ids.Select(x => Document.GetElement(x)).Where(x => x.get_Parameter(BuiltInParameter.DPART_VOLUME_COMPUTED).AsDouble() < 1).ToList();

            ids = els.Select(x => x.Id).ToList();

            ICollection<ElementId> ids1 = new List<ElementId>() { ids.First() };

            ElementId partNamingCategory = els[0].Category.Id;

            AssemblyInstance assembly = null;

                if (AssemblyInstance.IsValidNamingCategory(Document, partNamingCategory, ids))
                {
                    transaction.Start();
                    assembly = AssemblyInstance.Create(Document, ids, partNamingCategory);
                    transaction.Commit();
                    if (transaction.GetStatus() == TransactionStatus.Committed)
                    {
                        transaction.Start();
                        string name = $"{item.get_Parameter(new Guid(Properties.Resource.ADSK_Марка_изделия)).AsString()} ({item.get_Parameter(new Guid(Properties.Resource.DNS_Марка_элемента)).AsString()})";
                        assembly.AssemblyTypeName = name;
                        transaction.Commit();
                    }
                }

            return assembly;
        }
    }
}
