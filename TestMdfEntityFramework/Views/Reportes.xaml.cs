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
using TestMdfEntityFramework.EntityServices;

namespace TestMdfEntityFramework.Views
{
    /// <summary>
    /// Lógica de interacción para Reportes.xaml
    /// </summary>
    public partial class Reportes : UserControl
    {
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

        //POPUP OK
        private double left, top, right, bottom, centerX, centerY;
        private DoubleAnimation bottomToCenterAnimiation, topToCenterAnimation,
            leftToCenterAnimation, rightToCenterAnimation;
        private Storyboard bottomToCenterStoryboard, topToCenterStoryboard,
            leftToCenterStoryboard, rightToCenterStoryboard;

        public Reportes()
        {
            InitializeComponent();

            configura_puerto_serial();
        }

        private void Reportes_Load(object sender, RoutedEventArgs e)
        {
            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();
        }

        private void Reportes_OnUnload(object sender, RoutedEventArgs e)
        {
            close_serial_port();
        }

        private void btnCorteCaja_Click(object sender, RoutedEventArgs e)
        {
            if (validaDispositivoConectadoEnPuertoCOM())
            {
                DateTime varFechaHora = DateTime.Now;
                byte[] BCDDateTime = ToBCD_DT(varFechaHora);

                const decimal Comando = 3;
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
                string folio_corte = obtener_siguiente_folio_corte(); //"COR_000001_000001";

                ingresa_folio_en_buffer_send_data(folio_corte);
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

                Task.WaitAll(new Task[] { Task.Delay(200) });
                puertoSerie1.Read(RecievedDataGlobal, 0, 50);

                if (RecievedDataGlobal != null)
                {
                    //TODO: Popup notificando que se ha impreso el corte de forma correcta.

                    insert_corte_db_local(RecievedDataGlobal);
                    Task.WaitAll(new Task[] { Task.Delay(100) });

                    ejecutar_commando_04();
                    Task.WaitAll(new Task[] { Task.Delay(100) });

                    txtMensajePopup.Text = "CORTE IMPRESO";
                    mostrarPopupOk();

                    //MessageBox.Show("Corte Impreso desde Alcancia", "IMPRESION CORTE");
                }
                else
                {
                    MessageBox.Show("Algo salio mal", "ERROR");
                }
            }
            else
            {
                MessageBox.Show("Favor de Validar que este conectado el dispositivo al puerto COM");
            }
            
        }

        private void ejecutar_commando_04()
        {
            int CantidadDatos = 5;
            const decimal Comando = 4;
            const decimal CRC1 = 193;
            const decimal CRC2 = 194;
            const byte Code_xAA = 170;
            const byte Code_x55 = 85;

            ClearBufferSendData();

            BufferSendData[0] = decimal.ToByte(ByteInicio);
            BufferSendData[1] = decimal.ToByte(AddressAlcancia);
            BufferSendData[2] = decimal.ToByte(AddressConsola);
            BufferSendData[3] = decimal.ToByte(CantidadDatos);
            BufferSendData[4] = decimal.ToByte(Comando);

            BufferSendData[5] = Code_xAA;
            BufferSendData[6] = Code_x55;
            BufferSendData[7] = Code_xAA;
            BufferSendData[8] = Code_x55;

            BufferSendData[9] = decimal.ToByte(CRC1);
            BufferSendData[10] = decimal.ToByte(CRC2);

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);

        }

        private string obtener_siguiente_folio_corte()
        {
            string folio_corte_siguiente = "";

            ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();

            config_varios cv_pfc_ = serv_conf_varios.getEntityByClave("PREFIJO_FOLIO_CORTE");
            string prefijo_folio_corte = cv_pfc_.valor;

            config_varios cv_nu = serv_conf_varios.getEntityByClave("NUMERO_UNIDAD");
            string name_unidad = cv_nu.valor;

            ServiceUnidades serv_unidades = new ServiceUnidades();
            ct_unidades unidad = serv_unidades.getEntityByName(name_unidad);

            ServiceCortes serv_cortes = new ServiceCortes();
            sy_cortes obj_corte = serv_cortes.getEntityLast();
            if(obj_corte != null)
            {
                folio_corte_siguiente =
                prefijo_folio_corte
                + "_"
                + unidad.pkUnidad.ToString("D6")
                + "_"
                + (obj_corte != null ? (obj_corte.pkCorte + 1).ToString("D6") : 1.ToString("D6"));
            }
            else
            {
                folio_corte_siguiente =
                prefijo_folio_corte
                + "_"
                + unidad.pkUnidad.ToString("D6")
                + "_"
                + 1.ToString("D6");
            }

            return folio_corte_siguiente;
        }
        private void ingresa_folio_en_buffer_send_data(string folio_corte)
        {
            int n = 5;
            for (int i = 0; i < 17; i++)
            {
                BufferSendData[n++] = (byte)folio_corte[i];
            }
        }
        private void insert_corte_db_local(byte[] RecievedDataGlobal_local)
        {
            sy_cortes obj_corte = new sy_cortes();

            obj_corte.fkAsignacion = 0;
            obj_corte.fkLugarOrigen = 0;
            obj_corte.fkLugarDestino = 0;
            obj_corte.fkStatus = 1;

            obj_corte.folio = GetFolioCorteSrting(RecievedDataGlobal_local);

            string varStrFechaHora = StringToBCD(22);
            string fecha = varStrFechaHora.Substring(4, 2) + '/' + varStrFechaHora.Substring(2, 2) + '/' + "20" + varStrFechaHora.Substring(0, 2);
            string hora = varStrFechaHora.Substring(6, 2) + ':' + varStrFechaHora.Substring(8, 2) + ':' + varStrFechaHora.Substring(10, 2);

            obj_corte.fecha = fecha;
            obj_corte.hora = hora;

            uint varTotalEfectivo = VarByteToUInteger32(31);
            uint varTotalTarifasRst = VarByteToUInteger32(35);
            uint varTotalEfectivoRst = VarByteToUInteger32(39);

            string strTotalEfectivo = varTotalEfectivo.ToString();
            string strTarifaRst = varTotalTarifasRst.ToString();
            string strEfectivoRst = varTotalEfectivoRst.ToString();

            obj_corte.total_efectivo_acumulado = strTotalEfectivo != null && strTotalEfectivo != "" ? (varTotalEfectivo/100) : 0;
            obj_corte.total_tarifas = strTarifaRst != null && strTarifaRst != "" ? (varTotalTarifasRst/100) : 0;
            obj_corte.total_efectivo_rst = strEfectivoRst != null && strEfectivoRst != "" ? (varTotalEfectivoRst/100) : 0;

            ServiceCortes serv_cortes = new ServiceCortes();
            serv_cortes.addEntity(obj_corte);
        }

        private string GetFolioCorteSrting(byte[] RecievedDataGlobal_local)
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
        private uint VarByteToUInteger32(Int32 IndiceBuffer)
        {
            uint varFolioTemporal = 0;
            for (int i = 0; i < 4; i++)
            {
                varFolioTemporal <<= 8;
                varFolioTemporal |= RecievedDataGlobal[IndiceBuffer--];
            }
            return varFolioTemporal;
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

        private bool validaDispositivoConectadoEnPuertoCOM()
        {
            bool hayDispositivoConectado = false;

            try
            {
                if (puertoSerie1.IsOpen == false)
                {
                    puertoSerie1.Open();
                    hayDispositivoConectado = true;
                }
            }
            catch (Exception ex)
            {
                hayDispositivoConectado = false;
                MessageBox.Show(ex.Message);
            }

            return hayDispositivoConectado;
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
            }
            catch
            {
                MessageBox.Show("Verifique" + System.Environment.NewLine + "- Alimentación" + System.Environment.NewLine + "- Conexión del puerto", "Error de puerto COMM");
            }

        }

        private void open_serial_port()
        {
            try
            {
                if (puertoSerie1.IsOpen == false)
                {
                    puertoSerie1.Open();
                }
            }
            catch (Exception ex)
            {
                configura_puerto_serial();

            }
        }
        private void open_serial_port_v2()
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

        void ClearBufferSendData()
        {
            for (int i = 0; i < BufferSendData.Length; i++)
            {
                BufferSendData[i] = 0;
            }
        }

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


    }
}
