using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.MVVM.ViewModel
{
    public class AssemblyViewModel : INotifyPropertyChanged
    {
        private AssemblyModel selectedAssembly;
        private ExternalCommandData _externalCommandData;

        public ObservableCollection<AssemblyModel> Assemblies { get; set; }
        public AssemblyModel SelectedAssembly
        {
            get { return selectedAssembly; }
            set
            {
                selectedAssembly = value;
                OnPropertyChanged("SelectedPhone");
            }
        }

        public AssemblyViewModel(ExternalCommandData commandData)
        {
            _externalCommandData = commandData;
            Document document = _externalCommandData.Application.ActiveUIDocument.Document;
            Assemblies = new ObservableCollection<AssemblyModel>(
                new FilteredElementCollector(document).
                OfClass(typeof(AssemblyType)).
                Select(o => new AssemblyModel { Name = o.Name }).ToList()); 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void Export()
        {
            Document doc = _externalCommandData.Application.ActiveUIDocument.Document;
            AssemblyInstance assembly = new FilteredElementCollector(doc).
                OfClass(typeof(AssemblyInstance)).Cast<AssemblyInstance>().
                Where(o => o.IsValidObject).
                First(o => o.Name == selectedAssembly.Name);
            ElementId id = assembly.Id;

            List<ViewSchedule> viewSchedules = new FilteredElementCollector(doc).
                OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>().
                Where(o => o.IsValidObject).
                Where(o => o.AssociatedAssemblyInstanceId == id).ToList();

            ViewScheduleExportOptions exportOptions = new ViewScheduleExportOptions();

            foreach (ViewSchedule viewSchedule in viewSchedules)
            {
                string path = "C:\\Export\\";
                string name = String.Format("{0}-{1}_{2}.txt",
                    assembly.Name.Split(' ')[0],
                    assembly.Name.Split('-').Last(),
                    viewSchedule.Name);
                viewSchedule.Export(path, name, exportOptions);
            }
        }
    }
}
