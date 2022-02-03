using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSKPrim.PanelTools.ProjectEnvironment
{
    public class ViewsEnvironment: IResettable
    {

        private static ViewsEnvironment Instance;

        private Document Document;

        private ViewsEnvironment(Document document)
        {
            Document = document;
        }

        public static ViewsEnvironment getInstance(Document document)
        {
            if (Instance is null)
            {
                Instance = new ViewsEnvironment(document);
            }
            return Instance;
        }

        public void Dispose()
        {
            Instance = null;
        }

        public ElementId GetElementIdFromViewTemplateName(ViewTemplateName viewTemplateName)
        {
            Element temp = CheckIfTemplateExists(viewTemplateName);

            if (temp is null)
            {
                return null;
            }
            else
            {
                return temp.Id;
            }
        }

        private Element CheckIfTemplateExists(ViewTemplateName viewTemplateName)
        {
            ViewTemplateNamesEnumMapping.TryGetValue(viewTemplateName, out string tempNameString);
            if (tempNameString != null)
            {
                Type viewType = GetViewType(viewTemplateName);
                return new FilteredElementCollector(Document).OfClass(viewType).FirstOrDefault(o => o.Name.Contains(tempNameString));
            }
            else
            {
                return null;
            } 
        }

       

        private Type GetViewType(ViewTemplateName view)
        {
            if (view.ToString().Contains("SCHEDULE"))
            {
                return typeof(ViewSchedule);
            }
            else if (view.ToString().Contains("SHEET"))
            {
                return typeof(FamilySymbol);
            }
            else if (view.ToString().Contains("VIEW") && view.ToString().Contains("3D"))
            {
                return typeof(View3D);
            }
            else if (view.ToString().Contains("NOT_IMPLEMENTED"))
            {
                return null;
            }
            else
            {
                return typeof(View);
            }
        }

        public void Reset()
        {
            Document = null;
            Instance = null;
        }

        private readonly Dictionary<ViewTemplateName, string> ViewTemplateNamesEnumMapping = new Dictionary<ViewTemplateName, string>
        {
            //Sheets
            {ViewTemplateName.SHEET_FORM_4_TITLE, "Форма 4_чертил"},
            {ViewTemplateName.SHEET_FORM_6, "Форма 6"},
            //3D Views
            {ViewTemplateName.SLAB_3D_VIEW , "3D_Плита перекрытия"},
            {ViewTemplateName.PANEL_3D_VIEW , "3D_Стеновая панель"},
            //View sections
            {ViewTemplateName.FACADE_VIEW_FRONT_VIEW,"DNS_F_Плитка"},
            {ViewTemplateName.WALL_MESH_FRONT_VIEW,"DNS_КЖ_О_Р_Сетка арматурная"},
            {ViewTemplateName.WALL_MESH_INTERNAL_FRONT_VIEW,"DNS_КЖ_О_Р_Сетка арматурная_Внутренний слой"},
            {ViewTemplateName.WALL_MESH_OUTER_FRONT_VIEW,"DNS_КЖ_О_Р_Сетка арматурная_Наружный слой" },
            {ViewTemplateName.WALL_REBAR_FRONT_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Армирование"},
            {ViewTemplateName.WALL_REBAR_INTERNAL_FRONT_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Армирование_Внутренний слой"},
            {ViewTemplateName.WALL_REBAR_INTERNAL_SECTION_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Армирование_Внутренний слой_Сечение"},
            {ViewTemplateName.WALL_REBAR_OUTER_FRONT_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Армирование_Наружный слой"},
            {ViewTemplateName.WALL_REBAR_OUTER_SECTION_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Армирование_Наружный слой_Сечение"},
            {ViewTemplateName.WALL_REBAR_SECTION_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Армирование_Сечение"},
            {ViewTemplateName.WALL_JOINT_FRONT_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Бийски"},
            {ViewTemplateName.WALL_JOINT_SECTION_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Бийски_Сечение"},
            {ViewTemplateName.WALL_CASING_FRONT_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Опалубка"},
            {ViewTemplateName.WALL_CASING_SECTION_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Опалубка_Сечение"},
            {ViewTemplateName.WALL_INSULATION_FRONT_VIEW,"DNS_КЖ_О_Р_Стеновые панели_Утеплитель"},

            {ViewTemplateName.BALCONY_REBAR_FRONT_VIEW,  "DNS_КЖИ_О_Р_Плиты балконные_Армирование"},
            {ViewTemplateName.BALCONY_REBAR_SECTION_VIEW,  "DNS_КЖИ_О_Р_Плиты балконные_Армирование_Сечение"},
            {ViewTemplateName.BALCONY_CASING_PLAN_VIEW,  "DNS_КЖИ_О_Р_Плиты балконные_Опалубка"},
            {ViewTemplateName.BALCONY_CASING_SECTION_VIEW,  "DNS_КЖИ_О_Р_Плиты балконные_Опалубка_Сечение"},

            {ViewTemplateName.SLAB_PLAN_VIEW,  "П_Плита перекрытия"},
            {ViewTemplateName.SLAB_SECTION_VIEW,  "Р_Плита перекрытия"},

            //Legends
            {ViewTemplateName.LEGEND_ORIENTATION_ANNOTATION, "Обозначение_Символ ориентации"},
            {ViewTemplateName.LEGEND_WALL_OUTER_JOINT_ANNOTATION , "Обозначения_Бийски панелей НС"},
            {ViewTemplateName.LEGEND_WALL_OUTER_JOINT_DESCRIPTION,"Примечание_Бийски панелей НС" },

            {ViewTemplateName.LEGEND_WALL_OUTER_CASING_ANNOTATION,  "Примечание_НС_Лист 2"},
            {ViewTemplateName.LEGEND_WALL_OUTER_INSULATION_ANNOTATION,  "Примечание_НС_Лист 3"},
            {ViewTemplateName.LEGEND_WALL_OUTER_INSULATION_GRAPHIC_ANNOTATION,  "Обозначение_НС_Лист 3"},
            {ViewTemplateName.LEGEND_WALL_OUTER_REBAR_INNER_ANNOTATION,  "Примечание_НС_Лист 5"},
            {ViewTemplateName.LEGEND_WALL_OUTER_REBAR_OUTER_ANNOTATION,  "Примечание_НС_Лист 6"},
            {ViewTemplateName.LEGEND_WALL_OUTER_SCHEDULE_ANNOTATION,  "Примечание_НС_Лист 7"},
            {ViewTemplateName.LEGEND_WALL_OUTER_MESH_INNER_ANNOTATION,  "Примечание_НС_Лист 8"},
            {ViewTemplateName.LEGEND_WALL_OUTER_MESH_OUTER_ANNOTATION,  "Примечание_НС_Лист 9"},

            {ViewTemplateName.LEGEND_WALL_INNER_CASING_ANNOTATION,  "Примечание_ВС_Лист 2"},
            {ViewTemplateName.LEGEND_WALL_INNER_REBAR_ANNOTATION,  "Примечание_ВС_Лист 3"},
            {ViewTemplateName.LEGEND_WALL_INNER_MESH_ANNOTATION,  "Примечание_ВС_Лист 5"},

            {ViewTemplateName.LEGEND_BALCONY_CASING_ANNOTATION,  "Примечание_БП_Лист 2"},
            {ViewTemplateName.LEGEND_PARAPET_CASING_ANNOTATION,  "Примечание_ПС_Лист 2"},
            {ViewTemplateName.LEGEND_BEND_DETAILS_ANNOTATION,  "Эскиз_Деталей_Гнутых"},

            {ViewTemplateName.LEGEND_WALL_INNER_DESCRIPTION,"Обозначения_Панели стеновые внутренние"},
            {ViewTemplateName.LEGEND_WALL_OUTER_DESCRIPTION,  "Обозначения_Панели стеновые наружные"},
            {ViewTemplateName.LEGEND_PARAPET_DESCRIPTION,  "Обозначения_Панели стеновые парапетные"},
            {ViewTemplateName.LEGEND_WALL_INNER_ANNOTATION,  "Примечание_Панели стеновые внутренние"},
            {ViewTemplateName.LEGEND_WALL_OUTER_ANNOTATION,  "Примечание_Панели стеновые наружные"},
            {ViewTemplateName.LEGEND_PARAPET_ANNOTATION,  "Примечание_Панели стеновые парапетные"},

            {ViewTemplateName.LEGEND_FACADE_ANNOTATION,  "АКР примечание"},
            {ViewTemplateName.LEGEND_FACADE_LAYOUT_SCHEME,  "Схема раскладки плитки (рядовая панель)"},
            {ViewTemplateName.LEGEND_FACADE_DESCRIPTION,  "Условные обозначения"},

            //Schedules
            {ViewTemplateName.SCHEDULE_WALL_INSULATION_WOOL , "М_Минвата"},
            {ViewTemplateName.SCHEDULE_WALL_CONCRETE_VOLUME , "М_Панель стеновая"},

            {ViewTemplateName.SCHEDULE_BALCONY_CONCRETE_VOLUME , "М_Плита балконная"},

            {ViewTemplateName.SCHEDULE_SLAB_CONCRETE_VOLUME, "М_Плита перекрытия"},
            {ViewTemplateName.SCHEDULE_WALL_INSULATION_XPS,"М_Полистирол"},

            {ViewTemplateName.SCHEDULE_BALCONY_REINFORCEMENT, "СА_Балконная плита"},

            {ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_MASS,  "СА_Масса_Панель стеновая"},
            {ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT,  "СА_Панель стеновая"},
            {ViewTemplateName.SCHEDULE_SLAB_REINFORCEMENT,  "СА_Плита перекрытия"},
            {ViewTemplateName.SCHEDULE_WALL_MESH_REINFORCEMENT,  "СА_Сетка арматурная"},
            {ViewTemplateName.SCHEDULE_WALL_MESH_INTERNAL_REINFORCEMENT,  "СА_Сетка арматурная_Внутренний слой"},
            {ViewTemplateName.SCHEDULE_WALL_MESH_OUTER_REINFORCEMENT,  "СА_Сетка арматурная_Наружный слой"},

            {ViewTemplateName.SCHEDULE_BALCONY_CASING,  "СЗД_Балконная плита"},
            {ViewTemplateName.SCHEDULE_PARAPET_CASING,  "СЗД_Панель парапета"},
            {ViewTemplateName.SCHEDULE_WALL_CASING,  "СЗД_Панель стеновая"},

            {ViewTemplateName.SCHEDULE_BALCONY_REINFORCEMENT_TYPES,  "СТА_Балконная плита"},
            {ViewTemplateName.SCHEDULE_WALL_REINFORCEMENT_TYPES,  "СТА_Панель стеновая"},
            {ViewTemplateName.SCHEDULE_FACADE,  "DNS_Ф_Плитка"},
            {ViewTemplateName.SCHEDULE_FACADE_TYPES,  "DNS_Марки_Плитка"}
        };
    }
    public enum ViewTemplateName
    {
        NOT_IMPLEMENTED,

        SHEET_FORM_4_TITLE,
        SHEET_FORM_6,

        PANEL_3D_VIEW,
        SLAB_3D_VIEW,

        SLAB_PLAN_VIEW,
        SLAB_SECTION_VIEW,

        BALCONY_CASING_PLAN_VIEW,
        BALCONY_CASING_SECTION_VIEW,
        BALCONY_REBAR_FRONT_VIEW,
        BALCONY_REBAR_SECTION_VIEW,

        WALL_CASING_FRONT_VIEW,
        WALL_CASING_SECTION_VIEW,
        WALL_INSULATION_FRONT_VIEW,
        WALL_JOINT_FRONT_VIEW,
        WALL_JOINT_SECTION_VIEW,
        WALL_REBAR_FRONT_VIEW,
        WALL_REBAR_SECTION_VIEW,
        WALL_REBAR_INTERNAL_FRONT_VIEW,
        WALL_REBAR_INTERNAL_SECTION_VIEW,
        WALL_REBAR_OUTER_FRONT_VIEW,
        WALL_REBAR_OUTER_SECTION_VIEW,
        WALL_MESH_FRONT_VIEW,
        WALL_MESH_INTERNAL_FRONT_VIEW,
        WALL_MESH_OUTER_FRONT_VIEW,

        FACADE_VIEW_FRONT_VIEW,

        LEGEND_ORIENTATION_ANNOTATION,

        LEGEND_BALCONY_ANNOTATION,
        LEGEND_BALCONY_DESCRIPTION,
        LEGEND_BALCONY_CASING_ANNOTATION,

        LEGEND_PARAPET_ANNOTATION,
        LEGEND_PARAPET_DESCRIPTION,
        LEGEND_PARAPET_CASING_ANNOTATION,

        LEGEND_WALL_INNER_ANNOTATION,
        LEGEND_WALL_INNER_DESCRIPTION,
        LEGEND_WALL_INNER_CASING_ANNOTATION,
        LEGEND_WALL_INNER_REBAR_ANNOTATION,
        LEGEND_WALL_INNER_SCHEDULE_ANNOTATION,
        LEGEND_WALL_INNER_MESH_ANNOTATION,

        LEGEND_WALL_OUTER_ANNOTATION,
        LEGEND_WALL_OUTER_DESCRIPTION,
        LEGEND_WALL_OUTER_CASING_ANNOTATION,
        LEGEND_WALL_OUTER_INSULATION_GRAPHIC_ANNOTATION,
        LEGEND_WALL_OUTER_INSULATION_ANNOTATION,
        LEGEND_WALL_OUTER_JOINT_ANNOTATION,
        LEGEND_WALL_OUTER_JOINT_DESCRIPTION,
        LEGEND_WALL_OUTER_REBAR_INNER_ANNOTATION,
        LEGEND_WALL_OUTER_REBAR_OUTER_ANNOTATION,
        LEGEND_WALL_OUTER_SCHEDULE_ANNOTATION,
        LEGEND_WALL_OUTER_MESH_INNER_ANNOTATION,
        LEGEND_WALL_OUTER_MESH_OUTER_ANNOTATION,

        LEGEND_SLAB_ANNOTATION,

        LEGEND_BEND_DETAILS_ANNOTATION,

        LEGEND_FACADE_ANNOTATION,
        LEGEND_FACADE_LAYOUT_SCHEME,
        LEGEND_FACADE_DESCRIPTION,

        SCHEDULE_BALCONY_CASING,
        SCHEDULE_BALCONY_CONCRETE_VOLUME,
        SCHEDULE_BALCONY_REINFORCEMENT,
        SCHEDULE_BALCONY_REINFORCEMENT_TYPES,
        SCHEDULE_PARAPET_CASING,
        SCHEDULE_SLAB_CONCRETE_VOLUME,
        SCHEDULE_SLAB_REINFORCEMENT,
        SCHEDULE_WALL_CONCRETE_VOLUME,
        SCHEDULE_WALL_CASING,
        SCHEDULE_WALL_INSULATION_WOOL,
        SCHEDULE_WALL_INSULATION_XPS,
        SCHEDULE_WALL_JOINT,
        SCHEDULE_WALL_MESH_REINFORCEMENT,
        SCHEDULE_WALL_MESH_INTERNAL_REINFORCEMENT,
        SCHEDULE_WALL_MESH_OUTER_REINFORCEMENT,
        SCHEDULE_WALL_REINFORCEMENT,
        SCHEDULE_WALL_REINFORCEMENT_MASS,
        SCHEDULE_WALL_REINFORCEMENT_TYPES,
        SCHEDULE_FACADE,
        SCHEDULE_FACADE_TYPES,
    }

    
}
