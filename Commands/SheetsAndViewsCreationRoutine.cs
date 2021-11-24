using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools_v2.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DSKPrim.PanelTools_v2.Commands
{
    class SheetsAndViewsCreationRoutine : Routine
    {
        public override StructuralApps.Panel.Panel Behaviour { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Document Document { get ; set; }
        //ИДЕЯ
        // Что если парсить json на предмет листов, видов и шаблонов?
        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            Logger.Logger logger = Logger.Logger.getInstance();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            FilteredElementCollector assemblies = new FilteredElementCollector(Document).OfClass(typeof(AssemblyInstance)).WhereElementIsNotElementType();



            foreach (var element in assemblies)
            {
                AssemblyInstance item = (AssemblyInstance)element;
                if (item.Name.Contains("НС"))
                {
                    CreateNS(item);
                }
                else if (item.Name.Contains("ВС"))
                {
                    CreateVS(item);
                }
                else if(item.Name.Contains("ПП"))
                {
                    CreatePP(item);
                }
                else if (item.Name.Contains("БП"))
                {

                }
                else if (item.Name.Contains("ПС"))
                {

                }

            }

            logger.LogSuccessTime(stopWatch);

        }
        private void CreateNS(AssemblyInstance item)
        {
            Transaction transaction = new Transaction(Document, $"Создание листов панели {item.Name}");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            opts.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(opts);
            //Получаем нужные элементы, шаблоны видов

            IEnumerable<Element> titleBlock_F4 = new FilteredElementCollector(Document).OfClass(typeof(FamilySymbol)).WhereElementIsElementType().Where(x => x.Name.Contains("Форма 4_чертил"));
            IEnumerable<Element> titleBlock_F6 = new FilteredElementCollector(Document).OfClass(typeof(FamilySymbol)).WhereElementIsElementType().Where(x => x.Name.Contains("Форма 6"));

            //Виды
            IEnumerable<Element> view3D = new FilteredElementCollector(Document).OfClass(typeof(View3D)).Where(o => o.Name.Contains("3D_Стеновая панель"));
            IEnumerable<Element> viewFront_Page2 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Опалубка"));
            IEnumerable<Element> viewSection_Page2 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Опалубка_Сечение"));
            IEnumerable<Element> viewFront_Page3 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Утеплитель"));
            IEnumerable<Element> viewFront_Page4 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Бийски"));
            IEnumerable<Element> viewSection_Page4 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Бийски_Сечение"));
            IEnumerable<Element> viewFront_Page5 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Армирование_Внутренний слой"));
            IEnumerable<Element> viewSection_Page5 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Армирование_Внутренний слой_Сечение"));
            IEnumerable<Element> viewFront_Page6 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Армирование_Наружный слой"));
            IEnumerable<Element> viewSection_Page6 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Армирование_Наружный слой_Сечение"));
            IEnumerable<Element> viewFront_Page8 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Сетка арматурная_Внутренний слой"));
            IEnumerable<Element> viewFront_Page9 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Сетка арматурная_Наружный слой"));

            //Легенды
            IEnumerable<Element> NS_1st_leg = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Обозначения_Панели стеновые наружные"));
            IEnumerable<Element> NS_2nd_leg = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Панели стеновые наружные"));
            IEnumerable<Element> leg1 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Обозначение_Символ ориентации"));
            IEnumerable<Element> leg2 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 2"));
            IEnumerable<Element> leg3 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("УГО_Лист 3"));
            IEnumerable<Element> leg4 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 3"));
            IEnumerable<Element> leg5 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Бийски панелей НС"));
            IEnumerable<Element> leg6 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Обозначения_Бийски панелей НС"));
            IEnumerable<Element> leg7 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 5"));
            IEnumerable<Element> leg8 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 6"));
            IEnumerable<Element> leg9 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 7"));
            IEnumerable<Element> leg10 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 8"));
            IEnumerable<Element> leg11 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 9"));

            //Элемент "Обобщенная модель" для ссылок
            Element schedTypeElement = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();


            //Спеки
            IEnumerable<Element> NS_matSched = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("М_Панель стеновая"));
            IEnumerable<Element> Sched1 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СЗД_Панель стеновая"));
            IEnumerable<Element> Sched2 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("М_Минвата"));
            IEnumerable<Element> Sched3 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("М_Полистирол"));
            IEnumerable<Element> Sched4 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СБ_Панель стеновая"));
            IEnumerable<Element> Sched5 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СА_Панель стеновая"));
            IEnumerable<Element> Sched6 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СА_Масса_Панель стеновая"));
            IEnumerable<Element> Sched7 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СТА_Панель стеновая"));
            IEnumerable<Element> Sched8 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СА_Сетка арматурная_Внутренний слой"));
            IEnumerable<Element> Sched9 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СА_Сетка арматурная_Наружный слой"));

            //Создаём листы
            using (transaction)
            {
                transaction.Start();
                ViewSheet sheet1 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F4.First().Id);
                ViewSheet sheet2 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet3 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet4 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet5 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet6 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet7 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet8 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet9 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);

                //Создаем виды и размещаем на листах
                //1 лист
                View3D view = AssemblyViewUtils.Create3DOrthographic(Document, item.Id, view3D.First().Id, isAssigned: true);
                ViewSchedule matSched = AssemblyViewUtils.CreateMaterialTakeoff(Document, item.Id, NS_matSched.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet1.Id, view.Id, new XYZ(-0.925963339104755, 0.674129246620504, -0.750183557355297));
                ScheduleSheetInstance.Create(Document, sheet1.Id, matSched.Id, new XYZ(-0.609740381942169, 0.958005249343831, 0));

                Viewport.Create(Document, sheet1.Id, NS_1st_leg.First().Id,
                    new XYZ(-0.802193699667686, 0.228131596268927, 0));
                Viewport.Create(Document, sheet1.Id, NS_2nd_leg.First().Id,
                    new XYZ(-0.80437545819787, 0.0908524885288077, 0));

                //2 лист
                ViewSection vsFront2 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page2.First().Id, isAssigned: true);
                ViewSection vsUp2 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationTop, viewSection_Page2.First().Id, isAssigned: true);
                ViewSection vsLeft2 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationLeft, viewSection_Page2.First().Id, isAssigned: true);
                ViewSchedule matSched2 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched1.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet2.Id, vsFront2.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet2.Id, vsUp2.Id, new XYZ(-0.830265408004621, 0.313597305913539, -0.591511181102365));
                Viewport.Create(Document, sheet2.Id, vsLeft2.Id, new XYZ(-0.0912588388280269, 0.755243094492314, 0.437270657274603));

                Viewport.Create(Document, sheet2.Id, leg1.First().Id, new XYZ(-0.401979413423598, 0.181127567857286, 0));
                Viewport.Create(Document, sheet2.Id, leg2.First().Id, new XYZ(-0.375249977171706, 0.106285146351993, 0));

                ScheduleSheetInstance.Create(Document, sheet2.Id, matSched2.Id, new XYZ(-1.27797628823942, 0.256357547731719, 0));
                //3 лист
                ViewSection vsFront3 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page3.First().Id, isAssigned: true);
                ViewSchedule matSched3 = AssemblyViewUtils.CreateMaterialTakeoff(Document, item.Id, Sched2.First().Id, isAssigned: true);
                ViewSchedule matSched31 = AssemblyViewUtils.CreateMaterialTakeoff(Document, item.Id, Sched3.First().Id, isAssigned: true);

                ScheduleSheetInstance.Create(Document, sheet3.Id, matSched2.Id, new XYZ(-1.22883720684845, 0.280437531925655, 0));
                ScheduleSheetInstance.Create(Document, sheet3.Id, matSched31.Id, new XYZ(-0.589073427320895, 0.280437531925653, 0));

                Viewport.Create(Document, sheet3.Id, vsFront3.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet3.Id, leg3.First().Id, new XYZ(-0.401979413423598, 0.181127567857286, 0));
                Viewport.Create(Document, sheet3.Id, leg4.First().Id, new XYZ(-0.375249977171706, 0.106285146351993, 0));

                //4 лист
                ViewSection vsFront4 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page4.First().Id, isAssigned: true);
                ViewSection vsLeft4 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationLeft, viewSection_Page4.First().Id, isAssigned: true);
                ViewSchedule matSched4 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched4.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet4.Id, vsFront4.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet4.Id, vsLeft4.Id, new XYZ(-0.0912588388280269, 0.755243094492314, 0.437270657274603));

                Viewport.Create(Document, sheet4.Id, leg5.First().Id, new XYZ(-0.401979413423598, 0.181127567857286, 0));
                Viewport.Create(Document, sheet4.Id, leg6.First().Id, new XYZ(-0.375249977171706, 0.106285146351993, 0));

                ScheduleSheetInstance.Create(Document, sheet4.Id, matSched.Id, new XYZ(-1.27797628823942, 0.256357547731719, 0));
                //5 лист	
                ViewSection vsFront5 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page5.First().Id, isAssigned: true);
                ViewSection vsUp5 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationTop, viewSection_Page5.First().Id, isAssigned: true);
                ViewSection vsLeft5 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationLeft, viewSection_Page5.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet5.Id, vsFront5.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet5.Id, vsUp5.Id, new XYZ(-0.830265408004621, 0.313597305913539, -0.591511181102365));
                Viewport.Create(Document, sheet5.Id, vsLeft5.Id, new XYZ(-0.0912588388280269, 0.755243094492314, 0.437270657274603));

                Viewport.Create(Document, sheet5.Id, leg1.First().Id, new XYZ(-0.401979413423598, 0.181127567857286, 0));
                Viewport.Create(Document, sheet5.Id, leg7.First().Id, new XYZ(-0.375249977171706, 0.106285146351993, 0));

                //6 лист
                ViewSection vsFront6 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page6.First().Id, isAssigned: true);
                ViewSection vsUp6 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationTop, viewSection_Page6.First().Id, isAssigned: true);
                ViewSection vsLeft6 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationLeft, viewSection_Page6.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet6.Id, vsFront6.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet6.Id, vsUp6.Id, new XYZ(-0.830265408004621, 0.313597305913539, -0.591511181102365));
                Viewport.Create(Document, sheet6.Id, vsLeft6.Id, new XYZ(-0.0912588388280269, 0.755243094492314, 0.437270657274603));

                Viewport.Create(Document, sheet6.Id, leg1.First().Id, new XYZ(-0.401979413423598, 0.181127567857286, 0));
                Viewport.Create(Document, sheet6.Id, leg8.First().Id, new XYZ(-0.375249977171706, 0.106285146351993, 0));

                //7 лист
                ViewSchedule matSched70 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched5.First().Id, isAssigned: true);
                ViewSchedule matSched71 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched6.First().Id, isAssigned: true);
                ViewSchedule matSched72 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched7.First().Id, isAssigned: true);

                ScheduleSheetInstance.Create(Document, sheet7.Id, matSched70.Id, new XYZ(-1.28132, 0.93059, 0));
                ScheduleSheetInstance.Create(Document, sheet7.Id, matSched71.Id, new XYZ(-0.822, 0.8413, 0));
                ScheduleSheetInstance.Create(Document, sheet7.Id, matSched72.Id, new XYZ(-0.62811, 0.93059, 0));
                //8 лист
                ViewSection vsFront8 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page8.First().Id, isAssigned: true);
                ViewSchedule matSched8 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched8.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet8.Id, vsFront8.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet8.Id, leg10.First().Id, new XYZ(-0.343564734808835, 0.119108520484288, 0));
                ScheduleSheetInstance.Create(Document, sheet8.Id, matSched8.Id, new XYZ(-1.25102, 0.348649448818898, 0));

                //9 лист
                ViewSection vsFront9 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page9.First().Id, isAssigned: true);
                ViewSchedule matSched9 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched9.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet9.Id, vsFront9.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet9.Id, leg11.First().Id, new XYZ(-0.343564734808835, 0.119108520484288, 0));
                ScheduleSheetInstance.Create(Document, sheet9.Id, matSched9.Id, new XYZ(-1.25102, 0.348649448818898, 0));

                transaction.Commit();
            }
        }

        private void CreateVS(AssemblyInstance item)
        {
            Transaction transaction = new Transaction(Document, $"Создание листов панели {item.Name}");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            opts.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(opts);
            //Получаем нужные элементы, шаблоны видов

            

            IEnumerable<Element> titleBlock_F4 = new FilteredElementCollector(Document).OfClass(typeof(FamilySymbol)).WhereElementIsElementType().Where(x => x.Name.Contains("Форма 4_чертил"));
            IEnumerable<Element> titleBlock_F6 = new FilteredElementCollector(Document).OfClass(typeof(FamilySymbol)).WhereElementIsElementType().Where(x => x.Name.Contains("Форма 6"));

            //Виды
            IEnumerable<Element> view3D = new FilteredElementCollector(Document).OfClass(typeof(View3D)).Where(o => o.Name.Contains("3D_Стеновая панель"));
            IEnumerable<Element> viewFront_Page2 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Опалубка"));
            IEnumerable<Element> viewSection_Page2 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Опалубка_Сечение"));
            IEnumerable<Element> viewFront_Page3 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Армирование"));
            IEnumerable<Element> viewSection_Page3 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Стеновые панели_Армирование_Сечение"));
            IEnumerable<Element> viewFront_Page5 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("DNS_КЖ_О_Р_Сетка арматурная"));

            //Легенды
            IEnumerable<Element> NS_1st_leg = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Обозначения_Панели стеновые внутренние"));
            IEnumerable<Element> NS_2nd_leg = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Панели стеновые внутренние"));
            IEnumerable<Element> leg2 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 2"));
            IEnumerable<Element> leg4 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 3"));
            IEnumerable<Element> leg7 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Лист 5"));

            
            //Элемент "Обобщенная модель" для ссылок
            Element schedTypeElement = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();


            //Спеки
            IEnumerable<Element> NS_matSched = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("М_Панель стеновая"));
            IEnumerable<Element> Sched1 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СЗД_Панель стеновая"));

            IEnumerable<Element> Sched5 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СА_Панель стеновая"));
            IEnumerable<Element> Sched6 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СА_Масса_Панель стеновая"));
            IEnumerable<Element> Sched7 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СТА_Панель стеновая"));
            IEnumerable<Element> Sched8 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СА_Сетка арматурная"));

            //Создаём листы
            using (transaction)
            {
                transaction.Start();
                ViewSheet sheet1 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F4.First().Id);
                ViewSheet sheet2 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet3 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet4 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);
                ViewSheet sheet5 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F6.First().Id);


                //Создаем виды и размещаем на листах
                //1 лист
                View3D view = AssemblyViewUtils.Create3DOrthographic(Document, item.Id, view3D.First().Id, isAssigned: true);
                ViewSchedule matSched = AssemblyViewUtils.CreateMaterialTakeoff(Document, item.Id, NS_matSched.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet1.Id, view.Id, new XYZ(-0.925963339104755, 0.674129246620504, -0.750183557355297));
                ScheduleSheetInstance.Create(Document, sheet1.Id, matSched.Id, new XYZ(-0.609740381942169, 0.958005249343831, 0));

                Viewport.Create(Document, sheet1.Id, NS_2nd_leg.First().Id,
                    new XYZ(-0.80437545819787, 0.0908524885288077, 0));

                //2 лист
                ViewSection vsFront2 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page2.First().Id, isAssigned: true);
                ViewSection vsUp2 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationTop, viewSection_Page2.First().Id, isAssigned: true);
                ViewSection vsLeft2 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationLeft, viewSection_Page2.First().Id, isAssigned: true);
                ViewSchedule matSched2 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched1.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet2.Id, vsFront2.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet2.Id, vsUp2.Id, new XYZ(-0.830265408004621, 0.313597305913539, -0.591511181102365));
                Viewport.Create(Document, sheet2.Id, vsLeft2.Id, new XYZ(-0.0912588388280269, 0.755243094492314, 0.437270657274603));

                Viewport.Create(Document, sheet2.Id, leg2.First().Id, new XYZ(-0.375249977171706, 0.106285146351993, 0));

                ScheduleSheetInstance.Create(Document, sheet2.Id, matSched2.Id, new XYZ(-1.27797628823942, 0.256357547731719, 0));

                //3 лист	
                ViewSection vsFront3 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page3.First().Id, isAssigned: true);
                ViewSection vsUp3 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationTop, viewSection_Page3.First().Id, isAssigned: true);
                ViewSection vsLeft3 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationLeft, viewSection_Page3.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet3.Id, vsFront3.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet3.Id, vsUp3.Id, new XYZ(-0.830265408004621, 0.313597305913539, -0.591511181102365));
                Viewport.Create(Document, sheet3.Id, vsLeft3.Id, new XYZ(-0.0912588388280269, 0.755243094492314, 0.437270657274603));

                Viewport.Create(Document, sheet3.Id, NS_1st_leg.First().Id, new XYZ(-0.401979413423598, 0.181127567857286, 0));
                Viewport.Create(Document, sheet3.Id, leg7.First().Id, new XYZ(-0.375249977171706, 0.106285146351993, 0));


                //4 лист
                ViewSchedule matSched70 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched5.First().Id, isAssigned: true);
                ViewSchedule matSched71 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched6.First().Id, isAssigned: true);
                ViewSchedule matSched72 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched7.First().Id, isAssigned: true);

                ScheduleSheetInstance.Create(Document, sheet4.Id, matSched70.Id, new XYZ(-1.28132, 0.93059, 0));
                ScheduleSheetInstance.Create(Document, sheet4.Id, matSched71.Id, new XYZ(-0.822, 0.8413, 0));
                ScheduleSheetInstance.Create(Document, sheet4.Id, matSched72.Id, new XYZ(-0.62811, 0.93059, 0));

                //5 лист
                ViewSection vsFront9 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page5.First().Id, isAssigned: true);
                ViewSchedule matSched9 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched8.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet5.Id, vsFront9.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet5.Id, leg7.First().Id, new XYZ(-0.343564734808835, 0.119108520484288, 0));
                ScheduleSheetInstance.Create(Document, sheet5.Id, matSched9.Id, new XYZ(-1.25102, 0.348649448818898, 0));

                transaction.Commit();
            }
        }

        private void CreatePP(AssemblyInstance item)
        {
            Transaction transaction = new Transaction(Document, $"Создание листов панели {item.Name}");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            opts.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(opts);
            //Получаем нужные элементы, шаблоны видов

            IEnumerable<Element> titleBlock_F4 = new FilteredElementCollector(Document).OfClass(typeof(FamilySymbol)).WhereElementIsElementType().Where(x => x.Name.Contains("Форма 4_чертил"));


            //Виды
            IEnumerable<Element> view3D = new FilteredElementCollector(Document).OfClass(typeof(View3D)).Where(o => o.Name.Contains("3D_Плита перекрытия"));
            IEnumerable<Element> viewFront_Page2 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("П_Плита перекрытия"));
            IEnumerable<Element> viewSection_Page2 = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(o => o.Name.Contains("Р_Плита перекрытия"));

            //Легенды
            IEnumerable<Element> NS_1st_leg = new FilteredElementCollector(Document).OfClass(typeof(View)).Where(i => i.Name.Contains("Примечание_Пустотные плиты без схемы"));


            //Элемент "Обобщенная модель" для ссылок
            Element schedTypeElement = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();


            //Спеки
            IEnumerable<Element> Sched5 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("СА_Плита перекрытия"));
            IEnumerable<Element> Sched6 = new FilteredElementCollector(Document).OfClass(typeof(ViewSchedule)).Where(o => o.Name.Contains("М_Плита перекрытия"));


            //Создаём листы
            using (transaction)
            {
                transaction.Start();
                ViewSheet sheet1 = AssemblyViewUtils.CreateSheet(Document, item.Id, titleBlock_F4.First().Id);
                ViewSection vsFront2 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.DetailSectionA, viewFront_Page2.First().Id, isAssigned: true);
                ViewSection vsUp2 = AssemblyViewUtils.CreateDetailSection(Document, item.Id, AssemblyDetailViewOrientation.ElevationTop, viewSection_Page2.First().Id, isAssigned: true);


                //Создаем виды и размещаем на листах
                //1 лист
                View3D view = AssemblyViewUtils.Create3DOrthographic(Document, item.Id, view3D.First().Id, isAssigned: true);
                ViewSchedule matSched0 = AssemblyViewUtils.CreateSingleCategorySchedule(Document, item.Id, scheduleCategoryId: schedTypeElement.Category.Id, Sched5.First().Id, isAssigned: true);
                ViewSchedule matSched1 = AssemblyViewUtils.CreateMaterialTakeoff(Document, item.Id, Sched6.First().Id, isAssigned: true);

                Viewport.Create(Document, sheet1.Id, view.Id, new XYZ(-0.925963339104755, 0.674129246620504, -0.750183557355297));
                Viewport.Create(Document, sheet1.Id, vsFront2.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));
                Viewport.Create(Document, sheet1.Id, vsUp2.Id, new XYZ(-0.830265408004621, 0.313597305913539, -0.591511181102365));
                ScheduleSheetInstance.Create(Document, sheet1.Id, matSched0.Id, new XYZ(-0.62336, 0.44361, 0));
                ScheduleSheetInstance.Create(Document, sheet1.Id, matSched1.Id, new XYZ(-0.62336, 0.31714889661308, 0));
                Viewport.Create(Document, sheet1.Id, NS_1st_leg.First().Id, new XYZ(-0.375249977171706, 0.106285146351993, 0));

                transaction.Commit();
            }
        }

        private void CreateBP(AssemblyInstance item)
        {
            // TODO: записать создание листов для БП
        }

        private void CreatePS(AssemblyInstance item)
        {
            // TODO: записать создание листов для ПС
        }
    }
}
