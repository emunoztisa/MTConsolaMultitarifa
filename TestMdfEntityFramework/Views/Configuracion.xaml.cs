using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TestMdfEntityFramework.Views
{
    /// <summary>
    /// Lógica de interacción para Configuracion.xaml
    /// </summary>
    public partial class Configuracion : UserControl
    {
        public Configuracion()
        {
            InitializeComponent();
        }
        private void TBShow(object sender, RoutedEventArgs e)
        {
            GridContentConfiguraciones.Opacity = 0.5;
        }

        private void TBHide(object sender, RoutedEventArgs e)
        {
            GridContentConfiguraciones.Opacity = 1;
        }
        private void PreviewMouseLeftBottonDownBG_conf(object sender, MouseButtonEventArgs e)
        {
            btnShowHide.IsChecked = false;
        }

        //private void btnConfPuertoSerie_Click(object sender, RoutedEventArgs e)
        //{
        //    DataContext = new ConfPuertoSerie();
        //}

        //private void btnConfImpresora_Click(object sender, RoutedEventArgs e)
        //{
        //    DataContext = new ConfImpresora();
        //}
    }
}
