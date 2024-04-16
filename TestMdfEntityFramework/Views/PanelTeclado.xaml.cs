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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestMdfEntityFramework.Views
{
    /// <summary>
    /// Lógica de interacción para PanelTeclado.xaml
    /// </summary>
    public partial class PanelTeclado : UserControl
    {
        public PanelTeclado()
        {
            InitializeComponent();
            PanelTeclado_Load(null,null);
        }

        private void PanelTeclado_Load(object sender, RoutedEventArgs e)
        {
            List<string> abecedario = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "Ñ", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "ESPACIO" };
            foreach (var item in abecedario)
            {
                Button btn = new Button();
                btn.Content = item;
                btn.Background = Brushes.White;
                //btn.Click += Btn_Click;
                btn.Width = 20;
                btn.Height = 20;
                panel_teclado.Children.Add(btn);
                panel_teclado.Visibility = Visibility.Visible;
                
            }


        }
    }
}
