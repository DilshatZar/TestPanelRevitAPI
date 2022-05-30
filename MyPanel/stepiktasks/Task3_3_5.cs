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

            FilteredElementCollector familyInstances = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType();
            IList<Family> families = new List<Family>();
            IList<string> familyNames = new List<string>();

            int ids = 0;
            foreach (FamilyInstance element in familyInstances)
            {
                if (element.Symbol.Family.Id.IntegerValue <= 100000)
                {
                    ids += element.Id.IntegerValue;
                }
                else if(!familyNames.Contains(element.Symbol.FamilyName))
                {
                    families.Add(element.Symbol.Family);
                    familyNames.Add(element.Symbol.FamilyName);
                }
            }

            foreach (Family family in families)
            {
                foreach (ElementId elementId in family.GetFamilySymbolIds())
                {
                    FamilySymbol familySymbol = doc.GetElement(elementId) as FamilySymbol;
                    ids += familySymbol.Id.IntegerValue;
                }
            }

            AnswerWindow answerWindow = new AnswerWindow(ids);
            answerWindow.Show();

            Debug.Print("Complited the task3_3_5.");
            return Result.Succeeded;
        }
    }
}
