using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
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
    public partial class ChangeParameterWindow : Window
    {
        Element element;
        Document doc;
        Dictionary<string, Definition> parameterNameAndId;
        public ChangeParameterWindow(Element sel, Document doc)
        {
            InitializeComponent();
            this.doc = doc;
            element = sel;

            IList<Parameter> parametersList = new List<Parameter>();
            
            parameterNameAndId = new Dictionary<string, Definition>();
            foreach (Parameter parameter in element.Parameters)
            {
                parametersList.Add(parameter);
            }
            Parameter[] paramteretsArray = parametersList.ToArray();
            Array.Sort(paramteretsArray, new CompareParametersInterface());

            foreach (Parameter parameter in paramteretsArray)
            {
                if (parameter.StorageType == StorageType.String)
                {
                    ParametersList.Items.Add(parameter.Definition.Name);
                    parameterNameAndId[parameter.Definition.Name] = parameter.Definition;
                }
            }
            ParametersList.SelectedIndex = 0;
            ItemName.Text = element.Name;
        }
        public void ConfirmChange_Click(object sender, RoutedEventArgs e)
        {
            string parameterName = ParametersList.SelectedItem.ToString();
            string newValue = ParameterValueTxtbox.Text;
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Ручное изменение строкового параметра");
                element.get_Parameter(parameterNameAndId[parameterName]).Set(newValue);
                t.Commit();
            }
            MessageBox.Show("Изменение принято.");
        }

        private void ParametersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string parameterName = ParametersList.SelectedItem.ToString();
            ParameterValueTxtbox.Text = element.get_Parameter(parameterNameAndId[parameterName]).AsString();
        }
    }
}
