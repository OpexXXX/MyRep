using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoonPdfLib;
using MoonPdfLib.Helper;
using MoonPdfLib.MuPdf;
using Microsoft.Win32;
using System.Data.SQLite;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SAPFEWSELib;
using System.Threading;
using MyApp.Model;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using ATPWork.MyApp.ViewModel.AtpEditor;
using ATPWork.MyApp.ViewModel.PlombEditorVm;

namespace MyApp
{
    

    public partial class MainWindow : Window
    {
        internal MoonPdfPanel MoonPdfPanel { get { return this.moonPdfPanel; } }
        public MainWindow()
        {
            InitializeComponent();
            AktTehProverki akt = new AktTehProverki(1, new List<int> { 1, 2 }, "123.pdf");
            akt.Plombs.Add(new Plomba("2400_4", 12, "ÂÊÀ", false));
            akt.Plombs.Add(new Plomba("2400_5", 13, "ÂÊÀ", false));
            akt.Plombs.Add(new Plomba("2400_6", 14, "ÂÊÀ", false));

            AtpEditor ate = new AtpEditor(AtpEditorR);
            ate.AktInWork = akt;

            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }


    }
}




