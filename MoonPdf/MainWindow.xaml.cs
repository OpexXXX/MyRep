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
using ATPWork.MyApp.ViewModel;
using ATPWork.MyApp.ViewModel.PlombEditorVm;

namespace MyApp
{
    

    public partial class MainWindow : Window
    {
        internal MoonPdfPanel MoonPdfPanel { get { return this.moonPdfPanel; } }
        public MainWindow()
        {
            InitializeComponent();
            plombDataContext context = new plombDataContext(AtpEditorR.PlombEditorR);
            AtpEditorR.PlombEditorR.DataContext = context;

            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }


    }
}




