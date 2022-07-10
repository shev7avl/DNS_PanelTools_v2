using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSKPrim.PanelTools.Model;

namespace DSKPrim.PanelTools.ViewModel
{
    public class FamilyViewModel : INotifyPropertyChanged
    {
        private FamilyModel _selectedModel;

        public FamilyModel SelectedModel { get { return _selectedModel; } set { _selectedModel = value; } }

        public ObservableCollection<FamilySymbolModel> SelectedModelSymbols { get { return _selectedModel.FamilySymbols; } }


        public ObservableCollection<FamilyModel> FamilyModels { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        
    }
}
