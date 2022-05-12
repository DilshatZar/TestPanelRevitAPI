#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
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
            string tabName = "Тестовая вкладка";
            string panelName = "Тестовая панель";
            a.CreateRibbonTab(tabName);
            RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);

            PushButtonData newButton = new PushButtonData("Test Button", "ГК Третий\nТрест", Assembly.GetExecutingAssembly().Location, "MyPanel.ShowDialogInfoButton");
            PushButton newBtn = panel.AddItem(newButton) as PushButton;
            Image img = Properties.Resources.ГК_ТретийТрест;
            ImageSource imgSrc = Conver(img);
            newBtn.LargeImage = imgSrc;
            newBtn.Image = imgSrc;

            PushButtonData newButton1 = new PushButtonData("Task1", "Task1", Assembly.GetExecutingAssembly().Location, "MyPanel.TRGR_Apartmentography");
            PushButton newBtn1 = panel.AddItem(newButton1) as PushButton;
            Image img1 = Properties.Resources.test1_32;
            ImageSource imgSrc1 = Conver(img1);
            newBtn1.LargeImage = imgSrc1;
            newBtn1.Image = imgSrc1;

            PushButtonData newButton2 = new PushButtonData("Task2", "Task2", Assembly.GetExecutingAssembly().Location, "MyPanel.SelectMaterialsBtn");
            PushButton newBtn2 = panel.AddItem(newButton2) as PushButton;
            Image img2 = Properties.Resources.task3;
            ImageSource imgSrc2 = Conver(img2);
            newBtn2.LargeImage = imgSrc2;
            newBtn2.Image = imgSrc2;

            //PushButtonData newButton3 = new PushButtonData("RenameRooms", "Переименовать комнаты", Assembly.GetExecutingAssembly().Location, "MyPanel.RenameAllRooms");
            //PushButton newBtn3 = panel.AddItem(newButton3) as PushButton;
            //Image img3 = Properties.Resources.поэтажный_план_32;
            //ImageSource imgSrc3 = Conver(img3);
            //newBtn3.LargeImage = imgSrc3;
            //newBtn3.Image = imgSrc3;

            return Result.Succeeded;
        }
        public BitmapImage Conver (Image img)
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
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
