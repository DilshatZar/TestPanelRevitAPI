using Autodesk.Revit.DB;
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

namespace MyPanel
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PickMaterialsClass : Window
    {
        List<Element> materials = null;
        public PickMaterialsClass(IList<Element> materials)
        {
            InitializeComponent();

            this.materials = materials.ToList();
            foreach (Material material in materials)
            {
                allMaterialsList.Items.Add(material.Name);
            }
            //allMaterialsList.Items.SortDescriptions.Add(
            //    new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }
        private void selectedMaterialsList_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void moveToSelections_Click(object sender, RoutedEventArgs e)
        {
            var selectedMaterials = allMaterialsList.SelectedItems;
            if (selectedMaterials.Count > 0)
            {
                while (selectedMaterials.Count > 0)
                {
                    selectedMaterialsList.Items.Add(selectedMaterials[0]);
                    allMaterialsList.Items.Remove(selectedMaterials[0]);
                }
            }
        }

        private void removeFromSelections_Click(object sender, RoutedEventArgs e)
        {
            var selectedMaterials = selectedMaterialsList.SelectedItems;
            if (selectedMaterials.Count > 0)
            {
                while (selectedMaterials.Count > 0)
                {
                    selectedMaterialsList.Items.Remove(selectedMaterials[0]);
                    allMaterialsList.Items.Add(selectedMaterials[0]);
                }
            }
        }
    }
}
