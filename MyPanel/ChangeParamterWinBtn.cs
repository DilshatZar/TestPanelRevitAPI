#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace MyPanel
{
    [Transaction(TransactionMode.Manual)]
    public class ChangeParamterWinBtn : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            Selection sel = uidoc.Selection;
            List<Element> selections = new List<Element>();
            foreach (ElementId elementId in sel.GetElementIds())
            {
                selections.Add(doc.GetElement(elementId));
            }
            if (selections.Count > 0)
            {
                ChangeParameterWindow window = new ChangeParameterWindow(selections[0], doc);
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите один элемент!");
            }

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Transaction Name");
                tx.Commit();
            }

            Debug.Print("Complited the task.");
            return Result.Succeeded;
        }
    }
}
