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
    /// Lógica de interacción para CobroTarifaFijaBotones.xaml
    /// </summary>
    public partial class CobroTarifaFijaBotones : UserControl
    {
        public CobroTarifaFijaBotones()
        {
            InitializeComponent();
        }

        private void CobroTarifaFijaBotones_OnLoad(object sender, RoutedEventArgs e)
        {
            cargaBotonesSegunTarifasMontosFijos();
        }

        private void CobroTarifaFijaBotones_OnUnload(object sender, RoutedEventArgs e)
        {

        }


        private void cargaBotonesSegunTarifasMontosFijos()
        {
            Button bt = new Button();
            bt.Height = 20;
            bt.Tag = 1;
            bt.Click += new RoutedEventHandler(newBtn_Click);
            gridBotonesTarifasFijas.Children.Add(bt);
            
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int i = (int)btn.Tag;

            switch (i)
            {
                case 0:  /*do something*/ break;
                case 1:  /*do something else*/ break;
                default: /*do something by default*/ break;
            }
        }
    }
}
