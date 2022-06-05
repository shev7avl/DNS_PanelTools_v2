using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools.Model
{
    public class FamilyModel : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public ObservableCollection<FamilySymbolModel> FamilySymbols { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
