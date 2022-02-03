using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ARCH_CreateD : IExternalCommand
    {
        public Document Document { get; set; }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            CommonProjectEnvironment environment = CommonProjectEnvironment.GetInstance(Document);

            IList<Reference> list_Walls = GetFacadeFromSelection(commandData);
            List<AssemblyInstance> assemblies;
            Transaction transaction;
            CreateFacadeAssembly(list_Walls, out assemblies, out transaction);

            transaction.Start();
            foreach (var item in assemblies)
            {
                List<ViewSheet> viewSheets = new List<ViewSheet>();

                Utility.SheetUtils.CreateSheets(Document, item.Id, 1, out viewSheets);
                Utility.SheetUtils.CreateFacadeDrawing(Document, item.Id, viewSheets[0]);
            }
            transaction.Commit();

            return Result.Succeeded;
        }

        private void CreateFacadeAssembly(IList<Reference> list_Walls, out List<AssemblyInstance> assemblies, out Transaction transaction)
        {
            assemblies = new List<AssemblyInstance>();
            transaction = new Transaction(Document, "creating an assembly");
            TransactionSettings.SetFailuresPreprocessor(transaction);
            using (transaction)
            {
                foreach (var reference in list_Walls)
                {
                    assemblies.Add(CreatePartAssembly(transaction, reference));
                }
            }
        }

        private static IList<Reference> GetFacadeFromSelection(ExternalCommandData commandData)
        {
            Selection selection = commandData.Application.ActiveUIDocument.Selection;

            IList<Reference> list_Walls = selection.PickObjects(ObjectType.Element, new FacadeSelectionFilter(), "Выберите стены DNS_Фасад или DNS_Фасад2");
            return list_Walls;
        }

        private AssemblyInstance CreatePartAssembly(Transaction transaction, Reference reference)
        {
            Element item = Document.GetElement(reference.ElementId);
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
