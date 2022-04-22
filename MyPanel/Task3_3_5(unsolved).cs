#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace MyPanel
{
    [Transaction(TransactionMode.Manual)]
    public class Task3_3_5 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            Selection sel = uidoc.Selection;

            // Retrieve elements from database

            IList<Element> familyInstances = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).ToElements();
            IList<Element> familySymbols = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).ToElements();

            int ids = 0;
            foreach (FamilyInstance element in familyInstances)
            {
                if (element.Symbol.Family.Id.IntegerValue <= 100000)
                {
                    Debug.Print($"{element.Symbol.Family.Id.IntegerValue}");
                    ids += element.Id.IntegerValue;
                }
            }
            foreach (FamilySymbol element in familySymbols)
            {
                if (element.Family.Id.IntegerValue > 100000)
                {
                    Debug.Print($"{element.Family.Id.IntegerValue}");
                    ids += element.Id.IntegerValue;
                }
            }

            AnswerWindow answerWindow = new AnswerWindow(ids);
            answerWindow.ShowDialog();

            Debug.Print("Complited the task3_3_5.");
            return Result.Succeeded;
        }
    }
}
