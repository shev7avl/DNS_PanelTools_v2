using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

			List<PrecastPanel> panels = new List<PrecastPanel>();
			foreach (var item in wallsCollection)
			{
				panels.Add(new PrecastPanel(Document, item));
			}

			CreateAssemblyIfMissing(panels, transaction);
			CreateDrawingForSelectedPanels(panels);

			return Result.Succeeded;
		}

		private void CreateDrawingForSelectedPanels(List<PrecastPanel> list_Panels)
		{
			foreach (PrecastPanel item in list_Panels)
			{
				BasePanelWrapper panelWrapper = new DrawingWrapper(item);
				panelWrapper.Execute(Document);
			}
		}

		private void CreateAssemblyIfMissing(List<PrecastPanel> list_Panels, Transaction transaction)
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

		private ElementId GetWallHostId(ElementId partId)
		{
			if (Document.GetElement(partId) is Part)
			{
				Part part = (Part)Document.GetElement(partId);
				if (Document.GetElement(part.GetSourceElementIds().First().HostElementId) is Wall)
				{
					return Document.GetElement(part.GetSourceElementIds().First().HostElementId).Id;
				}
				else if (Document.GetElement(part.GetSourceElementIds().First().HostElementId) is Part)
				{
					part = Document.GetElement(part.GetSourceElementIds().First().HostElementId) as Part;
					return part.GetSourceElementIds().First().HostElementId;
				}
				else
				{
					throw new ArgumentException("Element is not a Part");
				}
			}
			else
			{
				throw new ArgumentException("Element is not a Part");
			}
		}

		private AssemblyInstance CreatePartAssembly(Transaction transaction, Element item)
		{
			XYZ[] boundaries = new XYZ[2];
			boundaries[0] = item.get_Geometry(new Options()).GetBoundingBox().Min;
			boundaries[1] = item.get_Geometry(new Options()).GetBoundingBox().Max;

			BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(new Outline(boundaries[0], boundaries[1]));

			ICollection<Part> parts = new FilteredElementCollector(Document).
				OfClass(typeof(Part)).
				WhereElementIsNotElementType().
				WherePasses(boundingBoxIntersectsFilter).
				Cast<Part>().ToList();

			ICollection<ElementId> ids = new FilteredElementCollector(Document).
				OfClass(typeof(Part)).
				WhereElementIsNotElementType().
				WherePasses(boundingBoxIntersectsFilter).
				Where(
					o => o.IsValidObject &&
					o.AssemblyInstanceId.IntegerValue == -1 &&
					AssemblyInstance.AreElementsValidForAssembly(Document, new List<ElementId>{ o.Id}, ElementId.InvalidElementId)&&
					o.get_Parameter(BuiltInParameter.DPART_VOLUME_COMPUTED).AsDouble() < 0.4).
				Cast<Part>().
				Where(o => GetWallHostId(o.Id) == item.Id).
				Select(o => o.Id).ToList();		

			ElementId partNamingCategory = ids.Select(x => Document.GetElement(x)).First().Category.Id;

			AssemblyInstance assembly = null;

			if (AssemblyInstance.IsValidNamingCategory(Document, partNamingCategory, ids))
			{
				transaction.Start();
				assembly = AssemblyInstance.Create(Document, ids, partNamingCategory);
				transaction.Commit();

				try
				{
					if (transaction.GetStatus() == TransactionStatus.Committed)
					{
						transaction.Start();
						string name = SetPartAssemblyName(parts);
						assembly.AssemblyTypeName = name;
						transaction.Commit();
						transaction.Start();
						CopyMaterialCodes(parts);
						transaction.Commit();
					}
				}
				catch (Exception)
				{
					if (transaction.GetStatus() == TransactionStatus.Started)
					{
						transaction.Commit();
					}
					if (transaction.GetStatus() == TransactionStatus.Committed)
					{
						transaction.Start();
						string[] nameParts = SetPartAssemblyName(parts).Split('-');
						var index = Int32.Parse(nameParts[2]);

						string name = String.Format("{0}-{1}-{2}", nameParts[0], nameParts[1], index+1);
						assembly.AssemblyTypeName = name;
						transaction.Commit();
						transaction.Start();
						CopyMaterialCodes(parts);
						transaction.Commit();
					}
				}	
			}
			return assembly;
		}

		private string SetPartAssemblyName(ICollection<Part> parts)
		{
			string markName = parts.First().ParametersMap.get_Item("ADSK_Марка конструкции").AsString().Split('.')[0];

			string paintName = "";

			List<string> partNames = new List<string>();

			foreach (var item in parts)
			{
				Element material = Document.GetElement(item.ParametersMap.get_Item("Материал").AsElementId());
				if (material != null)
				{
					if (material.ParametersMap.get_Item("ADSK_Наименование и номер цвета").AsString() != null)
					{
						string code = material.ParametersMap.get_Item("ADSK_Наименование и номер цвета").AsString();
						string name = code.Substring(code.Length - 2, 2);
						partNames.Add(name);
					}  
				}
			}
			if (partNames.Distinct().Count() > 0)
			{
				paintName= String.Join(".", partNames.Distinct());
			}
			else paintName= $"ID{parts.First().Id}";

			return String.Format("{0}-{1}-1", markName, paintName);
		}

		private void CopyMaterialCodes(ICollection<Part> parts)
		{
			foreach (var item in parts)
			{
				Element material = Document.GetElement(item.ParametersMap.get_Item("Материал").AsElementId());
				if (material != null)
				{
					if (material.ParametersMap.get_Item("ADSK_Наименование и номер цвета").AsString() != null)
					{
						string code = material.ParametersMap.get_Item("ADSK_Наименование и номер цвета").AsString();
						string name = code.Substring(code.Length - 2, 2);
						item.ParametersMap.get_Item("DNS_Номер цвета плитки").Set(name);
						item.ParametersMap.get_Item("ADSK_Позиция").Set(code);
					}
				}
			}
		}
	}
}
