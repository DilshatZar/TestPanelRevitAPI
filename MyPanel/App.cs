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
            string panelName = "�������� ������";
            a.CreateRibbonTab(tabName);
            RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);

            //PushButtonData newButton = new PushButtonData("Test Button", "�� ������\n�����", Assembly.GetExecutingAssembly().Location, "MyPanel.ShowDialogInfoButton");
            //PushButton newBtn = panel.AddItem(newButton) as PushButton;
            //Image img = Properties.Resources.��_�����������;
            //ImageSource imgSrc = ConvertToBitmap(img);
            //newBtn.LargeImage = imgSrc;
            //newBtn.Image = imgSrc;

            PushButtonData newButton1 = new PushButtonData("Task1", "Task1", Assembly.GetExecutingAssembly().Location, "MyPanel.TRGR_FillingApartments");
            PushButton newBtn1 = panel.AddItem(newButton1) as PushButton;
            Image img1 = Properties.Resources.test1_32;
            ImageSource imgSrc1 = ConvertToBitmap(img1);
            newBtn1.LargeImage = imgSrc1;
            newBtn1.Image = imgSrc1;

            //PushButtonData newButton2 = new PushButtonData("Task2", "Task2", Assembly.GetExecutingAssembly().Location, "MyPanel.SelectMaterialsBtn");
            //PushButton newBtn2 = panel.AddItem(newButton2) as PushButton;
            //Image img2 = Properties.Resources.task3;
            //ImageSource imgSrc2 = ConvertToBitmap(img2);
            //newBtn2.LargeImage = imgSrc2;
            //newBtn2.Image = imgSrc2;

            //PushButtonData newButton3 = new PushButtonData("RenameRooms", "������������� �������", Assembly.GetExecutingAssembly().Location, "MyPanel.RenameAllRooms");
            //PushButton newBtn3 = panel.AddItem(newButton3) as PushButton;
            //Image img3 = Properties.Resources.���������_����_32;
            //ImageSource imgSrc3 = Conver(img3);
            //newBtn3.LargeImage = imgSrc3;
            //newBtn3.Image = imgSrc3;

            RibbonPanel apartmnetographyPanel = a.CreateRibbonPanel(tabName, "��������������");

            PushButtonData calcAreas = new PushButtonData("CalculateAreasOfView", "��������������", Assembly.GetExecutingAssembly().Location, "MyPanel.TRGR_Apartmentography");
            calcAreas.ToolTip = "���������� ������� ��������� ��� ������ ��������.";
            calcAreas.LongDescription = "0.1: ���������� ���������� ������ � ������� � �������� ���� � � ������������ ����������� \"ADSK_����� ��������\" � \"ADSK_��� ���������\".";
            PushButton calcAreasBtn = apartmnetographyPanel.AddItem(calcAreas) as PushButton;
            Image roomsImg = Properties.Resources.���������_����_32;
            ImageSource roomsImgBitmap = ConvertToBitmap(roomsImg);
            calcAreasBtn.LargeImage = roomsImgBitmap;
            calcAreasBtn.Image = roomsImgBitmap;

            PushButtonData chgParametersBtnData = new PushButtonData("ChangeConfigSettings", "��������\n��������", Assembly.GetExecutingAssembly().Location, "MyPanel.ChangeConfigSettings");
            chgParametersBtnData.ToolTip = "��������� ���������� ��� ��������������.";
            chgParametersBtnData.LongDescription = "0.1: � ������ ������ ������������ ���� ���� �������� - \"���������� ����� ����� �������\", ����������� ��� ���������� �������� ����������.";
            PushButton chgParametersBtn = apartmnetographyPanel.AddItem(chgParametersBtnData) as PushButton;
            Image chgParametersImg = Properties.Resources.settings_32;
            ImageSource chgParametersImgBitmap = ConvertToBitmap(chgParametersImg);
            chgParametersBtn.LargeImage = chgParametersImgBitmap;
            chgParametersBtn.Image = chgParametersImgBitmap;

            return Result.Succeeded;
        }
        public BitmapImage ConvertToBitmap (Image img)
        {
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
