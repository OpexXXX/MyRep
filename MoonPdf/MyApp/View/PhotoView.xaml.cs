using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ATPWork.MyApp.View
{
    /// <summary>
    /// Логика взаимодействия для PhotoView.xaml
    /// </summary>
    public partial class PhotoView : Window
    {
       

        public PhotoView( string imagePath)
        {
           
            InitializeComponent();
            ImageSourceConverter converter = new ImageSourceConverter();
           
            ImageSource imageSource = (ImageSource)converter.ConvertFromString(imagePath);
            ImageW.Source = imageSource;

           
        }
    }
}
