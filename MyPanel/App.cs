#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endregion

namespace MyPanel
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            string tabName = "������ �����";
            //string panelName = "�������� ������";
            a.CreateRibbonTab(tabName);
            //RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);

            RibbonPanel apartmnetographyPanel = a.CreateRibbonPanel(tabName, "��������������");

            PushButtonData genRooms = new PushButtonData("RoomsGen", "���������\n� ����������\n���������", Assembly.GetExecutingAssembly().Location, typeof(TRGR_RoomsGeneration).FullName);
            genRooms.ToolTip = "��������� � ����������� ����� ���������. " +
                "���������� ��������� ��� ������ �������� � ������������ ������ �������� ��� ������� ��������� � ������� �����.";
            genRooms.LongDescription = "������������ ����� ���� ���������, ���: " +
                "\"����� �������\", \"�������\", \"������\", \"������\", \"�����\", \"�����������\", \"�.�.\". " +
                "��� ����������� ���� ������������ ����, �����, � ����� ����������� � ��������� ���������� ��������� PlumbingFixtures." + 
                "��������� �������� �������������� �� ������� �����, �������� ����������� ��������� �������� ������������� \"�����.����������\". " +
                "������������ ��������� \"ADSK_����� ��������\" � \"ADSK_����\"";
            PushButton genRoomsBtn = apartmnetographyPanel.AddItem(genRooms) as PushButton;
            //Image genRoomsImg = Properties.Resources.���������_����_32;
            //ImageSource genRoomsImgBitmap = ConvertToBitmap(genRoomsImg);
            //genRoomsBtn.LargeImage = genRoomsImgBitmap;
            //genRoomsBtn.Image = genRoomsImgBitmap;

            //PushButtonData fillApartments = new PushButtonData("FillApartments", "����������\n�������", Assembly.GetExecutingAssembly().Location, typeof(TRGR_FillingApartments).FullName);
            //fillApartments.ToolTip = "���������� ��������� ��� ������ �������� � ������������ ������ �������� ��� ������� ��������� � ������� �����.";
            //fillApartments.LongDescription = "��������� �������� �������������� �� ������� �����, �������� ����������� ��������� �������� ������������� \"�����.����������\". " +
            //    "������������ ��������� \"ADSK_����� ��������\" � \"ADSK_����\"";
            //PushButton fillApartmentsBtn = apartmnetographyPanel.AddItem(fillApartments) as PushButton;
            ////Image fillApartmentsImg = Properties.Resources.���������_����_32;
            ////ImageSource fillApartmentsBitmap = ConvertToBitmap(fillApartmentsImg);
            ////fillApartmentsBtn.LargeImage = fillApartmentsBitmap;
            ////fillApartmentsBtn.Image = fillApartmentsBitmap;

            PushButtonData calcAreas = new PushButtonData("CalculateAreas", "��������������", Assembly.GetExecutingAssembly().Location, typeof(TRGR_Apartmentography).FullName);
            calcAreas.ToolTip = "���������� ������� ��������� ��� ������ ��������.";
            calcAreas.LongDescription = "���������� ���������� ������ � ������� � �������� ���� � � ������������ ����������� " +
                "\"ADSK_����� ��������\", \"ADSK_��� ���������\", � \"ADSK_����������� �������\". " +
                "������������ �������� ��������� \"ADSK_����������� �������\" �� ������� ������, ��� ������� �������, ��������� ���� � ������������� ID ��������� � ���������������� �������������. " +
                "������������ ��������� ���� ������������ �� ����� ����������. " +
                "� �����: �������� ���������� �������������� ����� ��� �������� ������� ��������� � ����� ��������������.";
            PushButton calcAreasBtn = apartmnetographyPanel.AddItem(calcAreas) as PushButton;
            Image calcAreasImg = Properties.Resources.���������_����_32;
            calcAreasBtn.LargeImage = ConvertToBitmap(calcAreasImg, new Size(32, 32));
            calcAreasBtn.Image = ConvertToBitmap(calcAreasImg , new Size(16, 16));

            PushButtonData chgParametersBtnData = new PushButtonData("ChangeConfigSettings", "��������\n��������", Assembly.GetExecutingAssembly().Location, typeof(ChangeConfigSettings).FullName);
            chgParametersBtnData.ToolTip = "��������� ���������� ��� ��������������.";
            chgParametersBtnData.LongDescription = "������������ ��� �������������� ����� ����������, ���: \"���������� ����� ����� �������\", ����������� ��� ���������� �������� ����������; " +
                "\"����������� ������� ������\"; \"����������� ������� �������\"; \"����������� ������� ������� ���������\"; \"�������� ������� ������\"; \"�������� ������� �����\".";
            PushButton chgParametersBtn = apartmnetographyPanel.AddItem(chgParametersBtnData) as PushButton;
            Image chgParametersImg = Properties.Resources.settings_32;
            chgParametersBtn.LargeImage = ConvertToBitmap(chgParametersImg, new Size(32, 32));
            chgParametersBtn.Image = ConvertToBitmap(chgParametersImg, new Size(16, 16));

            return Result.Succeeded;
        }
        public BitmapImage ConvertToBitmap (Image img, Size size)
        {
            img = (Image)(new Bitmap(img, size));
            using(MemoryStream memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public string InsertNewLines(string str, int rowLength)
        {
            for (int i = rowLength - 1; i < str.Length; i += rowLength)
            {
                if (str[i] != ' ')
                {
                    int leftSpace = str.LastIndexOf(' ', i);
                    int rightSpace = str.IndexOf(' ', i);
                    string part1 = str.Substring(leftSpace, i - leftSpace);
                    string part2 = str.Substring(rightSpace, rightSpace - i);
                    if (part1.Length > part2.Length)
                    {
                        str = str.Insert(rightSpace + 1, "\n");
                    }
                    else
                    {
                        str = str.Insert(leftSpace + 1, "\n");
                    }
                }
                else
                {
                    str = str.Insert(i + 1, "\n");
                }
            }
            return str;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
