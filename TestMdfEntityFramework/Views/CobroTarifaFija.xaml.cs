using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Views
{
    /// <summary>
    /// Lógica de interacción para CobroTarifaFija.xaml
    /// </summary>
    public partial class CobroTarifaFija : UserControl
    {
        private string tarifa_seleccionada;

        const decimal ByteInicio = 1;
        const decimal AddressConsola = 1;
        const decimal AddressAlcancia = 2;

        const Int32 K_offsetDatos = 4;
        const Int32 K_posicionCantidadDatos = 3;

        //SERIAL PORT
        System.IO.Ports.SerialPort puertoSerie1 = new System.IO.Ports.SerialPort();
        String[] listado_puerto = System.IO.Ports.SerialPort.GetPortNames();

        //BUFFERS SEND AND RECIEVED
        byte[] BufferSendData = new byte[80];
        byte[] RecievedDataGlobal = new byte[80];

        //TIMERS
        DispatcherTimer timerWait = new DispatcherTimer();
        private string ESTATUS = "0";

        DispatcherTimer timerEvalua = new DispatcherTimer();

        //POPUP OK
        private double left, top, right, bottom, centerX, centerY;
        private DoubleAnimation bottomToCenterAnimiation, topToCenterAnimation,
            leftToCenterAnimation, rightToCenterAnimation;
        private Storyboard bottomToCenterStoryboard, topToCenterStoryboard,
            leftToCenterStoryboard, rightToCenterStoryboard;


        //DEFINICION DE DELEGADOS
        delegate void delegate_actualizaTxtStatus();
        delegate_actualizaTxtStatus delegado_actualiza_txt_status = null;
        public CobroTarifaFija()
        {
            InitializeComponent();

            // Para actualizar el campo de txtStatus en la interfas grafica.
            //delegate_actualizaTxtStatus delegado_actualiza_txt_status = new delegate_actualizaTxtStatus(actualizaTxtStatus);
            delegado_actualiza_txt_status = new delegate_actualizaTxtStatus(actualizaTxtStatus);
            delegado_actualiza_txt_status();

            configura_puerto_serial(); // UTILIZA ENTITY FRAMEWORK CON CONEXION A database.mdf
            //configura_puerto_serial_app_config();
            
        }

        

        #region METODOS - DELEGADOS
        private void actualizaTxtStatus()
        {
            txtStatus.Text = ESTATUS;
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

                    solicitar_status_alcancia();

                    string RecievedData;
                    timerWait.IsEnabled = true;

                    // delay para esperar 8 de respuesta
                    // 30 funciona en debug de visual studio
                    // 100 para app instalada en raspberry
                    Task.WaitAll(new Task[] { Task.Delay(100) });

                    RecievedData = puertoSerie1.ReadExisting();

                    int status = Convert.ToInt32(RecievedData[5]);
                    ESTATUS = status.ToString();

                    //int status = 
                    //    RecievedData != ""
                    //    && RecievedData != ""
                    //    && RecievedData != " ??"
                    //    && RecievedData != ""
                    //    && RecievedData != "="
                    //    && RecievedData != "???????"
                    //    ? Convert.ToInt32(RecievedData[5]) : 0;
                    //ESTATUS = status.ToString();


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

                //actualizaTxtStatus();
                delegado_actualiza_txt_status();

                detiene_timers();
            }
            else if (ESTATUS == "0" || ESTATUS == "1" || ESTATUS == "2" || ESTATUS == "3")
            {
                //actualizaTxtStatus();
                delegado_actualiza_txt_status();

                bloqueaCamposMientrasIngresaMonedas();
            }
        }
        private void detiene_timers()
        {
            timerWait.IsEnabled = false;
            timerEvalua.IsEnabled = false;
        }
        #endregion

        #region METODOS - CONTROLES INTERFAS GRAFICA
        private void bloqueaCamposMientrasIngresaMonedas()
        {
            btnTarifa_1.IsEnabled = false;
            btnTarifa_2.IsEnabled = false;
            cmbCantidadPasajeros.IsEnabled = false;
            btnEnviarTarifa.IsEnabled = false;
        }
        private void desbloqueaCampos()
        {
            btnTarifa_1.IsEnabled = true;
            btnTarifa_2.IsEnabled = true;
            cmbCantidadPasajeros.IsEnabled = true;
            btnEnviarTarifa.IsEnabled = true;

            //para mostrar el popup ok
            mostrarPopupOk();
        }
        #endregion

        #region METODOS - ALCANCIA
        private void solicitar_status_alcancia()
        {
            //timerPoll.Enabled = false;
            string BufferSendData = "\x0001" + "\x0002" + "\x0001" + "\x0001" + "\x0000" + "\x0041" + "\x0042";
            puertoSerie1.WriteLine(BufferSendData);
            //timerWait.Enabled = true;
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
        #endregion

        #region METODOS - DB LOCAL MDF
        private UInt32 ObtenerUltimoFolioInsertado()
        {
            //AQUI SE OBTENDRA EL ULTIMO FOLIO DE LA BASE DE DATOS LOCAL
            ServiceBoletosTarifaFija sbtf = new ServiceBoletosTarifaFija();
            UInt32 ultimo_folio = sbtf.getLastEntity();


            return ultimo_folio;
        }
        #endregion

        #region EVENTOS BOTONES Y VENTANAS
        private void CobroTarifaFija_OnLoad(object sender, RoutedEventArgs e)
        {
            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();
        }
        private void CobroTarifaFija_OnUnload(object sender, RoutedEventArgs e)
        {
            close_serial_port();

        }
        private void btnTarifa1_Click(object sender, RoutedEventArgs e)
        {
            //Ocultar el popup ok en caso se estar mostrandose
            ocultarPopupOk();


            tarifa_seleccionada = txtTarifa1.Text;

            btnTarifa_1.Background = Brushes.Blue;
            txtTarifa1.Foreground = Brushes.White;

            btnTarifa_2.Background = Brushes.GreenYellow;
            txtTarifa2.Foreground = Brushes.Black;
        }
        private void btnTarifa2_Click(object sender, RoutedEventArgs e)
        {
            //Ocultar el popup ok en caso se estar mostrandose
            ocultarPopupOk();

            tarifa_seleccionada = txtTarifa2.Text;

            btnTarifa_2.Background = Brushes.Blue;
            txtTarifa2.Foreground = Brushes.White;

            btnTarifa_1.Background = Brushes.GreenYellow;
            txtTarifa1.Foreground = Brushes.Black;
        }
        private void btnEnviarTarifa_Click(object sender, RoutedEventArgs e)
        {
            if (tarifa_seleccionada != null && tarifa_seleccionada != "")
            {
                bloqueaCamposMientrasIngresaMonedas();

                inicializa_timer_wait();
                inicializa_timer_evalua();

            
                tarifa_seleccionada = tarifa_seleccionada.Replace(".", "");

                const Int32 K_posicionCantidadTarifas = 11;
                //const decimal CantidadDatos = 17;
                const decimal Comando = 1;
                //const byte CantidadTarifas = 1;
                const decimal CRC1 = 193;
                const decimal CRC2 = 194;

                Int32 CantidadDatos = 0;
                Decimal CantidadTarifas = 0;
                UInt32 PrecioTarifa1 = Convert.ToUInt32(tarifa_seleccionada);

                //FOLIO BOLETO
                UInt32 folioVenta = 1; //ObtenerUltimoFolioInsertado(); CONSULTA LA BASE DE DATOS database.mdf
                folioVenta += 1;
                //txtFolioVenta.Text = folioVenta.ToString();


                int CantUsuarios = Convert.ToInt32(cmbCantidadPasajeros.Text);
                DateTime varFechaHora = DateTime.Now;

                byte[] BCDDateTime = ToBCD_DT(varFechaHora);

                ClearBufferSendData();

                BufferSendData[0] = decimal.ToByte(ByteInicio);
                BufferSendData[1] = decimal.ToByte(AddressConsola);
                BufferSendData[2] = decimal.ToByte(AddressAlcancia);

                BufferSendData[4] = decimal.ToByte(Comando);
                CantidadDatos += 1;

                // PREGUNTAR A GILBERTO
                varInteger32ToByte(K_offsetDatos + CantidadDatos, folioVenta);
                CantidadDatos += 4;

                //COLOCA FECHA EN EL BUFFER
                BufferSendData[9] = BCDDateTime[0];
                BufferSendData[10] = BCDDateTime[1];
                BufferSendData[11] = BCDDateTime[2];
                BufferSendData[12] = BCDDateTime[3];
                BufferSendData[13] = BCDDateTime[4];
                BufferSendData[14] = BCDDateTime[5];
                CantidadDatos += 6;

                // Se incrementa la variable de "CantidadDatos" para compensar la posición de las tarifas
                //BufferSendData[K_offsetDatos + K_posicionCantidadTarifas] = decimal.ToByte(CantidadTarifas);
                CantidadDatos += 1;

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
            }
        }

        #endregion

        #region SERIAL PORT
        public void configura_puerto_serial_app_config()
        {
            try
            {
                Comun mc = new Comun();
                string port_name = mc.obtenerValorDeAppConfig("PORT_NAME");
                string baud_rate = mc.obtenerValorDeAppConfig("BAUD_RATE");
                string data_bits = mc.obtenerValorDeAppConfig("DATA_BITS");
                string stop_bits = mc.obtenerValorDeAppConfig("STOP_BITS");
                string parity = mc.obtenerValorDeAppConfig("PARITY");

                this.puertoSerie1 = new System.IO.Ports.SerialPort("" + port_name 
                                                      , Convert.ToInt32(baud_rate)
                                                      , parity == "NONE" ? System.IO.Ports.Parity.None : System.IO.Ports.Parity.Mark
                                                      , Convert.ToInt32(data_bits)
                                                      , Convert.ToInt32(stop_bits) == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.None
                                                      );
                puertoSerie1.Handshake = Handshake.None;
            }
            catch
            {
                MessageBox.Show("Verifique" + System.Environment.NewLine + "- Alimentación" + System.Environment.NewLine + "- Conexión del puerto", "Error de puerto COMM");
                btnEnviarTarifa.IsEnabled = false;
            }

        }
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
                btnEnviarTarifa.IsEnabled = false;
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
        void varInteger32ToByte(Int32 IndiceBuffer, UInt32 Var32)
        {
            BufferSendData[IndiceBuffer] = (byte)Var32;
            BufferSendData[IndiceBuffer + 1] = (byte)(Var32 >> 8);
            BufferSendData[IndiceBuffer + 2] = (byte)(Var32 >> 16);
            BufferSendData[IndiceBuffer + 3] = (byte)(Var32 >> 24);
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
        #endregion

    }
}
