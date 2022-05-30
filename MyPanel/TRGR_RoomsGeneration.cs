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
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using System.Windows;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace MyPanel
{
    [Transaction(TransactionMode.Manual)]
    public class TRGR_RoomsGeneration : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            RevitApplication app = uiapp.Application;
            Document doc = uidoc.Document;

            IList<Element> roomsToRemove = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().ToList();
            if (roomsToRemove.Count > 0)
            {
                using (Transaction t = new Transaction(doc, "�������� ������������ ���������"))
                { // ������� �������� �� ������� ���������� ������
                    t.Start();
                    foreach (Room room in roomsToRemove)
                    {
                        doc.Delete(room.Id);
                    }
                    t.Commit();
                }
            }

            List<Room> rooms = new List<Room>();
            List<Phase> phases = new List<Phase>();
            using (Transaction t = new Transaction(doc))
            {
                t.Start("��������� ���������");

                int roomUp = 3300;
                int roomDown = 0;
                foreach (Phase phase in doc.Phases)
                {
                    phases.Add(phase);
                }

                Level level = doc.ActiveView.GenLevel;

                foreach (ElementId roomId in doc.Create.NewRooms2(level))
                {
                    rooms.Add(doc.GetElement(roomId) as Room);
                }

                t.Commit();

                t.Start("����� ���� ���������");
                FilteredElementCollector roomtags = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_RoomTags).WhereElementIsNotElementType();
                foreach (Room room in rooms)
                {
                    foreach (RoomTag roomTag in roomtags)
                    {
                        if (room.Id.IntegerValue == roomTag.Room.Id.IntegerValue)
                        {
                            roomTag.ChangeTypeId(new ElementId(159738));
                        }
                    }
                    room.get_Parameter(BuiltInParameter.ROOM_UPPER_OFFSET).Set(UnitUtils.ConvertToInternalUnits(roomUp, UnitTypeId.Millimeters));
                    room.get_Parameter(BuiltInParameter.ROOM_LOWER_OFFSET).Set(UnitUtils.ConvertToInternalUnits(roomDown, UnitTypeId.Millimeters));
                }
                t.Commit();
            }

            FilteredElementCollector plumbingFixtures = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_PlumbingFixtures).WhereElementIsNotElementType();

            List<FamilyInstance> kitchenPlumbs = new List<FamilyInstance>();
            List<FamilyInstance> toiletBowls = new List<FamilyInstance>();
            List<FamilyInstance> sinks = new List<FamilyInstance>();
            List<FamilyInstance> washers = new List<FamilyInstance>();
            List<FamilyInstance> baths = new List<FamilyInstance>();

            foreach (FamilyInstance fixture in plumbingFixtures)
            {
                string fixtureName = fixture.Symbol.Name;
                if (fixtureName.Contains("��������_������"))
                {
                    kitchenPlumbs.Add(fixture);
                }
                else if (fixtureName.Contains("������"))
                {
                    toiletBowls.Add(fixture);
                }
                else if (fixtureName.Contains("����������"))
                {
                    sinks.Add(fixture);
                }
                else if (fixtureName.Contains("����������_������"))
                {
                    washers.Add(fixture);
                }
                else if (fixtureName.Contains("������") || fixtureName.Contains("�������������"))
                {
                    baths.Add(fixture);
                }
            }

            FilteredElementCollector windows = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsNotElementType();
            FilteredElementCollector doors = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType();

            ConfigSettingsWindow config = new ConfigSettingsWindow();

            double loggieAreaCoef = 0.5;
            double balconyAreaCoef = 0.3;
            try
            {
                loggieAreaCoef = double.Parse(config.GetParameterValue("LoggiaAreaCoef").Replace(".", ","));
                balconyAreaCoef = double.Parse(config.GetParameterValue("BalconyAreaCoef").Replace(".", ","));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("������������� �������� �� ��������� ��� ����������: \n\"����������� ������� ������\": 0.5 \n\"����������� ������� �������\": 0.3", "������ ���������� ����������.");
                config.SetParameterValue("LoggiaAreaCoef", "0.5");
                config.SetParameterValue("BalconyAreaCoef", "0.3");
            }

            using (Transaction t = new Transaction(doc, "����������� ���������"))
            {
                t.Start();
                foreach (Room room in rooms)
                {
                    List<string> roomFixtures = new List<string>();
                    foreach (FamilyInstance fixture in kitchenPlumbs)
                    {
                        XYZ located = (fixture.Location as LocationPoint).Point;
                        if (room.IsPointInRoom(located))
                        {
                            roomFixtures.Add("�����");
                        }
                    }
                    foreach (FamilyInstance fixture in toiletBowls)
                    {
                        if (room.IsPointInRoom((fixture.Location as LocationPoint).Point))
                        {
                            roomFixtures.Add("������");
                        }
                    }
                    foreach (FamilyInstance fixture in sinks)
                    {
                        if (room.IsPointInRoom((fixture.Location as LocationPoint).Point))
                        {
                            roomFixtures.Add("����������");
                        }
                    }
                    foreach (FamilyInstance fixture in washers)
                    {
                        if (room.IsPointInRoom((fixture.Location as LocationPoint).Point))
                        {
                            roomFixtures.Add("���������� ������");
                        }
                    }
                    foreach (FamilyInstance fixture in baths)
                    {
                        if (room.IsPointInRoom((fixture.Location as LocationPoint).Point))
                        {
                            roomFixtures.Add("�����");
                        }
                    }

                    if (roomFixtures.Contains("������") && roomFixtures.Contains("�����"))
                    {
                        room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("�.�.");
                    }
                    else if (!roomFixtures.Contains("�����") && roomFixtures.Contains("����������") && roomFixtures.Contains("������"))
                    {
                        room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("�������");
                    }
                    else if (!roomFixtures.Contains("������") && roomFixtures.Contains("�����"))
                    {
                        room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("�����");
                    }
                    else if (!roomFixtures.Contains("�����") && !roomFixtures.Contains("����������") && !roomFixtures.Contains("������") && roomFixtures.Contains("���������� ������"))
                    {
                        room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("�����������");
                    }
                    else if (roomFixtures.Contains("�����"))
                    {
                        room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("�����");
                    } 

                    List<int> roomWindowsIds = new List<int>();
                    foreach (FamilyInstance window in windows)
                    {
                        Room windowToRoom = window.get_ToRoom(phases[phases.Count - 1]);
                        if (windowToRoom != null)
                        {
                            roomWindowsIds.Add(windowToRoom.Id.IntegerValue);
                        }
                    }
                    foreach (int elementId in roomWindowsIds.Distinct())
                    {
                        if (room.Id.IntegerValue == elementId && room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString().Contains("���������"))
                        {
                            room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("����� �������");
                        }
                    }
                    if (room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString() == "���������")
                    {
                        room.get_Parameter(BuiltInParameter.ROOM_NAME).Set("�������");
                    }
                    room.LookupParameter("ADSK_����������� �������").Set(1);
                }
                foreach (FamilyInstance door in doors)
                {
                    Room doorFromRoom = door.get_FromRoom(phases[phases.Count - 1]);
                    if (doorFromRoom != null && door.Symbol.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).AsString().Contains("���������"))
                    {
                        doorFromRoom.get_Parameter(BuiltInParameter.ROOM_NAME).Set("������");
                        doorFromRoom.LookupParameter("ADSK_����������� �������").Set(loggieAreaCoef);
                    }
                }
                t.Commit();
            }
            Debug.Print("Complited the task.");
            return Result.Succeeded;
        }
    }
}
