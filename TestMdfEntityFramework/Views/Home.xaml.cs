using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestMdfEntityFramework.EntityServices;

namespace TestMdfEntityFramework.Views
{
    /// <summary>
    /// Lógica de interacción para Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            cargar_logo_home();


        }

        private void cargar_logo_home()
        {
            //Cargar el logo home que se configuro
            ServiceConfigVarios serv_config_varios_apariencia = new ServiceConfigVarios();
            config_varios cv_apariencia_logo_home = serv_config_varios_apariencia.getEntityByClave("LOGO_HOME");

            ServiceImagenesSubidas serv_img_subidas = new ServiceImagenesSubidas();
            ct_imagenes_subidas obj_img_sub = serv_img_subidas.getEntityByName(cv_apariencia_logo_home.valor);

            ImageSource img_src = ByteToImage(obj_img_sub.imagen);

            imgLogoHome.Source = img_src;
        }
        public static ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }
    }
}
