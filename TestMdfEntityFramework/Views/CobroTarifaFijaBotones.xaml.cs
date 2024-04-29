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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TestMdfEntityFramework.EntityServices;

namespace TestMdfEntityFramework.Views
{
    /// <summary>
    /// Lógica de interacción para CobroTarifaFijaBotones.xaml
    /// </summary>
    public partial class CobroTarifaFijaBotones : UserControl
    {
        sy_asignaciones asign_activa = null;

        const decimal ByteInicio = 1;
        const decimal AddressConsola = 1;
        const decimal AddressAlcancia = 2;

        const Int32 K_offsetDatos = 4;
        const Int32 K_posicionCantidadDatos = 3;


        //BUFFERS SEND AND RECIEVED
        byte[] BufferSendData = new byte[80];
        byte[] RecievedDataGlobal = new byte[80];

        //POPUP OK
        private double left, top, right, bottom, centerX, centerY;
        private DoubleAnimation bottomToCenterAnimiation, topToCenterAnimation,
            leftToCenterAnimation, rightToCenterAnimation;
        private Storyboard bottomToCenterStoryboard, topToCenterStoryboard,
            leftToCenterStoryboard, rightToCenterStoryboard;


        //SERIAL PORT
        System.IO.Ports.SerialPort puertoSerie1 = new System.IO.Ports.SerialPort();
        String[] listado_puerto = System.IO.Ports.SerialPort.GetPortNames();


        //TIMERS
        DispatcherTimer timerWait = new DispatcherTimer();
        DispatcherTimer timerEvalua = new DispatcherTimer();


        #region DEFINICION DE DELEGADOS

        private string ESTATUS = "0";
        delegate void delegate_actualizaTxtStatus();
        delegate_actualizaTxtStatus delegado_actualiza_txt_status = null;

        private string MONTO_INGRESADO = "0";
        delegate void delegate_actualizalblMontoIngresado();
        delegate_actualizalblMontoIngresado delegado_actualiza_lbl_monto_ingresado = null;

        delegate void delegate_limpiarCamposDespuesDeVentaBoleto();
        delegate_limpiarCamposDespuesDeVentaBoleto delegado_limpiar_campos_despues_de_venta_boleto = null;


        #endregion

        TextBlock txtStatus = new TextBlock();
        TextBlock lblMontoCalculado = new TextBlock();
        TextBlock lblMontoIngresado = new TextBlock();
        Button btnCancelarVenta = new Button();


        int CANTIDAD_VECES_INSERTA_BOLETO = 0;

        public CobroTarifaFijaBotones()
        {
            InitializeComponent();

            // Para actualizar el campo de txtStatus en la interfas grafica.
            //delegate_actualizaTxtStatus delegado_actualiza_txt_status = new delegate_actualizaTxtStatus(actualizaTxtStatus);
            delegado_actualiza_txt_status = new delegate_actualizaTxtStatus(actualizaTxtStatus);
            delegado_actualiza_txt_status();

            delegado_actualiza_lbl_monto_ingresado = new delegate_actualizalblMontoIngresado(actualizalblMontoIngresado);
            delegado_actualiza_lbl_monto_ingresado();

            //delegado_limpiar_campos_despues_de_venta_boleto = new delegate_limpiarCamposDespuesDeVentaBoleto(LimpiarCamposDespuesDeVentaBoleto);
            //delegado_limpiar_campos_despues_de_venta_boleto();

            configura_puerto_serial(); // UTILIZA ENTITY FRAMEWORK CON CONEXION A database.mdf
        }

        private void cargaBotonesSegunTarifasMontosFijos()
        {
            //TextBlock txtStatus = new TextBlock();
            //TextBlock lbl_cobrado = new TextBlock();
            //TextBlock lbl_pagado = new TextBlock();

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
            int font_size_txt_desc = 0;
            int cant_tarifas_monto_fijo = list_tarifas_montos_fijos.Count;
            switch (cant_tarifas_monto_fijo)
            {
                case 1:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    font_size_txt = 120;
                    font_size_txt_desc = 20;
                    break;
                case 2:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    font_size_txt = 100;
                    font_size_txt_desc = 20;
                    break;
                case 3:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    miGridBotones.RowDefinitions.Add(rowDef2);
                    font_size_txt = 60;
                    font_size_txt_desc = 20;
                    break;
                case 4:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    miGridBotones.RowDefinitions.Add(rowDef2);
                    font_size_txt = 60;
                    font_size_txt_desc = 20;
                    break;
                case 5:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.ColumnDefinitions.Add(colDef3);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    miGridBotones.RowDefinitions.Add(rowDef2);
                    font_size_txt = 50;
                    font_size_txt_desc = 20;
                    break;
                case 6:
                    miGridBotones.ColumnDefinitions.Add(colDef1);
                    miGridBotones.ColumnDefinitions.Add(colDef2);
                    miGridBotones.ColumnDefinitions.Add(colDef3);
                    miGridBotones.RowDefinitions.Add(rowDef1);
                    miGridBotones.RowDefinitions.Add(rowDef2);
                    font_size_txt = 50;
                    font_size_txt_desc = 20;
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
                StackPanel miStack = new StackPanel();

                TextBlock txtDesc = new TextBlock();
                txtDesc.Text = list_tarifas_montos_fijos[i - 1].descripcion.ToString().Trim();
                txtDesc.FontSize = font_size_txt_desc;
                txtDesc.FontWeight = FontWeights.Bold;
                txtDesc.HorizontalAlignment = HorizontalAlignment.Center;
                txtDesc.VerticalAlignment = VerticalAlignment.Top;
                txtDesc.TextAlignment = TextAlignment.Center;
                txtDesc.Foreground = Brushes.Black;
                

                TextBlock txt1 = new TextBlock();
                txt1.Text = list_tarifas_montos_fijos[i - 1].texto.ToString().Trim();
                txt1.FontSize = font_size_txt;
                txt1.FontWeight = FontWeights.Bold;
                txt1.HorizontalAlignment = HorizontalAlignment.Center;
                txt1.VerticalAlignment = VerticalAlignment.Bottom;
                txt1.TextAlignment = TextAlignment.Center;
                txt1.Foreground = Brushes.Black;

                miStack.Children.Add(txtDesc);
                miStack.Children.Add(txt1);
                bt.Content = miStack;

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
            //TextBlock lbl_cobrado = new TextBlock();
            //lblMontoCalculado.Text = "$ 00.00";
            lblMontoCalculado.FontSize = font_size_lbl;
            lblMontoCalculado.FontWeight = FontWeights.Bold;
            lblMontoCalculado.Foreground = Brushes.Blue;
            lblMontoCalculado.HorizontalAlignment = HorizontalAlignment.Center;
            lblMontoCalculado.VerticalAlignment = VerticalAlignment.Center;
            lblMontoCalculado.TextAlignment = TextAlignment.Center;
            lblMontoCalculado.Foreground = Brushes.Black;
            Grid.SetRow(lblMontoCalculado, 0);
            Grid.SetColumn(lblMontoCalculado, 0);
            miGridCobro.Children.Add(lblMontoCalculado);
            #endregion

            #region LABEL QUE MUESTRA LO PAGADO ACTUALMENTE POR EL USUARIO EN LA VENTA ACTUAL
            //TextBlock lbl_pagado = new TextBlock();
            lblMontoIngresado.Text = "$ 00.00";
            lblMontoIngresado.FontSize = font_size_lbl;
            lblMontoIngresado.FontWeight = FontWeights.Bold;
            lblMontoIngresado.Foreground = Brushes.Green;
            lblMontoIngresado.HorizontalAlignment = HorizontalAlignment.Center;
            lblMontoIngresado.VerticalAlignment = VerticalAlignment.Center;
            lblMontoIngresado.TextAlignment = TextAlignment.Center;
            lblMontoIngresado.Foreground = Brushes.Black;
            Grid.SetRow(lblMontoIngresado, 1);
            Grid.SetColumn(lblMontoIngresado, 0);
            miGridCobro.Children.Add(lblMontoIngresado);
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
            btnCobrarVenta.Visibility = Visibility.Hidden;

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

            //Button btnCancelarVenta = new Button();
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

        private void CobroTarifaFijaBotones_OnLoad(object sender, RoutedEventArgs e)
        {
            asign_activa = ObtenerAsignacionActiva();

            cargaBotonesSegunTarifasMontosFijos();

            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();

            //btnCancelarVenta.IsEnabled = false;


        }

        private void CobroTarifaFijaBotones_OnUnload(object sender, RoutedEventArgs e)
        {
            close_serial_port();
        }

        private void Btn_CancelarVenta_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(((Button)sender).Tag.ToString());

            ejecutar_commando_06_cancelar_venta();

            Task.WaitAll(new Task[] { Task.Delay(200) });
            puertoSerie1.Read(RecievedDataGlobal, 0, 50);

            if (RecievedDataGlobal != null)
            {
                insert_boleto_cancelado_en_db_local(RecievedDataGlobal);
                Task.WaitAll(new Task[] { Task.Delay(100) });

                desbloqueaCampos();

                txtMensajePopup.Text = "VENTA CANCELADA";
                mostrarPopupOk();

                actualizaTxtStatus();
                delegado_actualiza_txt_status();

                //Actualiza Monto Ingresado
                delegado_actualiza_lbl_monto_ingresado();

                detiene_timers();
            }
            else
            {
                MessageBox.Show("Algo salio mal", "ERROR");
            }
        }


        private void Btn_CobrarVenta_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((Button)sender).Tag.ToString());
        }
        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((Button)sender).Tag.ToString().Trim();

            lblMontoCalculado.Text = tag.ToString().Trim();

            cobrar_tarifa_monto_fijo(Convert.ToDecimal(tag));

            

            //MessageBox.Show(((Button)sender).Tag.ToString());
        }
        private void cobrar_tarifa_monto_fijo(decimal tarifa_monto_fijo)
        {
            try
            {
                bloqueaCamposMientrasIngresaMonedas();

                //inicializa_timer_wait();
                //inicializa_timer_evalua();

                const Int32 K_posicionCantidadTarifas = 24;
                //const decimal CantidadDatos = 17;
                const decimal Comando = 1;
                //const byte CantidadTarifas = 1;
                const decimal CRC1 = 193;
                const decimal CRC2 = 194;

                Int32 CantidadDatos = 0;
                Decimal CantidadTarifas = 0;

                //FOLIO BOLETO
                string folioVenta = ObtenerSiguienteFolioAInsertarEnDbLocal();

                DateTime varFechaHora = DateTime.Now;

                byte[] BCDDateTime = ToBCD_DT(varFechaHora);

                ClearBufferSendData();

                BufferSendData[0] = decimal.ToByte(ByteInicio);
                BufferSendData[1] = decimal.ToByte(AddressAlcancia);
                BufferSendData[2] = decimal.ToByte(AddressConsola);

                BufferSendData[4] = decimal.ToByte(Comando);
                CantidadDatos += 1;

                // PREGUNTAR A GILBERTO
                //varStringToByte(K_offsetDatos + CantidadDatos, folioVenta);
                agregar_folio_to_buffer_send_data(folioVenta);
                CantidadDatos += 17;


                //COLOCA FECHA EN EL BUFFER
                BufferSendData[22] = BCDDateTime[0];
                BufferSendData[23] = BCDDateTime[1];
                BufferSendData[24] = BCDDateTime[2];
                BufferSendData[25] = BCDDateTime[3];
                BufferSendData[26] = BCDDateTime[4];
                BufferSendData[27] = BCDDateTime[5];
                CantidadDatos += 6;

                // Se incrementa la variable de "CantidadDatos" para compensar la posición de las tarifas
                //BufferSendData[K_offsetDatos + K_posicionCantidadTarifas] = decimal.ToByte(CantidadTarifas);
                CantidadDatos += 1;

                int CantUsuarios = 1;
                UInt32 PrecioTarifa1 = (UInt32)(tarifa_monto_fijo * 100);
                if (!(CantUsuarios == 0) && !(PrecioTarifa1 == 0))
                {
                    BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CantUsuarios);
                    CantidadDatos += 1;

                    varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa1);
                    CantidadDatos += 4;
                    CantidadTarifas += 1;
                }

                //La cantidad de datos se incremento previo a copiar las tarifas y cantidad de usuarios.
                BufferSendData[K_offsetDatos + K_posicionCantidadTarifas] = decimal.ToByte(CantidadTarifas);

                BufferSendData[K_posicionCantidadDatos] = (byte)(CantidadDatos);

                BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
                BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

                open_serial_port();
                puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);
                //close_serial_port();

                Task.WaitAll(new Task[] { Task.Delay(200) });
                puertoSerie1.Read(RecievedDataGlobal, 0, 32);

                if (RecievedDataGlobal[5] == 0)
                {
                    inicializa_timer_wait();
                    inicializa_timer_evalua();
                }
                else
                {
                    MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando enviado");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region SERIAL PORT
        public void configura_puerto_serial()
        {
            try
            {
                ServiceConfigVarios scv = new ServiceConfigVarios();
                config_varios cv_port_name = scv.getEntityByClave("PORT_NAME");
                config_varios cv_baud_rate = scv.getEntityByClave("BAUD_RATE");
                config_varios cv_paridad = scv.getEntityByClave("PARIDAD");
                config_varios cv_data_bits = scv.getEntityByClave("DATA_BITS");
                config_varios cv_stop_bits = scv.getEntityByClave("STOP_BITS");
                config_varios cv_handshake = scv.getEntityByClave("HANDSHAKE");

                this.puertoSerie1 = new System.IO.Ports.SerialPort
                    ("" + cv_port_name.valor
                    , Convert.ToInt32(cv_baud_rate.valor)
                    , cv_paridad.valor == "NONE" ? System.IO.Ports.Parity.None : System.IO.Ports.Parity.Mark
                    , Convert.ToInt32(cv_data_bits.valor)
                    , Convert.ToInt32(cv_stop_bits.valor) == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.None
                    );
                puertoSerie1.Handshake = cv_handshake.valor == "NONE" ? System.IO.Ports.Handshake.None : System.IO.Ports.Handshake.XOnXOff;



                //ServiceConfigPort scp = new ServiceConfigPort();
                //List<config_port> list = scp.getEntities();

                //this.puertoSerie1 = new System.IO.Ports.SerialPort
                //    ("" + list[0].port_name
                //    , Convert.ToInt32(list[0].baud_rate)
                //    , list[0].parity == "NONE" ? System.IO.Ports.Parity.None : System.IO.Ports.Parity.Mark
                //    , Convert.ToInt32(list[0].data_bits)
                //    , Convert.ToInt32(list[0].stop_bits) == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.None
                //    );
                //puertoSerie1.Handshake = Handshake.None;
            }
            catch
            {
                MessageBox.Show("Verifique" + System.Environment.NewLine + "- Alimentación" + System.Environment.NewLine + "- Conexión del puerto", "Error de puerto COMM");
                //btnCobrar.IsEnabled = false;
            }

        }
        private void open_serial_port()
        {
            if (puertoSerie1.IsOpen == false)
            {
                puertoSerie1.Open();
            }
        }
        private void close_serial_port()
        {
            if (puertoSerie1.IsOpen == true)
            {
                puertoSerie1.Close();
            }
        }

        #endregion

        #region CONVERSIONES

        void varInteger32ToByte(Int32 IndiceBuffer, UInt32 Var32)
        {
            BufferSendData[IndiceBuffer] = (byte)Var32;
            BufferSendData[IndiceBuffer + 1] = (byte)(Var32 >> 8);
            BufferSendData[IndiceBuffer + 2] = (byte)(Var32 >> 16);
            BufferSendData[IndiceBuffer + 3] = (byte)(Var32 >> 24);
        }

        //void varStringToByte(Int32 IndiceBuffer, string Var32)
        //{
        //    BufferSendData[IndiceBuffer] = (byte)Var32;
        //    BufferSendData[IndiceBuffer + 1] = (byte)(Var32 >> 8);
        //    BufferSendData[IndiceBuffer + 2] = (byte)(Var32 >> 16);
        //    BufferSendData[IndiceBuffer + 3] = (byte)(Var32 >> 24);
        //}
        private uint VarByteToUInteger32(Int32 IndiceBuffer)
        {
            //uint varFolioTemporal = 0;
            uint varFolioTemporal = 0;

            for (int i = 0; i < 4; i++)
            {
                varFolioTemporal <<= 8;
                varFolioTemporal |= RecievedDataGlobal[IndiceBuffer--];
            }

            return varFolioTemporal;
        }
        private uint VarByteToUInteger32_comand_05(byte[] RecievedDataGlobal)
        {
            uint varCantTemp = 0;
            for (int i = 9; i >= 6; i--) // para iterar por las posiciones de ReceivedData, donde sabemos viene el monto actual ingresado.
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            return varCantTemp;
        }
        static byte[] ToBCD_DT(DateTime d)
        {
            List<byte> bytes = new List<byte>();
            string s = d.ToString("yyMMddHHmmss");
            for (int i = 0; i < s.Length; i += 2)
            {
                bytes.Add((byte)((s[i] - '0') << 4 | (s[i + 1] - '0')));
            }
            return bytes.ToArray();
        }
        private string StringToBCD(int indexBuffer)
        {
            string TextoFechaHora = "";

            for (int i = 0; i < 6; i++)
            {
                TextoFechaHora += ((RecievedDataGlobal[indexBuffer] >> 4));
                TextoFechaHora += ((RecievedDataGlobal[indexBuffer] & 15));
                indexBuffer++;
            }


            return TextoFechaHora;
        }


        #endregion

        #region METODOS - TIMERS
        private void inicializa_timer_wait()
        {
            // INICIA TIMER QUE ESTARA ACTUALIZANDO EL VALOR DE ESTATUS
            timerWait.Tick += new EventHandler(dispatcherTimerWait_Tick);
            timerWait.Interval = new TimeSpan(0, 0, 1);
            timerWait.Start();
        }
        private void dispatcherTimerWait_Tick(object sender, EventArgs e)
        {
            try
            {
                //open_serial_port();
                if (puertoSerie1.IsOpen == true)
                {

                    //puertoSerie1.Dispose();
                    //open_serial_port();

                    //solicitar_status_alcancia();
                    solicitar_status_alcancia_commando_05();

                    //string RecievedData;
                    timerWait.IsEnabled = true;

                    // delay para esperar 8 de respuesta
                    // 30 funciona en debug de visual studio
                    // 100 para app instalada en raspberry
                    Task.WaitAll(new Task[] { Task.Delay(30) });

                    //RecievedData = puertoSerie1.ReadExisting();

                    puertoSerie1.Read(RecievedDataGlobal, 0, 32);

                    int status = Convert.ToInt32(RecievedDataGlobal[5]);
                    ESTATUS = status.ToString();

                    uint monto_ingresado = obtenerMontoIngresadoActual(RecievedDataGlobal); //PENDIENTE
                    MONTO_INGRESADO = (monto_ingresado / 100).ToString();

                }
                //open_serial_port();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EXCEPTION !!!");
            }

        }
        private void inicializa_timer_evalua()
        {
            CANTIDAD_VECES_INSERTA_BOLETO = 0;

            // INICIA TIMER QUE ESTARA EVALUANDO EL VALOR DE ESTATUS, PARA REALIZAR O NO CIERTAS ACCIONES SOBRE LA INTERFAS GRAFICA
            timerEvalua.Tick += new EventHandler(dispatcherTimerEvalua_Tick);
            timerEvalua.Interval = new TimeSpan(0, 0, 1);
            timerEvalua.Start();
        }
        private void dispatcherTimerEvalua_Tick(object sender, EventArgs e)
        {
            timerEvalua.IsEnabled = true;
            //CASO EN EL QUE DEBEN ESTAR HABILITADOS LOS CAMPOS PARA INGRESAR DE NUEVO LA TARIFA DESDE LA CONSOLA
            if (ESTATUS == "4" || ESTATUS == "")
            {

                desbloqueaCampos();

                txtMensajePopup.Text = "TARIFA COMPLETADA";
                mostrarPopupOk();

                //actualizaTxtStatus();
                delegado_actualiza_txt_status();

                //Actualiza Monto Ingresado
                delegado_actualiza_lbl_monto_ingresado();

                detiene_timers();

                //INSERTAR BOLETO EN LA BASE DE DATOS LOCAL
                Task.WaitAll(new Task[] { Task.Delay(500) });
                if (CANTIDAD_VECES_INSERTA_BOLETO == 0)
                {
                    decimal total_cobrado = 0;
                    decimal total_pagado = 0;

                    //Task.WaitAll(new Task[] { Task.Delay(100) });
                    //puertoSerie1.Read(RecievedDataGlobal, 0, 50);

                    //if (RecievedDataGlobal[5] == 0) // si el comando de tarifa se completo correctamente.
                    //{

                    // TODO: ejecutar_commando_02_ultima_venta para obtener los totales cobrado y pagado
                    ejecutar_commando_02_ultima_venta();

                    Task.WaitAll(new Task[] { Task.Delay(100) });
                    puertoSerie1.Read(RecievedDataGlobal, 0, 50);

                    total_cobrado = getTotalCobradoBoleto();
                    total_pagado = getTotalPagadoBoleto();

                    InsertarBoleto(total_cobrado, total_pagado);
                    CANTIDAD_VECES_INSERTA_BOLETO++;
                    //}

                }


                //Actualiza los campos limpiandolos despues de realizar la venta del boleto
                //delegado_limpiar_campos_despues_de_venta_boleto();
            }
            else if (ESTATUS == "0" || ESTATUS == "1" || ESTATUS == "2" || ESTATUS == "3")
            {
                //actualizaTxtStatus();
                delegado_actualiza_txt_status();

                //Actualiza Monto Ingresado
                delegado_actualiza_lbl_monto_ingresado();

                bloqueaCamposMientrasIngresaMonedas();
            }
        }
        private void detiene_timers()
        {
            //timerWait.Stop();
            //timerEvalua.Stop();

            timerWait.IsEnabled = false;
            timerEvalua.IsEnabled = false;
        }
        #endregion

        #region COMANDOS
        private void solicitar_status_alcancia_commando_05()
        {
            const byte ByteInicio = 1;
            const byte AddressConsola = 1;
            const byte AddressAlcancia = 2;
            const byte cantidadDatos = 1;
            const byte numeroComando = 5;
            const byte crc1 = 41;
            const byte crc2 = 42;

            //timerPoll.Enabled = false;
            byte[] BufferSendData = new byte[7];
            BufferSendData[0] = ByteInicio;
            BufferSendData[1] = AddressAlcancia;
            BufferSendData[2] = AddressConsola;
            BufferSendData[3] = cantidadDatos;
            BufferSendData[4] = numeroComando;
            BufferSendData[5] = crc1;
            BufferSendData[6] = crc2;

            puertoSerie1.Write(BufferSendData, 0, 7);
            //timerWait.Enabled = true;
        }
        private void solicitar_total_cobrado_ultima_venta_commando_02()
        {
            const byte ByteInicio = 1;
            const byte AddressConsola = 1;
            const byte AddressAlcancia = 2;
            const byte cantidadDatos = 1;
            const byte numeroComando = 2;
            const byte crc1 = 41;
            const byte crc2 = 42;

            //timerPoll.Enabled = false;
            byte[] BufferSendData = new byte[7];
            BufferSendData[0] = ByteInicio;
            BufferSendData[1] = AddressAlcancia;
            BufferSendData[2] = AddressConsola;
            BufferSendData[3] = cantidadDatos;
            BufferSendData[4] = numeroComando;
            BufferSendData[5] = crc1;
            BufferSendData[6] = crc2;

            puertoSerie1.Write(BufferSendData, 0, 7);
        }
        private void agregar_folio_to_buffer_send_data_v2(string folio)
        {

            byte prefijo1 = (byte)folio[0];
            byte prefijo2 = (byte)folio[1];
            byte prefijo3 = (byte)folio[2];

            byte guion1 = (byte)folio[3];

            byte uni1 = (byte)folio[4];
            byte uni2 = (byte)folio[5];
            byte uni3 = (byte)folio[6];
            byte uni4 = (byte)folio[7];
            byte uni5 = (byte)folio[8];
            byte uni6 = (byte)folio[9];

            byte guion2 = (byte)folio[10];

            byte cons1 = (byte)folio[11];
            byte cons2 = (byte)folio[12];
            byte cons3 = (byte)folio[13];
            byte cons4 = (byte)folio[14];
            byte cons5 = (byte)folio[15];
            byte cons6 = (byte)folio[16];

            BufferSendData[5] = (byte)folio[0];
            BufferSendData[6] = prefijo2;
            BufferSendData[7] = prefijo3;

            BufferSendData[8] = guion1;

            BufferSendData[9] = uni1;
            BufferSendData[10] = uni2;
            BufferSendData[11] = uni3;
            BufferSendData[12] = uni4;
            BufferSendData[13] = uni5;
            BufferSendData[14] = uni6;

            BufferSendData[15] = guion2;

            BufferSendData[16] = cons1;
            BufferSendData[17] = cons2;
            BufferSendData[18] = cons3;
            BufferSendData[19] = cons4;
            BufferSendData[19] = cons5;
            BufferSendData[20] = cons6;
        }
        private void ejecutar_commando_06_cancelar_venta()
        {
            DateTime varFechaHora = DateTime.Now;
            byte[] BCDDateTime = ToBCD_DT(varFechaHora);

            const decimal Comando = 6;
            const decimal CRC1 = 193;
            const decimal CRC2 = 194;

            int CantidadDatos = 0;

            ClearBufferSendData();

            BufferSendData[0] = decimal.ToByte(ByteInicio);
            BufferSendData[1] = decimal.ToByte(AddressAlcancia);
            BufferSendData[2] = decimal.ToByte(AddressConsola);

            BufferSendData[4] = decimal.ToByte(Comando);
            CantidadDatos += 1;

            // Metodo para consultar el siguiente corte de alcancia a asignar
            string folio_boleto_actual = obtener_folio_boleto_actual();

            ingresa_folio_boleto_actual_en_buffer_send_data(folio_boleto_actual);
            CantidadDatos += 17;

            BufferSendData[22] = BCDDateTime[0];
            BufferSendData[23] = BCDDateTime[1];
            BufferSendData[24] = BCDDateTime[2];
            BufferSendData[25] = BCDDateTime[3];
            BufferSendData[26] = BCDDateTime[4];
            BufferSendData[27] = BCDDateTime[5];
            CantidadDatos += 6;

            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
            BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

            BufferSendData[K_posicionCantidadDatos] = (byte)(CantidadDatos);

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);
        }
        private void ejecutar_commando_02_ultima_venta()
        {
            const decimal Comando = 2;
            //const byte CantidadTarifas = 1;
            const decimal CRC1 = 193;
            const decimal CRC2 = 194;

            int CantidadDatos = 0;

            ClearBufferSendData();

            BufferSendData[0] = decimal.ToByte(ByteInicio);
            BufferSendData[1] = decimal.ToByte(AddressAlcancia);
            BufferSendData[2] = decimal.ToByte(AddressConsola);

            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(Comando);
            CantidadDatos += 1;
            BufferSendData[K_posicionCantidadDatos] = (byte)(CantidadDatos);

            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
            BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);
        }
        #endregion

        #region METODOS GRID POPUP
        private void ocultarPopupOk()
        {
            try
            {
                SetPopupDlgCenter();
                bottomToCenterAnimiation.From = bottom;
                bottomToCenterAnimiation.To = centerY;

                Canvas.SetTop(popupBd, bottom);

                popupGrid.Visibility = Visibility.Hidden;

                bottomToCenterStoryboard.Begin();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void mostrarPopupOk()
        {
            try
            {
                SetPopupDlgCenter();
                bottomToCenterAnimiation.From = bottom;
                bottomToCenterAnimiation.To = centerY;

                Canvas.SetTop(popupBd, bottom);

                popupGrid.Visibility = Visibility.Visible;

                bottomToCenterStoryboard.Begin();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void SetPopupDlgCenter()
        {
            try
            {
                left = -(popupBd.ActualWidth);
                top = -(popupBd.ActualHeight);
                right = (popupGrid.ActualWidth);
                bottom = (popupGrid.ActualHeight);

                centerX = (popupGrid.ActualWidth / 2) - popupBd.ActualWidth / 2;
                centerY = (popupGrid.ActualHeight / 2) - popupBd.ActualHeight / 2;

                Canvas.SetLeft(popupBd, centerX);
                Canvas.SetTop(popupBd, centerY);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void InitializeAnimations()
        {
            try
            {
                #region bottom to center animation
                bottomToCenterAnimiation = new DoubleAnimation()
                {
                    From = bottom,
                    To = centerY,
                    Duration = TimeSpan.FromMilliseconds(250),
                    FillBehavior = FillBehavior.Stop,
                };

                Storyboard.SetTarget(bottomToCenterAnimiation, popupBd);
                Storyboard.SetTargetProperty(bottomToCenterAnimiation, new PropertyPath(Canvas.TopProperty));

                bottomToCenterStoryboard = new Storyboard();
                bottomToCenterStoryboard.Children.Add(bottomToCenterAnimiation);

                bottomToCenterStoryboard.Completed += OnStoryboardCompleted;
                #endregion

                #region top to center animation 
                topToCenterAnimation = new DoubleAnimation()
                {
                    From = top,
                    To = centerY,
                    Duration = TimeSpan.FromMilliseconds(250),
                    FillBehavior = FillBehavior.Stop,
                };

                Storyboard.SetTarget(topToCenterAnimation, popupBd);
                Storyboard.SetTargetProperty(topToCenterAnimation, new PropertyPath(Canvas.TopProperty));

                topToCenterStoryboard = new Storyboard();
                topToCenterStoryboard.Children.Add(topToCenterAnimation);

                topToCenterStoryboard.Completed += OnStoryboardCompleted;
                #endregion

                #region left to center animation
                leftToCenterAnimation = new DoubleAnimation()
                {
                    From = left,
                    To = centerX,
                    Duration = TimeSpan.FromMilliseconds(250),
                    FillBehavior = FillBehavior.Stop,
                };

                Storyboard.SetTarget(leftToCenterAnimation, popupBd);
                Storyboard.SetTargetProperty(leftToCenterAnimation, new PropertyPath(Canvas.LeftProperty));

                leftToCenterStoryboard = new Storyboard();
                leftToCenterStoryboard.Children.Add(leftToCenterAnimation);

                leftToCenterStoryboard.Completed += OnStoryboardCompleted;
                #endregion

                #region right to center animation
                rightToCenterAnimation = new DoubleAnimation()
                {
                    From = right,
                    To = centerX,
                    Duration = TimeSpan.FromMilliseconds(250),
                    FillBehavior = FillBehavior.Stop,
                };

                Storyboard.SetTarget(rightToCenterAnimation, popupBd);
                Storyboard.SetTargetProperty(rightToCenterAnimation, new PropertyPath(Canvas.LeftProperty));

                rightToCenterStoryboard = new Storyboard();
                rightToCenterStoryboard.Children.Add(rightToCenterAnimation);

                rightToCenterStoryboard.Completed += OnStoryboardCompleted;

                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void OnStoryboardCompleted(object sender, EventArgs e)
        {
            Canvas.SetLeft(popupBd, centerX);
            Canvas.SetTop(popupBd, centerY);
        }
        private void popupGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            ocultarPopupOk();
        }
        private void popupGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ocultarPopupOk();
        }
        private void popupGrid_TouchDown(object sender, TouchEventArgs e)
        {
            ocultarPopupOk();
        }

        #endregion

        #region METODOS - BUFFERS
        void ClearBufferSendData()
        {
            for (int i = 0; i < BufferSendData.Length; i++)
            {
                BufferSendData[i] = 0;
            }
        }
        void ClearBufferRecievedDataGlobal()
        {
            for (int i = 0; i < RecievedDataGlobal.Length; i++)
            {
                RecievedDataGlobal[i] = 0;
            }
        }
        private void agregar_folio_to_buffer_send_data(string folio)
        {
            int n = 5;
            for (int i = 0; i < 17; i++)
            {
                BufferSendData[n++] = (byte)folio[i];
            }
        }
        #endregion

        #region METODOS - DELEGADOS
        private void actualizaTxtStatus()
        {
            txtStatus.Text = ESTATUS;
        }
        private void actualizalblMontoIngresado()
        {
            lblMontoIngresado.Text = MONTO_INGRESADO;
        }

        #endregion

        private void bloqueaCamposMientrasIngresaMonedas()
        {
            grid1.IsEnabled = false;
            
            btnCancelarVenta.IsEnabled = true;

            //grid2.IsEnabled = false;
        }
        private void desbloqueaCampos()
        {
            grid1.IsEnabled = true;

            btnCancelarVenta.IsEnabled = false;

            //grid2.IsEnabled = true;
        }

        private string ObtenerSiguienteFolioAInsertarEnDbLocal()
        {
            string siguienteFolioAInsertar = "";

            ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();

            config_varios cv_pfc_ = serv_conf_varios.getEntityByClave("PREFIJO_FOLIO_BOLETO");
            string prefijo_folio_consola = cv_pfc_.valor;

            config_varios cv_nu = serv_conf_varios.getEntityByClave("NUMERO_UNIDAD");
            string name_unidad = cv_nu.valor;

            ServiceUnidades serv_unidades = new ServiceUnidades();
            ct_unidades unidad = serv_unidades.getEntityByName(name_unidad);

            ServiceBoletosTarifaFija serv_boletos = new ServiceBoletosTarifaFija();
            sy_boletos_tarifa_fija last_boleto = serv_boletos.getEntityLast();
            //Int64 last_boleto = serv_boletos.getLastEntity();

            siguienteFolioAInsertar =
                prefijo_folio_consola
                + "_"
                + unidad.pkUnidad.ToString("D6")
                + "_"
                + (last_boleto != null ? (last_boleto.pkBoleto + 1).ToString("D6") : 1.ToString("D6"));

            return siguienteFolioAInsertar;
        }
        private string obtener_folio_boleto_actual()
        {
            string folio_boleto_actual = "";

            folio_boleto_actual = ObtenerSiguienteFolioAInsertarEnDbLocal();

            return folio_boleto_actual;
        }
        private void ingresa_folio_boleto_actual_en_buffer_send_data(string folio_boleto_actual)
        {
            int n = 5;
            for (int i = 0; i < 17; i++)
            {
                BufferSendData[n++] = (byte)folio_boleto_actual[i];
            }
        }
        private string GetFolioBoletoString(byte[] RecievedDataGlobal_local)
        {
            string folio = "";
            int n = 5;
            for (int i = 0; i < 17; i++)
            {
                char varTmp = (char)RecievedDataGlobal_local[n++];
                folio += varTmp.ToString();
            }
            return folio;
        }
        private void insert_boleto_cancelado_en_db_local(byte[] RecievedDataGlobal_local)
        {
            //obtener el modo de la aplicacion
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_modo = serv_config_varios.getEntityByClave("MODO");
            string MODO_APP = cv_modo.valor;

            string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            sy_boletos_tarifa_fija obj_boleto = new sy_boletos_tarifa_fija();

            obj_boleto.pkBoletoTISA = null;
            obj_boleto.fkAsignacion = asign_activa.pkAsignacion;
            obj_boleto.fkStatus = 2; // 2 = Status Cancelado.
            obj_boleto.folio = GetFolioBoletoString(RecievedDataGlobal_local);

            
            
            obj_boleto.enviado = 0;
            obj_boleto.confirmadoTISA = 0;
            obj_boleto.modo = MODO_APP;

            obj_boleto.created_at = fecha_actual;
            obj_boleto.updated_at = null;
            obj_boleto.deleted_at = null;

            

            string varStrFechaHora = StringToBCD(22);
            string fecha = varStrFechaHora.Substring(4, 2) + '/' + varStrFechaHora.Substring(2, 2) + '/' + "20" + varStrFechaHora.Substring(0, 2);
            string hora = varStrFechaHora.Substring(6, 2) + ':' + varStrFechaHora.Substring(8, 2) + ':' + varStrFechaHora.Substring(10, 2);

            obj_boleto.fechaHoraCancelacion = fecha + " " + hora;

            uint varTotalCobrado = VarByteToUInteger32(31);
            uint varTotalPagado = VarByteToUInteger32(35);

            string strTotalCobrado = varTotalCobrado.ToString();
            string strTotalPagado = varTotalPagado.ToString();

            decimal num_cien = 100;

            obj_boleto.totalCobrado = strTotalCobrado != null && strTotalCobrado != "" ? (varTotalCobrado / num_cien) : 0;
            obj_boleto.totalPagado = strTotalPagado != null && strTotalPagado != "" ? (varTotalPagado / num_cien) : 0;

            obj_boleto.tarifa = obj_boleto.totalCobrado;
            obj_boleto.cant_pasajeros = 1;
            obj_boleto.total = (obj_boleto.cant_pasajeros * obj_boleto.tarifa);

            ServiceBoletosTarifaFija serv_boletos = new ServiceBoletosTarifaFija();
            serv_boletos.addEntity(obj_boleto);
        }
        private uint obtenerMontoIngresadoActual(byte[] RecievedDataGlobal)
        {
            uint monto_actual_ingresado = 0;
            monto_actual_ingresado = VarByteToUInteger32_comand_05(RecievedDataGlobal);
            return monto_actual_ingresado;
        }
        private sy_asignaciones ObtenerAsignacionActiva()
        {
            ServiceConfigVarios scv = new ServiceConfigVarios();
            config_varios cv = scv.getEntityByClave("ASIGNACION_ACTIVA");
            string asignacionActiva = cv.valor;

            ServiceAsignaciones sasign = new ServiceAsignaciones();
            sy_asignaciones asignacion_activa = sasign.getEntityByFolio(asignacionActiva);
            return asignacion_activa;
        }
        private void InsertarBoleto(decimal total_cobrado, decimal total_pagado)
        {
            try
            {
                //obtener el modo de la aplicacion
                ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
                config_varios cv_modo = serv_config_varios.getEntityByClave("MODO");
                string MODO_APP = cv_modo.valor;

                Int64 pkLastBoletoInserted = 0;

                decimal total = 0;
                string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (total_pagado >= total_cobrado)
                {

                    //Obtener el Folio siguiente para el boleto a insertar
                    string siguienteFolioAInsertar = ObtenerSiguienteFolioAInsertarEnDbLocal();

                    //Crear los objetos de boletos y detalle.
                    ServiceBoletosTarifaFija serv_boletos_tarifa_fija = new ServiceBoletosTarifaFija();
                    sy_boletos_tarifa_fija sb = new sy_boletos_tarifa_fija();
                    sb.pkBoleto = 0;
                    sb.pkBoletoTISA = null;
                    sb.fkAsignacion = asign_activa.pkAsignacion;
                    sb.fkStatus = 1;
                    sb.folio = siguienteFolioAInsertar;
                    sb.tarifa = total_cobrado;
                    sb.cant_pasajeros = 1;
                    sb.total = (sb.cant_pasajeros * sb.tarifa);
                    sb.totalCobrado = 0;
                    sb.totalPagado = 0;
                    sb.fechaHoraCancelacion = null;
                    sb.enviado = 0;
                    sb.confirmadoTISA = 0;
                    sb.modo = MODO_APP;
                    sb.created_at = fecha_actual;
                    sb.updated_at = null;
                    sb.deleted_at = null;

                    // Insertar boleto
                    serv_boletos_tarifa_fija.addEntity(sb);

                    // Obtener ultimo boleto insertado
                    sy_boletos_tarifa_fija sy_bol = serv_boletos_tarifa_fija.getEntityLast();
                    pkLastBoletoInserted = sy_bol.pkBoleto;

                    // Actualizar el campo total del boleto
                    ServiceBoletosTarifaFija serv_boletos_tarifa_fija_ = new ServiceBoletosTarifaFija();
                    sy_boletos_tarifa_fija sb_ = serv_boletos_tarifa_fija_.getEntity(pkLastBoletoInserted);
                    sb_.totalCobrado = total_cobrado;
                    sb_.totalPagado = total_pagado;
                    serv_boletos_tarifa_fija_.updEntity(sb_);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private decimal getTotalCobradoBoleto()
        {
            //uint total_cobrado = 0;
            decimal total_cobrado = 0;
            decimal num_cien = 100;

            uint varTotalCobrado = VarByteToUInteger32(31);
            string strTotalCobrado = varTotalCobrado.ToString();
            total_cobrado = strTotalCobrado != null && strTotalCobrado != "" ? (varTotalCobrado / num_cien) : 0;

            return total_cobrado;
        }
        private decimal getTotalPagadoBoleto()
        {
            //uint total_pagado = 0;
            decimal total_pagado = 0;
            decimal num_cien = 100;

            uint varTotalPagado = VarByteToUInteger32(35);
            string strTotalPagado = varTotalPagado.ToString();
            total_pagado = strTotalPagado != null && strTotalPagado != "" ? (varTotalPagado / num_cien) : 0;

            return total_pagado;
        }
    }
}
