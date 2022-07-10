using Autodesk.Revit.UI;
using DSKPrim.PanelTools.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSKPrim.PanelTools.GUI
{
    /// <summary>
    /// Логика взаимодействия для ExportSchedules.xaml
    /// </summary>
    public partial class ExportSchedules : Window
    {
        private AssemblyViewModel _assemblyViewModel;

        public ExportSchedules(ExternalCommandData commandData)
        {
            InitializeComponent();

            _assemblyViewModel = new AssemblyViewModel(commandData);
            DataContext = _assemblyViewModel;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            _assemblyViewModel.Export();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FileDialog_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
