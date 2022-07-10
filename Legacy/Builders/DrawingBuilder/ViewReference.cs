using Autodesk.Revit.DB;
using DSKPrim.PanelTools.ProjectEnvironment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Builders
{
    public class ViewReference
    {
        public ViewTemplateName ViewTemplate;

        public ElementId TemplateId;

        private Dictionary<View, XYZ> ViewsAndCoords;

        public ViewReference(Document document, ViewTemplateName viewTemplate)
        {
            ViewsEnvironment views = ViewsEnvironment.getInstance(document);

            ViewTemplate = viewTemplate;
            TemplateId = views.GetElementIdFromViewTemplateName(ViewTemplate);
        }

        public Dictionary<View, XYZ> GetCreatedViewsWithCoords()
        {
            return ViewsAndCoords;
        }
        public void SetCreatedViews(View view)
        {
            if (ViewsAndCoords == null)
            {
                ViewsAndCoords = new Dictionary<View, XYZ>();
            }
            ViewsAndCoords.Add(view, SetViewInsertionPoint(ViewTemplate));
        }

        private XYZ SetViewInsertionPoint(ViewTemplateName templateName)
        {
            XYZ output;
            if (templateName.ToString().Contains("FRONT_VIEW"))
            {
                output = new XYZ(-0.938260407627828, 0.751429560594716, -0.2404);
            }
            else if (templateName.ToString().Contains("SECTION_VIEW"))
            {
                Random random = new Random(1);
                int rand = random.Next(2);
                if (rand == 1)
                {
                    output = new XYZ(-1.02859620179768, 0.32438762451903, -0.615920629921261);
                }
                else
                {
                    output = new XYZ(-0.199149364419911, 0.777492920174768, -0.276935433070868);
                }
                
            }
            else if (templateName.ToString().Contains("3D"))
            {
                output = new XYZ(-0.900722413820055, 0.601757777384431, 0.276008003285899);
            }
            else if (templateName.ToString().Contains("LEGEND"))
            {
                output = SetLegendInsertionPoint(templateName);

            }
            else if (templateName.ToString().Contains("SCHEDULE"))
            {
                output = SetScheduleInsertionPoint(templateName);
            }
            
            else
            {
                output = XYZ.Zero;
            }

            return output;
        }

        private static XYZ SetLegendInsertionPoint(ViewTemplateName templateName)
        {
            XYZ output;
            if (templateName == ViewTemplateName.LEGEND_FACADE_LAYOUT_SCHEME)
            {
                output = new XYZ(-1.09861879496965, 0.131817244347602, 0);
            }
            else if (templateName == ViewTemplateName.LEGEND_FACADE_DESCRIPTION)
            {
                output = new XYZ(-0.802193699667686, 0.136959664497014, 0);
            }
            else if (templateName == ViewTemplateName.LEGEND_FACADE_ANNOTATION)
            {
                output = new XYZ(-0.45817761978176, 0.210102477484447, 0);
            }
            else
            {
                output = new XYZ(-0.802193699667686, 0.228131596268927, 0);
            }

            return output;
        }

        private static XYZ SetScheduleInsertionPoint(ViewTemplateName templateName)
        {
            XYZ output;
            if (templateName == ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_MASS)
            {
                output = new XYZ(-0.822, 0.8413, 0);
            }
            else if (templateName == ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT)
            {
                output = new XYZ(-1.28132, 0.93059, 0);
            }
            else if (templateName == ViewTemplateName.SCHEDULE_SLAB_REINFORCEMENT)
            {
                output = new XYZ(-0.62336, 0.44361, 0);
            }
            else if (templateName == ViewTemplateName.SCHEDULE_SLAB_CONCRETE_VOLUME)
            {
                output = new XYZ(-0.62336, 0.29064, 0);
            }
            else if (templateName == ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_TYPES ||
                    templateName == ViewTemplateName.SCHEDULE_BALCONY_REINFORCEMENT_TYPES)
            {
                output = new XYZ(-0.62811, 0.93059, 0);
            }
            else if (templateName == ViewTemplateName.SCHEDULE_WALL_INSULATION_XPS)
            {
                output = new XYZ(-0.589073427320895, 0.280437531925653, 0);
            }
            else if (templateName == ViewTemplateName.SCHEDULE_WALL_INSULATION_WOOL)
            {
                output = new XYZ(-1.22883720684845, 0.280437531925655, 0);
            }
            else if (templateName == ViewTemplateName.SCHEDULE_BALCONY_CONCRETE_VOLUME ||
                templateName == ViewTemplateName.SCHEDULE_SLAB_CONCRETE_VOLUME ||
                templateName == ViewTemplateName.SCHEDULE_WALL_CONCRETE_VOLUME)
            {
                output = new XYZ(-0.609740381942169, 0.958005249343831, 0);
            }
            else if (templateName == ViewTemplateName.SCHEDULE_FACADE_DETAILS)
            {
                output = new XYZ(-1.31233595800525, 0.414485338711801, 0);
            }
            else if (templateName == ViewTemplateName.SCHEDULE_FACADE_DETAILS_TYPES)
            {
                output = new XYZ(-0.623359580052493, 0.414485338711801, 0);
            }
            else
            {
                output = new XYZ(-1.27797628823942, 0.256357547731719, 0);
            }

            return output;
        }
    }
}
