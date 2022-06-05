using DSKPrim.PanelTools.Model;
using DSKPrim.PanelTools.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Data
{
    internal class FakeDataFactory
    {
        public static FamilyViewModel CreateViewModel()
        {
            FamilyViewModel viewModel = new FamilyViewModel();

            viewModel.FamilyModels = new ObservableCollection<FamilyModel>
            {
                new FamilyModel {
                    Name = "ЗД-1",
                    FamilySymbols = new ObservableCollection<FamilySymbolModel>
                    {
                        new FamilySymbolModel { Name = "ЗД-1"},
                        new FamilySymbolModel { Name = "ЗД-1.1"},
                        new FamilySymbolModel { Name = "ЗД-1.2"}
                    }},
                new FamilyModel {
                    Name = "ЗД-5",
                    FamilySymbols = new ObservableCollection<FamilySymbolModel>
                    {
                        new FamilySymbolModel { Name = "ЗД-5.1"},
                        new FamilySymbolModel { Name = "ЗД-5.2"}
                    }},
                new FamilyModel {
                    Name = "ЗД-10",
                    FamilySymbols = new ObservableCollection<FamilySymbolModel>
                    {
                        new FamilySymbolModel { Name = "ЗД-10"},
                    }}
            };

            return viewModel;
        }

    }
}
