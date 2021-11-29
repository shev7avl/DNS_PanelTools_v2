using Autodesk.Revit.DB;
using DSKPrim.PanelTools_v2.StructuralApps.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.Utility
{
    public static class Sheets
    {

        public static void CreateSheets(Document document, ElementId id, int number, out List<ViewSheet> sheets)
        {
            ViewsSingleton views = ViewsSingleton.getInstance(document);
            sheets = new List<ViewSheet>();
            Element title_F4 = default;
            Element title_F6 = default;

            foreach (var item in views.viewReferences)
            {
                if (item.Name == "Форма 4_чертил" || item.Name == "Форма 4")
                {
                    title_F4 = item.viewTemplate;
                }
                if (item.Name == "Форма 6")
                {
                    title_F6 = item.viewTemplate;
                }
            }

            for (int i = 0; i < number; i++)
            {
                ElementId curTempId;
                if (i == 0)
                {
                    curTempId = title_F4.Id;
                }
                else
                {
                    curTempId = title_F6.Id;
                }

                ViewSheet sheet = AssemblyViewUtils.CreateSheet(document, id, curTempId);
                sheets.Add(sheet);
            }
        }

        public static void Create3DSheet(Document document, ElementId id, ViewSheet sheet)
        {
            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId view3D = default;
            ElementId schedule = default;
            ElementId leg1 = default;
            ElementId leg2 = default;

            foreach (var item in views.viewReferences)
            {
                switch (item.Name)
                {
                    case "3D_Стеновая панель":
                        view3D = item.viewTemplate.Id;
                        break;
                    case "Обозначения_Панели стеновые наружные":
                        if (item.viewTemplate != null)
                        {
                            leg1 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg1 = null;
                        }
                        break;
                    case "Примечание_Панели стеновые наружные":
                        if (item.viewTemplate != null)
                        {
                            leg2 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg2 = null;
                        }
                        break;
                    case "М_Панель стеновая":
                        schedule = item.viewTemplate.Id;
                        break;
                }
            }

            View3D view = AssemblyViewUtils.Create3DOrthographic(document, id, view3D, isAssigned: true);
            ViewSchedule matSched = AssemblyViewUtils.CreateMaterialTakeoff(document, id, schedule, isAssigned: true);

            view.OrientTo(new XYZ(1, 1, -1));

            Viewport.Create(document, sheet.Id, view.Id, new XYZ(-0.900722413820055, 0.601757777384431, 0.276008003285899));
            ScheduleSheetInstance.Create(document, sheet.Id, matSched.Id, new XYZ(-0.609740381942169, 0.958005249343831, 0));
            if (leg1 != null)
            {
                Viewport.Create(document, sheet.Id, leg1,
                                new XYZ(-0.802193699667686, 0.228131596268927, 0));
            }
            if (leg2 != null)
            {
                Viewport.Create(document, sheet.Id, leg2,
                                new XYZ(-0.80437545819787, 0.0908524885288077, 0));
            }


        }

        public static void Create3DSheetPP(Document document, ElementId id, ViewSheet sheet)
        {

            //Элемент "Обобщенная модель" для ссылок
            Element schedTypeElement = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();


            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId view3D = default;
            ElementId viewFront = default;
            ElementId viewSection = default;
            ElementId Sched5 = default;
            ElementId Sched6 = default;
            ElementId leg1 = default;


            foreach (var item in views.viewReferences)
            {
                switch (item.Name)
                {
                    case "3D_Плита перекрытия":
                        view3D = item.viewTemplate.Id;
                        break;
                    case "П_Плита перекрытия":
                        viewFront = item.viewTemplate.Id;
                        break;
                    case "Р_Плита перекрытия":
                        viewSection = item.viewTemplate.Id;
                        break;
                    case "СА_Плита перекрытия":
                        Sched5 = item.viewTemplate.Id;
                        break;
                    case "М_Плита перекрытия":
                        Sched6 = item.viewTemplate.Id;
                        break;
                    case "Примечание_Пустотные плиты без схемы":
                        if (item.viewTemplate != null)
                        {
                            leg1 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg1 = null;
                        }
                        break;
                }
            }

            ViewSection vsLeft = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.DetailSectionB, viewSection, isAssigned: true);
            ViewSection vsUp2 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationTop, viewFront, isAssigned: true);


            //Создаем виды и размещаем на листах
            //1 лист
            View3D view = AssemblyViewUtils.Create3DOrthographic(document, id, view3D, isAssigned: true);
            ViewSchedule matSched0 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, scheduleCategoryId: schedTypeElement.Category.Id, Sched5, isAssigned: true);
            ViewSchedule matSched1 = AssemblyViewUtils.CreateMaterialTakeoff(document, id, Sched6, isAssigned: true);

            Viewport.Create(document, sheet.Id, view.Id, new XYZ(-0.925963339104755, 0.674129246620504, -0.750183557355297));
            Viewport.Create(document, sheet.Id, vsUp2.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
            Viewport.Create(document, sheet.Id, vsLeft.Id, new XYZ(-0.830265408004621, 0.313597305913539, -0.591511181102365));
            ScheduleSheetInstance.Create(document, sheet.Id, matSched0.Id, new XYZ(-0.62336, 0.44361, 0));
            ScheduleSheetInstance.Create(document, sheet.Id, matSched1.Id, new XYZ(-0.62336, 0.31714889661308, 0));

            if (leg1 != null)
            {
                Viewport.Create(document, sheet.Id, leg1, new XYZ(-0.375249977171706, 0.106285146351993, 0));
            }

        }

        public static void CreateCasingDrawing(Document document, ElementId id, ViewSheet sheet, bool BP = false)
        {
            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId viewFront = default;
            ElementId viewSection = default;
            ElementId schedule = default;
            ElementId leg1 = default;
            ElementId leg2 = default;

            if (BP)
            {
                foreach (var item in views.viewReferences)
                {
                    switch (item.Name)
                    {
                        case "DNS_КЖИ_О_Р_Плиты балконные_Опалубка":
                            viewFront = item.viewTemplate.Id;
                            break;
                        case "DNS_КЖИ_О_Р_Плиты балконные_Опалубка_Сечение":
                            viewSection = item.viewTemplate.Id;
                            break;
                        case "Обозначение_Символ ориентации":
                            if (item.viewTemplate != null)
                            {
                                leg1 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg1 = null;
                            }

                            break;
                        case "Примечание_Лист 2":
                            if (item.viewTemplate != null)
                            {
                                leg2 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg2 = null;
                            }

                            break;
                        case "СЗД_Балконная плита":
                            schedule = item.viewTemplate.Id;
                            break;
                    }
                }
            }
            else
            {
                foreach (var item in views.viewReferences)
                {
                    switch (item.Name)
                    {
                        case "DNS_КЖ_О_Р_Стеновые панели_Опалубка":
                            viewFront = item.viewTemplate.Id;
                            break;
                        case "DNS_КЖ_О_Р_Стеновые панели_Опалубка_Сечение":
                            viewSection = item.viewTemplate.Id;
                            break;
                        case "Обозначение_Символ ориентации":
                            if (item.viewTemplate != null)
                            {
                                leg1 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg1 = null;
                            }

                            break;
                        case "Примечание_Лист 2":
                            if (item.viewTemplate != null)
                            {
                                leg2 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg2 = null;
                            }

                            break;
                        case "СЗД_Панель стеновая":
                            schedule = item.viewTemplate.Id;
                            break;
                    }
                }
            }


            //ссылка для категории спецификации
            Element schedTypeElement = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();

            ViewSection vsFront2 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationFront, viewFront, isAssigned: true);

            ViewSection vsUp2 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.DetailSectionA, viewSection, isAssigned: true);

            ViewSection vsLeft2 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationLeft, viewSection, isAssigned: true);

            ViewSchedule matSched2 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, scheduleCategoryId: schedTypeElement.Category.Id, schedule, isAssigned: true);

            Viewport.Create(document, sheet.Id, vsFront2.Id, new XYZ(-0.75459, 0.63812, -0.241));
            Viewport.Create(document, sheet.Id, vsUp2.Id, new XYZ(-0.75459, 0.259682048822124, -0.351351502340035));
            Viewport.Create(document, sheet.Id, vsLeft2.Id, new XYZ(-0.108762048822124, 0.634839160104988, -0.225465616797901));

            if (leg1 != null)
            {
                Viewport.Create(document, sheet.Id, leg1, new XYZ(-0.401979413423598, 0.181127567857286, 0));
            }
            if (leg2 != null)
            {
                Viewport.Create(document, sheet.Id, leg2, new XYZ(-0.375249977171706, 0.106285146351993, 0));
            }

            ScheduleSheetInstance.Create(document, sheet.Id, matSched2.Id, new XYZ(-1.27797628823942, 0.256357547731719, 0));

        }

        public static void CreateRebarDrawing(Document document, ElementId id, ViewSheet sheet)
        {
            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId viewFront = default;
            ElementId viewSection = default;
            ElementId leg1 = default;
            ElementId leg2 = default;

            foreach (var item in views.viewReferences)
            {
                switch (item.Name)
                {
                    case "DNS_КЖ_О_Р_Стеновые панели_Армирование":
                        viewFront = item.viewTemplate.Id;
                        break;
                    case "DNS_КЖ_О_Р_Стеновые панели_Армирование_Сечение":
                        viewSection = item.viewTemplate.Id;
                        break;
                    case "Обозначение_Символ ориентации":
                        if (item.viewTemplate != null)
                        {
                            leg1 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg1 = null;
                        }
                        break;
                    case "Примечание_Лист 5":
                        if (item.viewTemplate != null)
                        {
                            leg2 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg2 = null;
                        }
                        break;
                }
            }
            //ссылка для категории спецификации
            ViewSection vsFront5 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationFront, viewFront, isAssigned: true);

            ViewSection vsUp5 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.HorizontalDetail, viewSection, isAssigned: true);
            ViewSection vsLeft5 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.DetailSectionA, viewSection, isAssigned: true);

            Viewport.Create(document, sheet.Id, vsFront5.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
            Viewport.Create(document, sheet.Id, vsUp5.Id, new XYZ(-0.830265408004621, 0.313597305913539, -0.591511181102365));
            Viewport.Create(document, sheet.Id, vsLeft5.Id, new XYZ(-0.0912588388280269, 0.755243094492314, 0.437270657274603));

            if (leg1 != null)
            {
                Viewport.Create(document, sheet.Id, leg1, new XYZ(-0.401979413423598, 0.181127567857286, 0));
            }
            if (leg2 != null)
            {
                Viewport.Create(document, sheet.Id, leg2, new XYZ(-0.375249977171706, 0.106285146351993, 0));
            }


        }

        public static void CreateRebarDrawing(Document document, ElementId id, ViewSheet sheet, bool Internal)
        {
            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId viewFront = default;
            ElementId viewSection = default;
            ElementId leg1 = default;
            ElementId leg2 = default;

            foreach (var item in views.viewReferences)
            {
                if (Internal)
                {
                    switch (item.Name)
                    {
                        case "DNS_КЖ_О_Р_Стеновые панели_Армирование_Внутренний слой":
                            viewFront = item.viewTemplate.Id;
                            break;
                        case "DNS_КЖ_О_Р_Стеновые панели_Армирование_Внутренний слой_Сечение":
                            viewSection = item.viewTemplate.Id;
                            break;
                        case "Обозначение_Символ ориентации":
                            if (item.viewTemplate != null)
                            {
                                leg1 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg1 = null;
                            }
                            break;
                        case "Примечание_Лист 5":
                            if (item.viewTemplate != null)
                            {
                                leg2 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg2 = null;
                            }
                            break;

                    }
                }
                else
                {
                    switch (item.Name)
                    {
                        case "DNS_КЖ_О_Р_Стеновые панели_Армирование_Наружный слой":
                            viewFront = item.viewTemplate.Id;
                            break;
                        case "DNS_КЖ_О_Р_Стеновые панели_Армирование_Наружный слой_Сечение":
                            viewSection = item.viewTemplate.Id;
                            break;
                        case "Обозначение_Символ ориентации":
                            if (item.viewTemplate != null)
                            {
                                leg1 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg1 = null;
                            }
                            break;
                        case "Примечание_Лист 6":
                            if (item.viewTemplate != null)
                            {
                                leg2 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg2 = null;
                            }
                            break;

                    }
                }

            }
            //ссылка для категории спецификации
            ViewSection vsFront5 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationFront, viewFront, isAssigned: true);
            ViewSection vsUp5 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.HorizontalDetail, viewSection, isAssigned: true);
            ViewSection vsLeft5 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.DetailSectionA, viewSection, isAssigned: true);

            Viewport.Create(document, sheet.Id, vsFront5.Id, new XYZ(-0.75459, 0.63812, -0.240399999999999));
            Viewport.Create(document, sheet.Id, vsUp5.Id, new XYZ(-0.75459, 0.255417951177876, -0.184255118110238));
            Viewport.Create(document, sheet.Id, vsLeft5.Id, new XYZ(-0.108762048822124, 0.634839160104988, -0.225465616797901));

            if (leg1 != null)
            {
                Viewport.Create(document, sheet.Id, leg1, new XYZ(-0.401979413423598, 0.181127567857286, 0));
            }
            if (leg2 != null)
            {
                Viewport.Create(document, sheet.Id, leg2, new XYZ(-0.375249977171706, 0.106285146351993, 0));
            }


        }

        public static void CreateMeshDrawing(Document document, ElementId id, ViewSheet sheet)
        {
            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId viewFront = default;
            ElementId schedule = default;
            ElementId leg2 = default;

            foreach (var item in views.viewReferences)
            {
                switch (item.Name)
                {
                    case "DNS_КЖ_О_Р_Сетка арматурная":
                        viewFront = item.viewTemplate.Id;
                        break;
                    case "СА_Сетка арматурная":
                        schedule = item.viewTemplate.Id;
                        break;
                    case "Примечание_Лист 5":
                        if (item.viewTemplate != null)
                        {
                            leg2 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg2 = null;
                        }
                        break;
                }
            }

            Element schedTypeElement = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();

            //ссылка для категории спецификации
            ViewSection vsFront9 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationFront, viewFront, isAssigned: true);
            ViewSchedule matSched9 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, scheduleCategoryId: schedTypeElement.Category.Id, schedule, isAssigned: true);

            Viewport.Create(document, sheet.Id, vsFront9.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));

            if (leg2 != null)
            {
                Viewport.Create(document, sheet.Id, leg2, new XYZ(-0.343564734808835, 0.119108520484288, 0));
            }

            ScheduleSheetInstance.Create(document, sheet.Id, matSched9.Id, new XYZ(-1.25102, 0.348649448818898, 0));
        }

        public static void CreateMeshDrawing(Document document, ElementId id, ViewSheet sheet, bool Internal)
        {
            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId viewFront = default;
            ElementId schedule = default;
            ElementId leg2 = default;

            foreach (var item in views.viewReferences)
            {
                if (Internal)
                {
                    switch (item.Name)
                    {
                        case "DNS_КЖ_О_Р_Сетка арматурная_Внутренний слой":
                            viewFront = item.viewTemplate.Id;
                            break;
                        case "СА_Сетка арматурная_Внутренний слой":
                            schedule = item.viewTemplate.Id;
                            break;
                        case "Примечание_Лист 8":
                            if (item.viewTemplate != null)
                            {
                                leg2 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg2 = null;
                            }
                            break;
                    }
                }
                else
                {
                    switch (item.Name)
                    {
                        case "DNS_КЖ_О_Р_Сетка арматурная_Наружный слой":
                            viewFront = item.viewTemplate.Id;
                            break;
                        case "СА_Сетка арматурная_Наружный слой":
                            schedule = item.viewTemplate.Id;
                            break;
                        case "Примечание_Лист 9":
                            if (item.viewTemplate != null)
                            {
                                leg2 = item.viewTemplate.Id;
                            }
                            else
                            {
                                leg2 = null;
                            }
                            break;
                    }
                }
            }

            //ссылка для категории спецификации
            Element schedTypeElement = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();

            //ссылка для категории спецификации
            ViewSection vsFront9 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationFront, viewFront, isAssigned: true);
            ViewSchedule matSched9 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, scheduleCategoryId: schedTypeElement.Category.Id, schedule, isAssigned: true);

            Viewport.Create(document, sheet.Id, vsFront9.Id, new XYZ(-0.70948, 0.65119, -0.240400000000002));
            if (leg2 != null)
            {
                Viewport.Create(document, sheet.Id, leg2, new XYZ(-0.343564734808835, 0.119108520484288, 0));
            }

            ScheduleSheetInstance.Create(document, sheet.Id, matSched9.Id, new XYZ(-1.25102, 0.348649448818898, 0));
        }

        public static void CreateScheduleDrawing(Document document, ElementId id, ViewSheet sheet)
        {

            //TODO: Понять почему не создаются спеки для ПС

            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId schedule1 = default;
            ElementId schedule2 = default;
            ElementId schedule3 = default;


            foreach (var item in views.viewReferences)
            {
                switch (item.Name)
                {
                    case "СА_Панель стеновая":
                        if (item.viewTemplate != null)
                        {
                            schedule1 = item.viewTemplate.Id;
                        }
                        else
                        {
                            schedule1 = null;
                        }
                        break;
                    case "СА_Масса_Панель стеновая":
                        if (item.viewTemplate != null)
                        {
                            schedule2 = item.viewTemplate.Id;
                        }
                        else
                        {
                            schedule2 = null;
                        }
                        break;
                    case "СТА_Панель стеновая":
                        if (item.viewTemplate != null)
                        {
                            schedule3 = item.viewTemplate.Id;
                        }
                        else
                        {
                            schedule3 = null;
                        }

                        break;
                }
            }

            //ссылка для категории спецификации
            Element schedTypeElement = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();

            if (schedule1 != null)
            {
                ViewSchedule matSched70 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, scheduleCategoryId: schedTypeElement.Category.Id, schedule1, isAssigned: true);
                ScheduleSheetInstance.Create(document, sheet.Id, matSched70.Id, new XYZ(-1.28132, 0.93059, 0));
            }

            if (schedule2 != null)
            {
                ViewSchedule matSched71 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, scheduleCategoryId: schedTypeElement.Category.Id, schedule2, isAssigned: true);
                ScheduleSheetInstance.Create(document, sheet.Id, matSched71.Id, new XYZ(-0.822, 0.8413, 0));
            }

            if (schedule3 != null)
            {
                ViewSchedule matSched72 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, scheduleCategoryId: schedTypeElement.Category.Id, schedule3, isAssigned: true);
                ScheduleSheetInstance.Create(document, sheet.Id, matSched72.Id, new XYZ(-0.62811, 0.93059, 0));
            }

            ;


        }

        public static void CreateJointDrawing(Document document, ElementId id, ViewSheet sheet)
        {
            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId viewFront = default;
            ElementId viewSection = default;
            ElementId leg5 = default;
            ElementId leg6 = default;
            ElementId sched4 = default;


            foreach (var item in views.viewReferences)
            {
                switch (item.Name)
                {
                    case "DNS_КЖ_О_Р_Стеновые панели_Бийски":
                        viewFront = item.viewTemplate.Id;
                        break;
                    case "DNS_КЖ_О_Р_Стеновые панели_Бийски_Сечение":
                        viewSection = item.viewTemplate.Id;
                        break;
                    case "СТА_Панель стеновая":
                        sched4 = item.viewTemplate.Id;
                        break;
                    case "Примечание_Бийски панелей НС":
                        if (item.viewTemplate != null)
                        {
                            leg5 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg5 = null;
                        }
                        break;
                    case "Обозначения_Бийски панелей НС":
                        if (item.viewTemplate != null)
                        {
                            leg6 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg6 = null;
                        }
                        break;
                }
            }

            //ссылка для категории спецификации
            Element schedTypeElement = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();

            ViewSection vsFront4 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationFront, viewFront, isAssigned: true);
            ViewSection vsLeft4 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.DetailSectionA, viewSection, isAssigned: true);
            ViewSchedule matSched4 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, scheduleCategoryId: schedTypeElement.Category.Id, sched4, isAssigned: true);

            Viewport.Create(document, sheet.Id, vsFront4.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
            Viewport.Create(document, sheet.Id, vsLeft4.Id, new XYZ(-0.0912588388280269, 0.755243094492314, 0.437270657274603));
            if (leg5 != null)
            {
                Viewport.Create(document, sheet.Id, leg5, new XYZ(-0.401979413423598, 0.181127567857286, 0));
            }

            if (leg6 != null)
            {
                Viewport.Create(document, sheet.Id, leg6, new XYZ(-0.375249977171706, 0.106285146351993, 0));
            }


            ScheduleSheetInstance.Create(document, sheet.Id, matSched4.Id, new XYZ(-1.27797628823942, 0.256357547731719, 0));


        }
        public static void CreateInsulationDrawing(Document document, ElementId id, ViewSheet sheet)
        {
            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId viewFront = default;
            ElementId leg3 = default;
            ElementId leg4 = default;
            ElementId Sched2 = default;
            ElementId Sched3 = default;


            foreach (var item in views.viewReferences)
            {
                switch (item.Name)
                {
                    case "DNS_КЖ_О_Р_Стеновые панели_Утеплитель":
                        viewFront = item.viewTemplate.Id;
                        break;
                    case "УГО_Лист 3":
                        if (item.viewTemplate != null)
                        {
                            leg3 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg3 = null;
                        }
                        break;
                    case "Примечание_Лист 3":
                        if (item.viewTemplate != null)
                        {
                            leg4 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg4 = null;
                        }
                        break;
                    case "М_Минвата":
                        Sched2 = item.viewTemplate.Id;
                        break;
                    case "М_Полистирол":
                        Sched3 = item.viewTemplate.Id;
                        break;
                }
            }

            //ссылка для категории спецификации

            ViewSection vsFront3 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationFront, viewFront, isAssigned: true);
            ViewSchedule matSched3 = AssemblyViewUtils.CreateMaterialTakeoff(document, id, Sched2, isAssigned: true);
            ViewSchedule matSched31 = AssemblyViewUtils.CreateMaterialTakeoff(document, id, Sched3, isAssigned: true);

            ScheduleSheetInstance.Create(document, sheet.Id, matSched3.Id, new XYZ(-1.22883720684845, 0.280437531925655, 0));
            ScheduleSheetInstance.Create(document, sheet.Id, matSched31.Id, new XYZ(-0.589073427320895, 0.280437531925653, 0));

            Viewport.Create(document, sheet.Id, vsFront3.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
            if (leg3 != null)
            {
                Viewport.Create(document, sheet.Id, leg3, new XYZ(-0.401979413423598, 0.181127567857286, 0));
            }

            if (leg4 != null)
            {
                Viewport.Create(document, sheet.Id, leg4, new XYZ(-0.375249977171706, 0.106285146351993, 0));
            }

        }

        public static void SetSheetParameters(Document document, ElementId id, ViewSheet sheet, int n)
        {
            Element assembly = document.GetElement(id);
            Element view = (Element)sheet;

            view.get_Parameter(BuiltInParameter.SHEET_NAME).Set(assembly.Name);
            view.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(n.ToString());

        }

        public static void CreateFacadeDrawing(Document document, ElementId id, ViewSheet sheet)
        {

            //Элемент "Обобщенная модель" для ссылок
            Element schedTypeElement = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Parts).FirstElement();

            ViewsSingleton views = ViewsSingleton.getInstance(document);

            ElementId viewFront = default;
            ElementId sched1 = default;
            ElementId sched2 = default;
            ElementId leg1 = default;
            ElementId leg2 = default;
            ElementId leg3 = default;

            foreach (var item in views.viewReferences)
            {
                switch (item.Name)
                {
                    case "DNS_F_Плитка":
                        viewFront = item.viewTemplate.Id;
                        break;

                    case "DNS_Ф_Плитка":
                        sched1 = item.viewTemplate.Id;
                        break;

                    case "DNS_Марки_Плитка":
                        sched2 = item.viewTemplate.Id;
                        break;

                    case "АКР примечание":
                        if (item.viewTemplate != null)
                        {
                            leg1 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg1 = null;
                        }
                        break;
                    case "Схема раскладки плитки (рядовая панель)":
                        if (item.viewTemplate != null)
                        {
                            leg2 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg2 = null;
                        }
                        break;
                    case "Условные обозначения":
                        if (item.viewTemplate != null)
                        {
                            leg3 = item.viewTemplate.Id;
                        }
                        else
                        {
                            leg3 = null;
                        }
                        break;
                }
            }


            ViewSection vsUp2 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationRight, viewFront, isAssigned: true);


            //Создаем виды и размещаем на листах
            //1 лист
            ViewSchedule ptSched1 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, schedTypeElement.Category.Id, sched1, false);

            ViewSchedule ptSched2 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, schedTypeElement.Category.Id, sched2, false);

            Viewport.Create(document, sheet.Id, vsUp2.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));

            ScheduleSheetInstance.Create(document, sheet.Id, ptSched1.Id, new XYZ(-0.62336, 0.44361, 0));
            ScheduleSheetInstance.Create(document, sheet.Id, ptSched2.Id, new XYZ(-0.62336, 0.31714889661308, 0));

            if (leg1 != null)
            {
                Viewport.Create(document, sheet.Id, leg1, new XYZ(-0.471315624987842, 0.21358, 0));
            }
            if (leg2 != null)
            {
                Viewport.Create(document, sheet.Id, leg2, new XYZ(-1.14009, 0.14663, -3.89432076330052E-17));
            }
            if (leg3 != null)
            {
                Viewport.Create(document, sheet.Id, leg3, new XYZ(-0.7956, 0.14663, 9.90389806990418E-17));
            }

        }

    }
}


