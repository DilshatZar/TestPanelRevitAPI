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
    public class Task3_2_5 : IExternalCommand
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

            IList<Element> col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().ToElements();
            IList<Room> allRooms = new List<Room>();
            IList<Room> instuctionRooms = new List<Room>();
            foreach (Room el in col)
            {
                allRooms.Add(el);
                if (el.Name.Contains("Instruction"))
                {
                    instuctionRooms.Add(el);
                }
            }
            using (Transaction t = new Transaction(doc, "Изменение верхнего смещения комнат Instuction"))
            {
                t.Start();
                foreach (Room room in instuctionRooms)
                {
                    if (room.Level.Name.Contains("01"))
                    {
                        double upperOffset = UnitUtils.ConvertToInternalUnits(3000.0, UnitTypeId.Millimeters);
                        room.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET).Set(upperOffset);
                    }
                    else if (room.Level.Name.Contains("02"))
                    {
                        double upperOffset = UnitUtils.ConvertToInternalUnits(2800.0, UnitTypeId.Millimeters);
                        room.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET).Set(upperOffset);
                    }
                    else if (room.Level.Name.Contains("03"))
                    {
                        double upperOffset = UnitUtils.ConvertToInternalUnits(2500.0, UnitTypeId.Millimeters);
                        room.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET).Set(upperOffset);
                    }
                }
                t.Commit();
            }

            double allRoomsVolume = 0;
            foreach (Room room in allRooms)
            {
                double internalValue = room.get_Parameter(BuiltInParameter.ROOM_VOLUME).AsDouble();
                double externalValue = UnitUtils.ConvertFromInternalUnits(internalValue, UnitTypeId.CubicMeters);
                allRoomsVolume += externalValue;
            }

            Debug.Print($"{Math.Round(allRoomsVolume)}");
            Debug.Print("Complited the task3_2_5.");
            return Result.Succeeded;
        }
    }
}
