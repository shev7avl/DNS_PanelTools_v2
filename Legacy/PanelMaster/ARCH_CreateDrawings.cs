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

			List<BasePanel> panels = wallsCollection.Select(o => new Facade_Panel(o))
				.Cast<BasePanel>()
				.ToList();
			
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
					item.AssemblyInstance = CreateAssembly(transaction, item.ActiveElement);

					

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

		private AssemblyInstance CreateAssembly(Transaction transaction, Element item)
		{
			var map = item.ParametersMap;

			string constrMark = map.get_Item("ADSK_Марка конструкции").AsString();
			string locMark = map.get_Item("DNS_Марка элемента").AsString();
			string longMark = map.get_Item("DNS_Код изделия полный").AsString();

			var wallBox = item.get_Geometry(new Options()).GetBoundingBox();
			var newBox = new BoundingBoxXYZ
			{
				Max = new XYZ(
					x: wallBox.Max.X + 5,
					y: wallBox.Max.Y + 5,
					z: wallBox.Max.Z + 5),
				Min = new XYZ(
					x: wallBox.Min.X - 5,
					y: wallBox.Min.Y - 5,
					z: wallBox.Min.Z - 5),
				Transform = wallBox.Transform

			};

			BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(new Outline(
				newBox.Min, newBox.Max));


			ICollection<Part> parts = new FilteredElementCollector(Document).
				OfClass(typeof(Part)).
				WherePasses(filter).
				Where(o => 
					o.ParametersMap.get_Item("ADSK_Марка конструкции").AsString() == constrMark &&
					o.ParametersMap.get_Item("DNS_Марка элемента").AsString() == locMark &&
					o.ParametersMap.get_Item("DNS_Код изделия полный").AsString() == longMark &&
					o.IsValidObject &&
					o.AssemblyInstanceId.IntegerValue == -1 &&
					AssemblyInstance.AreElementsValidForAssembly(Document, new List<ElementId> { o.Id }, ElementId.InvalidElementId) &&
					o.get_Parameter(BuiltInParameter.DPART_VOLUME_COMPUTED).AsDouble() < 0.4).
				Cast<Part>().ToList();

			var ids = parts.Select(o => o.Id).ToList();

			ElementId partNamingCategory = ids.Select(x => Document.GetElement(x)).First().Category.Id;

			AssemblyInstance assembly = null;

			if (AssemblyInstance.IsValidNamingCategory(Document, partNamingCategory, ids))
			{
				transaction.Start();
				assembly = AssemblyInstance.Create(Document, ids, partNamingCategory);
				
				transaction.Commit();

				//try
				//{
					
				if (transaction.GetStatus() == TransactionStatus.Committed)
					{
						transaction.Start();
						string name = SetPartAssemblyName(parts);
						assembly.AssemblyTypeName = name;
						transaction.Commit();
						transaction.Start();
						CopyMaterialCodes(parts, name);
						transaction.Commit();
					}
				//}
				//catch (Exception)
				//{
				//	if (transaction.GetStatus() == TransactionStatus.Started)
				//	{
				//		transaction.Commit();
				//	}
				//	if (transaction.GetStatus() == TransactionStatus.Committed)
				//	{
				//		transaction.Start();
				//		string name = SetPartAssemblyName(parts);
				//		var newName = FindMaxAssemblyIndex(name);
						
				//		assembly.AssemblyTypeName = newName;
				//		transaction.Commit();
				//		transaction.Start();
				//		CopyMaterialCodes(parts, newName);
				//		transaction.Commit();
				//	}
				//}
			}
			return assembly;

		}

		private string FindMaxAssemblyIndex(string name)
		{

			var mainCode = $"{name.Split('-')[0]}-{name.Split('-')[1]}";
		List<string> names = new FilteredElementCollector(Document).
				OfClass(typeof(AssemblyType)).
				Select(x => x.Name).
				Where(x => x.Contains(mainCode)).Distinct().ToList();
            if (names.Count == 0)
            {
				return $"{mainCode}-1";

			}
			List<int> indices = 
				names.Select(o => int.Parse(o.Split('-')[2])).
				ToList();

			return $"{mainCode}-{indices.Max()+1}";

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

			var resName = String.Format("{0}-{1}-1", markName, paintName);

			return FindMaxAssemblyIndex(resName);
		}

		private void CopyMaterialCodes(ICollection<Part> parts, string colorCode)
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
						item.ParametersMap.get_Item("DNS_Код фасада").Set(colorCode.Split('-')[1]);
					}
				}
			}
		}
	}
}
