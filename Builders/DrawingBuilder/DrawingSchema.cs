using DSKPrim.PanelTools.ProjectEnvironment;
using System.Collections.Generic;

namespace DSKPrim.PanelTools.Builders
{
    public class DrawingSchema
    {
        private Dictionary<int, List<ViewTemplateName>> Schema;

        public DrawingSchema(Dictionary<int, List<ViewTemplateName>> keyValuePairs)
        {
            Schema = keyValuePairs;
        }

        public Dictionary<int, List<ViewTemplateName>> GetSchema()
        {
            return Schema;
        }
    }
}