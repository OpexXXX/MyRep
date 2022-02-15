using System.Windows;
using System.Windows.Media;

namespace ATPWork.MyApp.View
{
    /// <summary>
    /// Логика взаимодействия для PhotoView.xaml
    /// </summary>
    public partial class PhotoView : Window
    {


        public PhotoView(string imagePath)
        {

            InitializeComponent();

            if (imagePath != null)
            {
                ImageSourceConverter converter = new ImageSourceConverter();
                ImageSource imageSource = (ImageSource)converter.ConvertFromString(imagePath);
                ImageW.Source = imageSource;
            }

        }
    }
}
