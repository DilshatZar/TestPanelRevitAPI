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
    public class TRGR_Apartmentography : IExternalCommand
    {
        private static AnswerWindow ans = new AnswerWindow();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            RevitApplication app = uiapp.Application;
            Document doc = uidoc.Document;

            FilteredElementCollector areas = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();
            Dictionary<string, List<Room>> apartments = new Dictionary<string, List<Room>>();

            IList<IList<object>> mistakes = new List<IList<object>>();

            ConfigSettingsWindow config = new ConfigSettingsWindow();
            int roundNum = 2;
            double loggieAreaCoef = 0.5;
            double balconyAreaCoef = 0.3;
            double defaultAreaCoef = 1.0;
            try
            {
                roundNum = int.Parse(config.GetParameterValue("ROUNDING_NUMBER"));
                loggieAreaCoef = double.Parse(config.GetParameterValue("LoggiaAreaCoef").Replace(".", ","));
                balconyAreaCoef = double.Parse(config.GetParameterValue("BalconyAreaCoef").Replace(".", ","));
                defaultAreaCoef = double.Parse(config.GetParameterValue("DefaultAreaCoef").Replace(".", ","));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("������������� �������� �� ��������� ��� ����������: \n" +
                    "\"����� ����� �������\": 2\n" +
                    "\"����������� ������� ������\": 0.5\n" +
                    "\"����������� ������� �������\": 0.3\n" +
                    "\"����������� ������� ������� ���������\": 1.0\n", "������ ���������� ����������.");
                config.SetParameterValue("ROUNDING_NUMBER", "2");
                config.SetParameterValue("LoggiaAreaCoef", "0.5");
                config.SetParameterValue("BalconyAreaCoef", "0.3");
                config.SetParameterValue("DefaultAreaCoef", "1.0");
            }

            foreach (Room room in areas)
            {
                if (room.LookupParameter("ADSK_����� ��������").AsString() != null)
                {
                    if (!apartments.ContainsKey(room.LookupParameter("ADSK_����� ��������").AsString()))
                    {
                        apartments.Add(room.LookupParameter("ADSK_����� ��������").AsString(), new List<Room>());
                    }
                    apartments[room.LookupParameter("ADSK_����� ��������").AsString()].Add(room);
                }
            }

            using (Transaction t = new Transaction(doc, "��������������"))
            {
                t.Start();
                foreach (string num in apartments.Keys)
                {
                    int numberOfLivingRooms = 0;
                    double apartmentAreaLivingRooms = 0;            // ADSK_������� �������� �����
                    double apartmaneAreaWithoutSummerRooms = 0;     // ADSK_������� ��������
                    double apartmentAreaGeneral = 0;                // ADSK_������� �������� �����
                    double apartmentAreaGeneralWithoutCoef = 0;     // TRGR_������� �������� ��� ��
                    foreach (Room room in apartments[num])
                    {
                        double areaOfRoom = Math.Round(UnitUtils.ConvertFromInternalUnits(room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble(), UnitTypeId.SquareMeters), roundNum);
                        try
                        {
                            room.LookupParameter("TRGR_������� ���������").Set(UnitUtils.ConvertToInternalUnits(Math.Round(areaOfRoom, roundNum), UnitTypeId.SquareMeters));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                        try
                        {
                            int roomType = room.LookupParameter("ADSK_��� ���������").AsInteger();

                            double coefficent = room.LookupParameter("ADSK_����������� �������").AsDouble();

                            if (roomType != 3 && roomType != 4)
                            {
                                apartmaneAreaWithoutSummerRooms += areaOfRoom;
                                apartmentAreaGeneral += areaOfRoom;
                                if (coefficent != defaultAreaCoef)
                                {
                                    mistakes.Add(new List<object> { room.Id.IntegerValue, coefficent, defaultAreaCoef});
                                    coefficent = defaultAreaCoef;
                                    room.LookupParameter("ADSK_����������� �������").Set(defaultAreaCoef);
                                }
                            }
                            if (roomType == 3 || roomType == 4)
                            {
                                if (roomType == 3 && coefficent != loggieAreaCoef)
                                {
                                    mistakes.Add(new List<object> {room.Id.IntegerValue, coefficent, loggieAreaCoef});
                                    coefficent = loggieAreaCoef;
                                    room.LookupParameter("ADSK_����������� �������").Set(loggieAreaCoef);
                                }
                                else if (roomType == 4 && coefficent != balconyAreaCoef)
                                {
                                    mistakes.Add(new List<object> {room.Id.IntegerValue, coefficent, balconyAreaCoef});
                                    coefficent = balconyAreaCoef;
                                    room.LookupParameter("ADSK_����������� �������").Set(balconyAreaCoef);
                                }
                                apartmentAreaGeneral += Math.Round(areaOfRoom * coefficent, roundNum);
                            }
                            if (roomType == 1)
                            {
                                numberOfLivingRooms++;
                                apartmentAreaLivingRooms += areaOfRoom;
                            }
                            room.LookupParameter("ADSK_������� � �������������").Set(UnitUtils.ConvertToInternalUnits(Math.Round(areaOfRoom * coefficent, roundNum), UnitTypeId.SquareMeters));
                            apartmentAreaGeneralWithoutCoef += areaOfRoom;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                    apartmaneAreaWithoutSummerRooms = UnitUtils.ConvertToInternalUnits(Math.Round(apartmaneAreaWithoutSummerRooms, roundNum), UnitTypeId.SquareMeters);
                    apartmentAreaLivingRooms = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaLivingRooms, roundNum), UnitTypeId.SquareMeters);
                    apartmentAreaGeneral = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaGeneral, roundNum), UnitTypeId.SquareMeters);
                    apartmentAreaGeneralWithoutCoef = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaGeneralWithoutCoef, roundNum), UnitTypeId.SquareMeters);
                    foreach (Room room in apartments[num])
                    {
                        try
                        {
                            room.LookupParameter("ADSK_���������� ������").Set(numberOfLivingRooms);
                            room.LookupParameter("ADSK_������� ��������").Set(apartmaneAreaWithoutSummerRooms);
                            room.LookupParameter("ADSK_������� �������� �����").Set(apartmentAreaLivingRooms);
                            room.LookupParameter("ADSK_������� �������� �����").Set(apartmentAreaGeneral);
                            room.LookupParameter("TRGR_������� �������� ��� ��").Set(apartmentAreaGeneralWithoutCoef);
                            CreateRoomTag(room, doc, 159742);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
                t.Commit();
            }
            if (mistakes.Count > 0)
            {
                TRGR_RoomIdListWindow roomIdWin = new TRGR_RoomIdListWindow();
                for (int i = 0; i < mistakes.Count; i++)
                {
                    roomIdWin.AddNewLine((int)mistakes[i][0], (double)mistakes[i][1], (double)mistakes[i][2]);
                }
                roomIdWin.Show();
            }
            return Result.Succeeded;
        }
        private static void CreateRoomTag(Room room, Document doc, int tagType)
        {
            RoomTag tag = doc.Create.NewRoomTag(new LinkElementId(room.Id), new UV(), null);
            tag.ChangeTypeId(new ElementId(tagType));
            ans.WriteLine(tag.Id.IntegerValue.ToString());
        }
    }
}
