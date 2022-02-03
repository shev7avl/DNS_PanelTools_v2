using Autodesk.Revit.DB;
using DSKPrim.PanelTools.ProjectEnvironment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Builders
{
    public class TemplateFactory
    {
        public List<Sheet> Sheets { get; set; }

        public List<ViewReference> ViewReferences { get; set; }

        public DrawingSchema DrawingSchema;

        public TemplateFactory(DrawingSchema drawingSchema)
        {
            DrawingSchema = drawingSchema;
        }

        public Dictionary<Sheet, List<ViewReference>> GetDrawingSetMapping()
        {
            Dictionary<Sheet, List<ViewReference>> mappingDictionary = new Dictionary<Sheet, List<ViewReference>>();
            CreateMappingFromTemplate(mappingDictionary);

            return mappingDictionary;
        }

        public int GetNumberOfSheets()
        {
            return DrawingSchema.GetSchema().Keys.Max();
        }

        public List<ViewTemplateName> GetViewTemplatesList()
        {
            List<ViewTemplateName> viewTemplates = new List<ViewTemplateName>();

            foreach (int index in DrawingSchema.GetSchema().Keys)
            {
                foreach (ViewTemplateName templateName in DrawingSchema.GetSchema()[index])
                {
                    viewTemplates.Add(templateName);
                }
            }

            return viewTemplates;
        }

        private void CreateMappingFromTemplate(Dictionary<Sheet, List<ViewReference>> mappingDictionary)
        {
            foreach (Sheet sheet in Sheets)
            {
                List<ViewReference> tempRefs = new List<ViewReference>();
                foreach (var reference in ViewReferences)
                {
                    if (ViewIsOnSheet(sheet, reference))
                    {
                        tempRefs.Add(reference);
                    }
                }
                mappingDictionary.Add(sheet, tempRefs);
            }
        }

        private bool ViewIsOnSheet(Sheet sheet, ViewReference reference)
        {
            return DrawingSchema.GetSchema().ContainsKey(sheet.Index) && DrawingSchema.GetSchema()[sheet.Index].Contains(reference.ViewTemplate);
        }
    }
}
