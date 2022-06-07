using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Legacy.Builders.MarkBuilder;
using DSKPrim.PanelTools.Legacy.Panel;

namespace DSKPrim.PanelTools.Panel
{

    /// <summary>
    /// Base class, that should be inherited by all new panel types.
    /// Panel wraps API Element of Panel (BuiltInCategory.OST_StructuralFraming) with the document.
    /// Allows further wrapping with interfaces to create suitable panel classes.
    /// </summary>
    public class PrecastPanel: IResettable
    {
        public Document ActiveDocument { get; set; }
        public Element ActiveElement { get; set; }
        public AssemblyInstance AssemblyInstance { get; set; }
        public Mark Mark { get; set; }
        public StructureCategory StructureCategory { get; set; }

        public PrecastPanel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
            StructureCategory = new StructureCategory(element);
        }

        /// <summary>
        /// Проверяет равенство двух панелей по значениям "Марка" и прочим логическим условиям
        /// </summary>
        /// <param name="panelMark">Панель для сравнения</param>
        /// <returns></returns>
        public virtual bool Equal(BasePanel panelMark)
        {
            if (Mark == panelMark.Mark)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Заполняет значения короткой и длинной марки
        /// </summary>
        public abstract void CreateMarks();

        /// <summary>
        /// Метод для переназначения значения короткой марки после присвоения индекса
        /// </summary>
        public virtual void SetIndex(int index, bool overwrite = false)
        {

            if (overwrite)
            {
                ShortMark = ShortMark.Split('-')[0];
            }
            ShortMark = $"{ShortMark}-{index}";
            Guid ADSK_panelMark = new Guid("92ae0425-031b-40a9-8904-023f7389963b");
            Transaction transaction = new Transaction(ActiveDocument, $"Назначение индекса: {ShortMark}");
            transaction.Start();
            ActiveElement.get_Parameter(ADSK_panelMark).Set(ShortMark);
            transaction.Commit();

        }

        public virtual void ReadMarks()
        {

            Guid DNS_panelMark1 = new Guid(Properties.Resource.DNS_Полная_марка_изделия);

            Guid ADSK_panelMark1 = new Guid(Properties.Resource.ADSK_Марка_изделия);

            Guid ADSK_panelNum = new Guid(Properties.Resource.ADSK_Номер_изделия);

            try
            {
                LongMark = ActiveElement.get_Parameter(DNS_panelMark1).ToString();
            }
            catch (NullReferenceException)
            {
                Debug.WriteLine("ОШИБКА: Не найден параметр DNS_Полная_марка_изделия");
                Debug.WriteLine($"Нужный GUID: {Properties.Resource.DNS_Полная_марка_изделия}");
                Debug.WriteLine("Проверьте шаблон и ФОП");
                Debug.WriteLine("Завершение процедуры с ошибкой");
                throw;
            }

            try
            {
                ShortMark = ActiveElement.get_Parameter(ADSK_panelMark1).AsString();
            }
            catch (Exception)
            {
                Debug.WriteLine("ОШИБКА: Не найден параметр ADSK_Марка_изделия");
                Debug.WriteLine($"Нужный GUID: {Properties.Resource.ADSK_Марка_изделия}");
                Debug.WriteLine("Проверьте шаблон и ФОП");
                Debug.WriteLine("Завершение процедуры с ошибкой");
            }

            try
            {
                Index = ActiveElement.get_Parameter(ADSK_panelNum).AsString();
            }
            catch (Exception)
            {
                Debug.WriteLine("ОШИБКА: Не найден параметр ADSK_Номер_изделия");
                Debug.WriteLine($"Нужный GUID: {Properties.Resource.ADSK_Номер_изделия}");
                Debug.WriteLine("Проверьте шаблон и ФОП");
                Debug.WriteLine("Завершение процедуры с ошибкой");

            }

            if (ActiveElement.AssemblyInstanceId.IntegerValue == -1)
            {
                AssemblyInstance = null;
            }
            else
            {
                AssemblyInstance = (AssemblyInstance) ActiveDocument.GetElement(ActiveElement.AssemblyInstanceId);
            }
        }

        public virtual void SetMarks(string positionEnum)
        {

            Guid DNS_panelMark1 = new Guid(Properties.Resource.DNS_Полная_марка_изделия);

            Transaction transaction = new Transaction(ActiveDocument);

            transaction.Start($"Транзакция - {ActiveElement.Name}");

            ActiveElement.get_Parameter(DNS_panelMark1).Set(LongMark);

            ActiveElement.get_Parameter(new Guid(Properties.Resource.ADSK_Марка_изделия)).Set(ShortMark);

            string name = ActiveElement.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString();
            string code = $"{name.Substring(name.Length - 2, 2)}.{positionEnum}";
            ActiveElement.get_Parameter(new Guid(Properties.Resource.DNS_Марка_элемента)).Set(code);

            transaction.Commit();
        }

        public void Reset()
        {
            ActiveDocument = null ;
            ActiveElement = null;
            if (AssemblyInstance != null)
            {
                AssemblyInstance = null;
            }
            LongMark = default;
            ShortMark = default;
            Index = default;
        }
    }
}
