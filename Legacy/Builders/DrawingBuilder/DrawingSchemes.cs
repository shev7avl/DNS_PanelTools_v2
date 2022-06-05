using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Builders
{
    internal static class DrawingSchemes
    {

        private static readonly DrawingSchema NS_SCHEMA = new DrawingSchema(new Dictionary<int, List<ViewTemplateName>>()
        {
            { 1, new List<ViewTemplateName>()
                {
                ViewTemplateName.PANEL_3D_VIEW,
                ViewTemplateName.LEGEND_WALL_OUTER_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_OUTER_DESCRIPTION,
                ViewTemplateName.SCHEDULE_WALL_CONCRETE_VOLUME} },
            { 2, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_CASING_FRONT_VIEW,
                ViewTemplateName.WALL_CASING_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_OUTER_CASING_ANNOTATION,
                ViewTemplateName.SCHEDULE_WALL_CASING,
                }},
            { 3, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_INSULATION_FRONT_VIEW,
                ViewTemplateName.LEGEND_WALL_OUTER_INSULATION_ANNOTATION,
                ViewTemplateName.SCHEDULE_WALL_INSULATION_WOOL,
                ViewTemplateName.SCHEDULE_WALL_INSULATION_XPS
                }},
            { 4, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_JOINT_FRONT_VIEW,
                ViewTemplateName.WALL_JOINT_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_OUTER_JOINT_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_OUTER_JOINT_DESCRIPTION,
                ViewTemplateName.SCHEDULE_WALL_JOINT
                }},
            { 5, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_REBAR_INTERNAL_FRONT_VIEW,
                ViewTemplateName.WALL_REBAR_INTERNAL_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_OUTER_REBAR_INNER_ANNOTATION
                }},
            { 6, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_REBAR_OUTER_FRONT_VIEW,
                ViewTemplateName.WALL_REBAR_OUTER_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_OUTER_REBAR_OUTER_ANNOTATION
                }},
            { 7, new List<ViewTemplateName>()
                {
                ViewTemplateName.LEGEND_WALL_OUTER_SCHEDULE_ANNOTATION,
                ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT,
                ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_MASS,
                ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_TYPES
                }},
            { 8, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_MESH_INTERNAL_FRONT_VIEW,
                ViewTemplateName.LEGEND_WALL_OUTER_MESH_INNER_ANNOTATION,
                ViewTemplateName.SCHEDULE_WALL_MESH_INTERNAL_REINFORCEMENT
                }},
            { 9, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_MESH_OUTER_FRONT_VIEW,
                ViewTemplateName.LEGEND_WALL_OUTER_MESH_OUTER_ANNOTATION,
                ViewTemplateName.SCHEDULE_WALL_MESH_OUTER_REINFORCEMENT
                }},
        });

        private static readonly DrawingSchema VS_SCHEMA = new DrawingSchema(new Dictionary<int, List<ViewTemplateName>>()
        {
            { 1, new List<ViewTemplateName>()
                {
                ViewTemplateName.PANEL_3D_VIEW,
                ViewTemplateName.LEGEND_WALL_INNER_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_INNER_DESCRIPTION,
                ViewTemplateName.SCHEDULE_WALL_CONCRETE_VOLUME} },
            { 2, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_CASING_FRONT_VIEW,
                ViewTemplateName.WALL_CASING_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_INNER_CASING_ANNOTATION,
                ViewTemplateName.SCHEDULE_WALL_CASING,
                }},
            { 3, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_REBAR_FRONT_VIEW,
                ViewTemplateName.WALL_REBAR_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_INNER_REBAR_ANNOTATION,
                }},
            { 4, new List<ViewTemplateName>()
                {
                ViewTemplateName.LEGEND_WALL_INNER_SCHEDULE_ANNOTATION,
                ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT,
                ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_MASS,
                ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_TYPES
                }},
            { 5, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_MESH_FRONT_VIEW,
                ViewTemplateName.LEGEND_WALL_INNER_MESH_ANNOTATION,
                ViewTemplateName.SCHEDULE_WALL_MESH_REINFORCEMENT
                }}
        });

        private static readonly DrawingSchema BP_SCHEMA = new DrawingSchema(new Dictionary<int, List<ViewTemplateName>>()
        {
            { 1, new List<ViewTemplateName>()
                {
                ViewTemplateName.PANEL_3D_VIEW,
                ViewTemplateName.LEGEND_BALCONY_ANNOTATION,
                ViewTemplateName.LEGEND_BALCONY_DESCRIPTION,
                ViewTemplateName.SCHEDULE_BALCONY_CONCRETE_VOLUME} },
            { 2, new List<ViewTemplateName>()
                {
                ViewTemplateName.BALCONY_CASING_PLAN_VIEW,
                ViewTemplateName.BALCONY_CASING_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION,
                ViewTemplateName.LEGEND_BALCONY_CASING_ANNOTATION,
                ViewTemplateName.SCHEDULE_BALCONY_CASING,
                }},
            { 3, new List<ViewTemplateName>()
                {
                ViewTemplateName.BALCONY_REBAR_FRONT_VIEW,
                ViewTemplateName.BALCONY_REBAR_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION,
                }},
            { 4, new List<ViewTemplateName>()
                {
                ViewTemplateName.SCHEDULE_BALCONY_REINFORCEMENT,
                ViewTemplateName.SCHEDULE_BALCONY_REINFORCEMENT_TYPES
                }}
        });

        private static readonly DrawingSchema PP_SCHEMA = new DrawingSchema(new Dictionary<int, List<ViewTemplateName>>()
        {
            { 1, new List<ViewTemplateName>()
                {
                ViewTemplateName.SLAB_3D_VIEW,
                ViewTemplateName.SLAB_PLAN_VIEW,
                ViewTemplateName.SLAB_SECTION_VIEW,
                ViewTemplateName.LEGEND_SLAB_ANNOTATION,
                ViewTemplateName.SCHEDULE_SLAB_CONCRETE_VOLUME,
                ViewTemplateName.SCHEDULE_SLAB_REINFORCEMENT} }
        });

        private static readonly DrawingSchema PS_SCHEMA = new DrawingSchema(new Dictionary<int, List<ViewTemplateName>>()
        {
            { 1, new List<ViewTemplateName>()
                {
                ViewTemplateName.PANEL_3D_VIEW,
                ViewTemplateName.LEGEND_PARAPET_ANNOTATION,
                ViewTemplateName.LEGEND_WALL_INNER_DESCRIPTION,
                ViewTemplateName.SCHEDULE_WALL_CONCRETE_VOLUME} },
            { 2, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_CASING_FRONT_VIEW,
                ViewTemplateName.WALL_CASING_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION,
                ViewTemplateName.LEGEND_PARAPET_CASING_ANNOTATION,
                ViewTemplateName.SCHEDULE_PARAPET_CASING,
                }},
            { 3, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_REBAR_FRONT_VIEW,
                ViewTemplateName.WALL_REBAR_SECTION_VIEW,
                ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION
                }},
            { 4, new List<ViewTemplateName>()
                {
                ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT,
                ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_MASS,
                ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_TYPES
                }},
            { 5, new List<ViewTemplateName>()
                {
                ViewTemplateName.WALL_MESH_FRONT_VIEW,
                ViewTemplateName.LEGEND_WALL_INNER_MESH_ANNOTATION,
                ViewTemplateName.SCHEDULE_WALL_MESH_REINFORCEMENT
                }}
        });

        private static readonly DrawingSchema FACADE_SCHEMA = new DrawingSchema(new Dictionary<int, List<ViewTemplateName>>() 
        {
            { 1, new List<ViewTemplateName>()
                {
                    ViewTemplateName.FACADE_VIEW_FRONT_VIEW,
                    ViewTemplateName.LEGEND_FACADE_ANNOTATION,
                    ViewTemplateName.LEGEND_FACADE_DESCRIPTION,
                    ViewTemplateName.LEGEND_FACADE_LAYOUT_SCHEME,
                    ViewTemplateName.SCHEDULE_FACADE_DETAILS,
                    ViewTemplateName.SCHEDULE_FACADE_DETAILS_TYPES
                } }
        });


        internal static DrawingSchema GetNsSchema()
        { 
            return NS_SCHEMA;
        }
        internal static DrawingSchema GetVsSchema()
        {
            return VS_SCHEMA;
        }
        internal static DrawingSchema GetBpSchema()
        {
            return BP_SCHEMA;
        }
        internal static DrawingSchema GetPpSchema()
        {
            return PP_SCHEMA;
        }
        internal static DrawingSchema GetPsSchema()
        {
            return PS_SCHEMA;
        }
        internal static DrawingSchema GetFacadeSchema()
        { return FACADE_SCHEMA; }
    }
}
