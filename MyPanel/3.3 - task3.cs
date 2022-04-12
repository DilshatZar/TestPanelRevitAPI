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

#endregion

namespace MyPanel
{
    [Transaction(TransactionMode.Manual)]
    public class CountFamilyIds : IExternalCommand
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

            FilteredElementCollector familyCol = new FilteredElementCollector(doc).OfClass(typeof(Family));
            IList<Element> familySymbolCol = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).ToElements();
            IList<Element> familyInstanceCol = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).ToElements();

            List<Element> familyArray = new List<Element>();
            foreach (Element family in familyCol)
            {
                foreach (FamilyInstance familyInstance in familyInstanceCol)
                {
                    if (!familyArray.Contains(familyInstance) && family.Name == familyInstance.Name)
                    {
                        familyArray.Add(family);
                    }
                }
            }
            int counter = 0;
            int[] cnt = new int[] {0, 0, 0, 0};
            foreach (Element family in familyCol)
            {
                if (family.Id.IntegerValue > 100000)
                {
                    cnt[0]++;
                    foreach (FamilySymbol familySymbol in familySymbolCol)
                    {
                        if (family.Name == familySymbol.FamilyName)
                        {
                            cnt[1]++;
                            counter += familySymbol.Id.IntegerValue;
                        }
                    }
                }
                else
                {
                    cnt[2]++;
                    foreach (FamilyInstance familyInstance in familyInstanceCol)
                    {
                        if (familyInstance.Name == family.Name)
                        {
                            cnt[3]++;
                            counter += familyInstance.Id.IntegerValue;
                        }
                    }
                }
            }


            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Transaction Name");
                tx.Commit();
            }
            Debug.Print($"----------------------------------");
            Debug.Print($"{cnt[0]} {cnt[1]} {cnt[2]} {cnt[3]}");
            Debug.Print($"----------------------------------");
            Debug.Print($"{familyArray.Count}");
            Debug.Print($"{familyInstanceCol.Count}");
            Debug.Print($"{familySymbolCol.Count}");
            Debug.Print($"{counter}");
            Debug.Print("Complited the task.");
            return Result.Succeeded;
        }

    }
}
