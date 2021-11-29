using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.StructuralApps.Views
{
    public class ViewsSingleton: IDisposable
    {

        public class ViewReference
        {
            public Document Document;
            public string Name;
            public Type Type;
            public Element viewTemplate;

            public ViewReference(Document document, string name, Type type)
            {
                Document = document;
                Name = name;
                Type = type;
                try
                {
                    viewTemplate = new FilteredElementCollector(Document).OfClass(type).FirstOrDefault(o => o.Name.Contains(name));
                }
                catch (Exception)
                {
                    viewTemplate = null;
                }
                
            }

        }

        public class ViewReferences : IEnumerable
        {
            private ViewReference[] _viewReferences;
            public ViewReferences(ViewReference[] pArray)
            {
                _viewReferences = new ViewReference[pArray.Length];

                for (int i = 0; i < pArray.Length; i++)
                {
                    _viewReferences[i] = pArray[i];
                }
            }

            // Implementation for the GetEnumerator method.
            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }

            public ViewEnum GetEnumerator()
            {
                return new ViewEnum(_viewReferences);
            }
        }

        public class ViewEnum : IEnumerator
        {
            public ViewReference[] _viewReferences;

            // Enumerators are positioned before the first element
            // until the first MoveNext() call.
            int position = -1;

            public ViewEnum(ViewReference[] list)
            {
                _viewReferences = list;
            }

            public bool MoveNext()
            {
                position++;
                return (position < _viewReferences.Length);
            }

            public void Reset()
            {
                position = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public ViewReference Current
            {
                get
                {
                    try
                    {
                        return _viewReferences[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        private static ViewsSingleton Instance;

        private Document Document;

        public ViewReferences viewReferences;

        private static string[] PanelNames = new string[5]
            {
                "НС",
                "ВС",
                "ПП",
                "БП",
                "ПС"
            };

        private ViewsSingleton(Document document)
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            Document = document;
            ViewReference[] refs = new ViewReference[ViewTemplatesNames.Count];
            IEnumerator<string> keys = ViewTemplatesNames.Keys.GetEnumerator();
            IEnumerator<Type> values = ViewTemplatesNames.Values.GetEnumerator();

            //TODO: после создания выходит пустой viewReferences
            for (int i = 0; i < ViewTemplatesNames.Keys.Count(); i++)
            {
                    keys.MoveNext();
                    values.MoveNext();
                    refs[i] = new ViewReference(Document, keys.Current, values.Current);
            }
            viewReferences = new ViewReferences(refs);
        }

        public static ViewsSingleton getInstance(Document document)
        {
            if (Instance is null)
            {
                Instance = new ViewsSingleton(document);
            }
            return Instance;
        }

        public void Dispose()
        {
            Instance = null;
        }

        //Все шаблоны видов
        private static readonly Dictionary<string, Type> ViewTemplatesNames = new Dictionary<string, Type>
        {
            {"Форма 4_чертил",typeof(FamilySymbol)},
            {"Форма 6",typeof(FamilySymbol)},

            {"DNS_F_Плитка",typeof(View)},
            {"DNS_КЖ_О_Р_Сетка арматурная",typeof(View)},
            {"DNS_КЖ_О_Р_Сетка арматурная_Внутренний слой",typeof(View)},
            {"DNS_КЖ_О_Р_Сетка арматурная_Наружный слой",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Армирование",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Армирование_Внутренний слой",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Армирование_Внутренний слой_Сечение",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Армирование_Наружный слой",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Армирование_Наружный слой_Сечение",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Армирование_Сечение",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Бийски",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Бийски_Сечение",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Опалубка",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Опалубка_Сечение",typeof(View)},
            {"DNS_КЖ_О_Р_Стеновые панели_Утеплитель",typeof(View)},
            {"DNS_КЖИ_О_Р_Плиты балконные_Армирование",typeof(View)},
            {"DNS_КЖИ_О_Р_Плиты балконные_Армирование_Сечение",typeof(View)},
            {"DNS_КЖИ_О_Р_Плиты балконные_Опалубка",typeof(View)},
            {"DNS_КЖИ_О_Р_Плиты балконные_Опалубка_Сечение",typeof(View)},
            {"Обозначение_Символ ориентации",typeof(View)},
            {"Обозначения_Бийски панелей НС",typeof(View)},
            {"Обозначения_Панели стеновые внутренние",typeof(View)},
            {"Обозначения_Панели стеновые наружные",typeof(View)},
            {"П_Плита перекрытия",typeof(View)},
            {"Примечание_Бийски панелей НС",typeof(View)},
            {"Примечание_Лист 2",typeof(View)},
            {"Примечание_Лист 3",typeof(View)},
            {"Примечание_Лист 5",typeof(View)},
            {"Примечание_Лист 6",typeof(View)},
            {"Примечание_Лист 7",typeof(View)},
            {"Примечание_Лист 8",typeof(View)},
            {"Примечание_Лист 9",typeof(View)},
            {"Примечание_Панели стеновые внутренние",typeof(View)},
            {"Примечание_Панели стеновые наружные",typeof(View)},
            {"Примечание_Панели стеновые парапетные",typeof(View)},
            {"Примечание_Пустотные плиты без схемы",typeof(View)},
            {"Р_Плита перекрытия",typeof(View)},
            {"УГО_Лист 3",typeof(View)},
            {"АКР примечание",typeof(View)},
            {"Схема раскладки плитки (рядовая панель)",typeof(View)},
            {"Условные обозначения",typeof(View)},

            {"3D_Плита перекрытия",typeof(View3D)},
            {"3D_Стеновая панель",typeof(View3D)},

            {"М_Минвата",typeof(ViewSchedule)},
            {"М_Панель стеновая",typeof(ViewSchedule)},
            {"М_Плита балконная",typeof(ViewSchedule)},
            {"М_Плита перекрытия",typeof(ViewSchedule)},
            {"М_Полистирол",typeof(ViewSchedule)},
            {"СА_Балконная плита",typeof(ViewSchedule)},
            {"СА_Масса_Панель стеновая",typeof(ViewSchedule)},
            {"СА_Панель стеновая",typeof(ViewSchedule)},
            {"СА_Плита перекрытия",typeof(ViewSchedule)},
            {"СА_Сетка арматурная",typeof(ViewSchedule)},
            {"СА_Сетка арматурная_Внутренний слой",typeof(ViewSchedule)},
            {"СА_Сетка арматурная_Наружный слой",typeof(ViewSchedule)},
            {"СБ_Панель стеновая",typeof(ViewSchedule)},
            {"СЗД_Балконная плита",typeof(ViewSchedule)},
            {"СЗД_Панель парапета",typeof(ViewSchedule)},
            {"СЗД_Панель стеновая",typeof(ViewSchedule)},
            {"СТА_Балконная плита",typeof(ViewSchedule)},
            {"СТА_Панель стеновая",typeof(ViewSchedule)},
            {"DNS_Ф_Плитка",typeof(ViewSchedule)},
            {"DNS_Марки_Плитка",typeof(ViewSchedule)}
        };
    }
}
