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
using TestMdfEntityFramework.EntityServices;

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
            

            ServiceTarifasMontosFijos serv_tarifas_montos_fijos = new ServiceTarifasMontosFijos();
            List<ct_tarifas_montos_fijos> list_tarifas_montos_fijos = new List<ct_tarifas_montos_fijos>();
            list_tarifas_montos_fijos = serv_tarifas_montos_fijos.getEntities();


            ColumnDefinition colDef1 = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();
            ColumnDefinition colDef3 = new ColumnDefinition();
            RowDefinition rowDef1 = new RowDefinition();
            RowDefinition rowDef2 = new RowDefinition();


            #region CREACION GRID BOTONES

            //CREAR EL GRID PARA LOS BOTONES DE TARIFAS DE ACUERDO A LA CATIDAD DE TARIFAS QUE HAYA.
            Grid miGridBotones = new Grid();
            miGridBotones.Width = 900;
            miGridBotones.Height = 250;
            miGridBotones.HorizontalAlignment = HorizontalAlignment.Left;
            miGridBotones.VerticalAlignment = VerticalAlignment.Top;
            miGridBotones.ShowGridLines = true;

            #endregion

            #region definicion de columnas y filas del miGridBotones
            int font_size_txt = 0;
            int cant_tarifas_monto_fijo = list_tarifas_montos_fijos.Count;
            switch (cant_tarifas_monto_fijo)
            {
                case 1:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    font_size_txt = 150;
                    break;
                case 2:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    font_size_txt = 100;
                    break;
                case 3:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    miGridBotones.RowDefinitions.Add(rowDef2);
                    font_size_txt = 100;
                    break;
                case 4:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    miGridBotones.RowDefinitions.Add(rowDef2);
                    font_size_txt = 100;
                    break;
                case 5:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.ColumnDefinitions.Add(colDef3);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    miGridBotones.RowDefinitions.Add(rowDef2);
                    font_size_txt = 70;
                    break;
                case 6:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.ColumnDefinitions.Add(colDef3);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    miGridBotones.RowDefinitions.Add(rowDef2);
                    font_size_txt = 70;
                    break;
            }
            #endregion

            // AGREGAMOS EL GRID DE BOTONES AL GRID PRINCIPAL
            //this.Content = miGridBotones;
            this.grid1.Children.Add(miGridBotones);

            #region creacion de botones dinamicamente
            for (int i = 1; i <= list_tarifas_montos_fijos.Count; i++)
            {
                // BOTON DINAMICO
                Button bt = new Button();

                bt.Tag = list_tarifas_montos_fijos[i - 1].valor.ToString().Trim();
                bt.Background = Brushes.GreenYellow;
                bt.VerticalAlignment = VerticalAlignment.Center;
                bt.HorizontalAlignment = HorizontalAlignment.Center;
                bt.Margin = new Thickness(15, 15, 15, 15);
                bt.ClickMode = ClickMode.Press;
                bt.Click += new RoutedEventHandler(newBtn_Click);

                WrapPanel miWrap = new WrapPanel();

                TextBlock txt1 = new TextBlock();
                txt1.Text = list_tarifas_montos_fijos[i - 1].texto.ToString().Trim();
                txt1.FontSize = font_size_txt;
                txt1.FontWeight = FontWeights.Bold;
                txt1.HorizontalAlignment = HorizontalAlignment.Center;
                txt1.VerticalAlignment = VerticalAlignment.Center;
                txt1.TextAlignment = TextAlignment.Center;
                txt1.Foreground = Brushes.Black;

                miWrap.Children.Add(txt1);
                bt.Content = miWrap;

                switch (i)
                {
                    case 1:
                        Grid.SetRow(bt, 0);
                        Grid.SetColumn(bt, 0);
                        break;
                    case 2:
                        Grid.SetRow(bt, 0);
                        Grid.SetColumn(bt, 1);
                        break;
                    case 3:
                        Grid.SetRow(bt, 1);
                        Grid.SetColumn(bt, 0);
                        break;
                    case 4:
                        Grid.SetRow(bt, 1);
                        Grid.SetColumn(bt, 1);
                        break;
                    case 5:
                        Grid.SetRow(bt, 0);
                        Grid.SetColumn(bt, 2);
                        break;
                    case 6:
                        Grid.SetRow(bt, 1);
                        Grid.SetColumn(bt, 2);
                        break;
                }

                miGridBotones.Children.Add(bt);
            }
            #endregion



            #region GRID DE COBROS

            ColumnDefinition colDefCob1 = new ColumnDefinition();
            ColumnDefinition colDefCob2 = new ColumnDefinition();
            ColumnDefinition colDefCob3 = new ColumnDefinition();
            RowDefinition rowDefCob1 = new RowDefinition();
            RowDefinition rowDefCob2 = new RowDefinition();

            #region CREACION GRID COBROS

            //GRID PARA DEMAS CONTROLES
            Grid miGridCobro = new Grid();
            miGridCobro.Width = 900;
            miGridCobro.Height = 200;
            miGridCobro.HorizontalAlignment = HorizontalAlignment.Left;
            miGridCobro.VerticalAlignment = VerticalAlignment.Bottom;
            miGridCobro.ShowGridLines = true;

            #endregion

            #region definicion de columnas y filas del miGridCobro

            miGridCobro.ColumnDefinitions.Add(colDefCob1);
            miGridCobro.ColumnDefinitions.Add(colDefCob2);
            miGridCobro.ColumnDefinitions.Add(colDefCob3);
            miGridCobro.RowDefinitions.Add(rowDefCob1);
            miGridCobro.RowDefinitions.Add(rowDefCob2);
            font_size_txt = 50;

            #endregion

            // AGREGAMOS EL GRID DE BOTONES AL GRID PRINCIPAL
            //this.Content = miGridCobro;
            this.grid2.Children.Add(miGridCobro);

            #region creacion de controles de cobro y cancelar venta

            int font_size_lbl = 50;

            #region LABEL QUE MUESTRA LO COBRADO POR LA TARIFA
            TextBlock lbl_cobrado = new TextBlock();
            lbl_cobrado.Text = "$ 00.00";
            lbl_cobrado.FontSize = font_size_lbl;
            lbl_cobrado.FontWeight = FontWeights.Bold;
            lbl_cobrado.Foreground = Brushes.Blue;
            lbl_cobrado.HorizontalAlignment = HorizontalAlignment.Center;
            lbl_cobrado.VerticalAlignment = VerticalAlignment.Center;
            lbl_cobrado.TextAlignment = TextAlignment.Center;
            lbl_cobrado.Foreground = Brushes.Black;
            Grid.SetRow(lbl_cobrado, 0);
            Grid.SetColumn(lbl_cobrado, 0);
            miGridCobro.Children.Add(lbl_cobrado);
            #endregion

            #region LABEL QUE MUESTRA LO PAGADO ACTUALMENTE POR EL USUARIO EN LA VENTA ACTUAL
            TextBlock lbl_pagado = new TextBlock();
            lbl_pagado.Text = "$ 00.00";
            lbl_pagado.FontSize = font_size_lbl;
            lbl_pagado.FontWeight = FontWeights.Bold;
            lbl_cobrado.Foreground = Brushes.Green;
            lbl_pagado.HorizontalAlignment = HorizontalAlignment.Center;
            lbl_pagado.VerticalAlignment = VerticalAlignment.Center;
            lbl_pagado.TextAlignment = TextAlignment.Center;
            lbl_pagado.Foreground = Brushes.Black;
            Grid.SetRow(lbl_pagado, 1);
            Grid.SetColumn(lbl_pagado, 0);
            miGridCobro.Children.Add(lbl_pagado);
            #endregion

            #region BOTON DE COBRAR VENTA

            Button btnCobrarVenta = new Button();
            btnCobrarVenta.Width = 250;
            btnCobrarVenta.Height = 150;
            btnCobrarVenta.Tag = "COBRAR";
            btnCobrarVenta.Background = Brushes.Black;
            btnCobrarVenta.VerticalAlignment = VerticalAlignment.Center;
            btnCobrarVenta.HorizontalAlignment = HorizontalAlignment.Center;
            btnCobrarVenta.Margin = new Thickness(15, 15, 15, 15);
            btnCobrarVenta.ClickMode = ClickMode.Press;
            btnCobrarVenta.Click += new RoutedEventHandler(Btn_CobrarVenta_Click);

            WrapPanel miWrapCobrarVenta = new WrapPanel();

            TextBlock lbl_btn_cobrar_venta = new TextBlock();
            lbl_btn_cobrar_venta.Text = "COBRAR";
            lbl_btn_cobrar_venta.Foreground = Brushes.White;
            lbl_btn_cobrar_venta.FontSize = font_size_txt;
            lbl_btn_cobrar_venta.FontWeight = FontWeights.Bold;
            lbl_btn_cobrar_venta.HorizontalAlignment = HorizontalAlignment.Center;
            lbl_btn_cobrar_venta.VerticalAlignment = VerticalAlignment.Center;
            lbl_btn_cobrar_venta.TextAlignment = TextAlignment.Center;
            
            miWrapCobrarVenta.Children.Add(lbl_btn_cobrar_venta);
            btnCobrarVenta.Content = miWrapCobrarVenta;

            Grid.SetRow(btnCobrarVenta, 1);
            Grid.SetColumn(btnCobrarVenta, 1);
            //Grid.SetRowSpan(btnCobrarVenta, 2);

            miGridCobro.Children.Add(btnCobrarVenta);

            #endregion

            #region BOTON DE CANCELAR VENTA

            Button btnCancelarVenta = new Button();
            btnCobrarVenta.Width = 250;
            btnCobrarVenta.Height = 250;
            btnCancelarVenta.Tag = "CANCELAR";
            btnCancelarVenta.Background = Brushes.Black;
            btnCancelarVenta.VerticalAlignment = VerticalAlignment.Center;
            btnCancelarVenta.HorizontalAlignment = HorizontalAlignment.Center;
            btnCancelarVenta.Margin = new Thickness(15, 15, 15, 15);
            btnCancelarVenta.ClickMode = ClickMode.Press;
            btnCancelarVenta.Click += new RoutedEventHandler(Btn_CancelarVenta_Click);

            WrapPanel miWrapCancelarVenta = new WrapPanel();

            TextBlock lbl_btn_cancelar_venta = new TextBlock();
            lbl_btn_cancelar_venta.Text = "CANCELAR";
            lbl_btn_cancelar_venta.Foreground = Brushes.White;
            lbl_btn_cancelar_venta.FontSize = font_size_txt;
            lbl_btn_cancelar_venta.FontWeight = FontWeights.Bold;
            lbl_btn_cancelar_venta.HorizontalAlignment = HorizontalAlignment.Center;
            lbl_btn_cancelar_venta.VerticalAlignment = VerticalAlignment.Center;
            lbl_btn_cancelar_venta.TextAlignment = TextAlignment.Center;

            miWrapCancelarVenta.Children.Add(lbl_btn_cancelar_venta);
            btnCancelarVenta.Content = miWrapCancelarVenta;

            Grid.SetRow(btnCancelarVenta, 1);
            Grid.SetColumn(btnCancelarVenta, 2);
            //Grid.SetRowSpan(btnCancelarVenta, 2);

            miGridCobro.Children.Add(btnCancelarVenta);

            #endregion



            #endregion


            #endregion












            //// BOTON 1
            //Button bt = new Button();
            ////bt.Template = new ControlTemplate(typeof(Button))
            ////{
            ////    VisualTree = borderFactory
            ////};
            ////bt.Width = 200;
            ////bt.Height = 100;
            //bt.Tag = 1;
            //bt.Background = Brushes.GreenYellow;
            //bt.VerticalAlignment = VerticalAlignment.Center;
            //bt.HorizontalAlignment = HorizontalAlignment.Center;
            //bt.Margin = new Thickness(15, 15, 15, 15);
            //bt.ClickMode = ClickMode.Press;
            //bt.Click += new RoutedEventHandler(newBtn_Click);

            //WrapPanel miWrap = new WrapPanel();

            //TextBlock txt1 = new TextBlock();
            //txt1.Text = "$9.50";
            //txt1.FontSize = font_size_txt;
            //txt1.FontWeight = FontWeights.Bold;
            //txt1.HorizontalAlignment = HorizontalAlignment.Center;
            //txt1.VerticalAlignment = VerticalAlignment.Center;
            //txt1.TextAlignment = TextAlignment.Center;
            //txt1.Foreground = Brushes.Black;

            //miWrap.Children.Add(txt1);

            //bt.Content = miWrap;
            //Grid.SetRow(bt, 0);
            //Grid.SetColumn(bt, 0);


            //// BOTON 2
            //Button bt2 = new Button();
            ////bt2.Template = new ControlTemplate(typeof(Button))
            ////{
            ////    VisualTree = borderFactory
            ////};
            ////bt2.Width = 200;
            ////bt2.Height = 100;
            //bt2.Tag = 2;
            //bt2.Background = Brushes.GreenYellow;
            //bt2.VerticalAlignment = VerticalAlignment.Center;
            //bt2.HorizontalAlignment = HorizontalAlignment.Center;
            //bt2.Margin = new Thickness(15, 15, 15, 15);
            //bt2.ClickMode = ClickMode.Press;
            //bt2.Click += new RoutedEventHandler(newBtn_Click);


            //WrapPanel miWrap2 = new WrapPanel();

            //TextBlock txt2 = new TextBlock();
            //txt2.Text = "$4.75";
            //txt2.FontSize = font_size_txt;
            //txt2.FontWeight = FontWeights.Bold;
            //txt2.HorizontalAlignment = HorizontalAlignment.Center;
            //txt2.VerticalAlignment = VerticalAlignment.Center;
            //txt2.TextAlignment = TextAlignment.Center;
            //txt2.Foreground = Brushes.Black;

            //miWrap2.Children.Add(txt2);

            //bt2.Content = miWrap2;

            //Grid.SetRow(bt2, 0);
            //Grid.SetColumn(bt2, 1);



            //miGridBotones.Children.Add(bt);
            //miGridBotones.Children.Add(bt2);



            //var borderFactory = new FrameworkElementFactory(typeof(Border));
            //borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(5));
            //borderFactory.SetValue(Border.BackgroundProperty, Brushes.GreenYellow);

            //bt.Template = new ControlTemplate(typeof(Button))
            //{
            //    VisualTree = borderFactory
            //};
            //bt.Width = 200;
            //bt.Height = 100;

        }

        private void Btn_CancelarVenta_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((Button)sender).Tag.ToString());
        }

        private void Btn_CobrarVenta_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((Button)sender).Tag.ToString());
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((Button)sender).Tag.ToString());
        }
    }
}
