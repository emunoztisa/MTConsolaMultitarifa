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

            //configura_puerto_serial();
        }

        private void Reportes_Load(object sender, RoutedEventArgs e)
        {
            if (validaPuertoCOMConfigurado())
            {
                configura_puerto_serial();
            }

            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();
        }

        private void Reportes_OnUnload(object sender, RoutedEventArgs e)
        {
            close_serial_port();
            dispose_serial_port();
        }

        private void btnCorteCaja_Click(object sender, RoutedEventArgs e)
        {
            if (validaDispositivoConectadoEnPuertoCOM())
            {
                //ejecutar_commando_03_corte_ventas_realizadas();
                ejecutar_commando_08_corte_ventas_realizadas_con_desglose_monedas_billetes();

                //Task.WaitAll(new Task[] { Task.Delay(200) });
                //puertoSerie1.Read(RecievedDataGlobal, 0, 50);

                Task.WaitAll(new Task[] { Task.Delay(300) });
                puertoSerie1.Read(RecievedDataGlobal, 0, 80);

                if (RecievedDataGlobal != null)
                {
                    //TODO: Popup notificando que se ha impreso el corte de forma correcta.

                    insert_corte_db_local(RecievedDataGlobal);
                    Task.WaitAll(new Task[] { Task.Delay(100) });

                    ejecutar_commando_04_reiniciar_contadores();
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
        private void ejecutar_commando_03_corte_ventas_realizadas()
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

            //close_serial_port();
            //dispose_serial_port();

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);
        }
        private void ejecutar_commando_04_reiniciar_contadores()
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

            //close_serial_port();
            //dispose_serial_port();

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);

        }
        private void ejecutar_commando_08_corte_ventas_realizadas_con_desglose_monedas_billetes()
        {
            DateTime varFechaHora = DateTime.Now;
            byte[] BCDDateTime = ToBCD_DT(varFechaHora);

            const decimal Comando = 8;
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

            //close_serial_port();
            //dispose_serial_port();

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

            //DESGLOSE DE CANTIDAD DE BILLETES POR DENOMINACION
            //Agregar la cantidad de piezas por denominacion al corte
            string[] arrCantPiezasPorDenominacion = VarByteToUInteger32_comand_08_tipo_moneda_billete(RecievedDataGlobal_local);

            obj_corte.cant_mon_tipo_0 = arrCantPiezasPorDenominacion[0];
            obj_corte.cant_mon_tipo_1 = arrCantPiezasPorDenominacion[1];
            obj_corte.cant_mon_tipo_2 = arrCantPiezasPorDenominacion[2];
            obj_corte.cant_mon_tipo_3 = arrCantPiezasPorDenominacion[3];
            obj_corte.cant_mon_tipo_4 = arrCantPiezasPorDenominacion[4];
            //obj_corte.cant_mon_tipo_5 = arrCantPiezasPorDenominacion[5];
            //obj_corte.cant_mon_tipo_6 = arrCantPiezasPorDenominacion[6];

            obj_corte.cant_bill_tipo_0 = arrCantPiezasPorDenominacion[7];
            obj_corte.cant_bill_tipo_1 = arrCantPiezasPorDenominacion[8];
            obj_corte.cant_bill_tipo_2 = arrCantPiezasPorDenominacion[9];
            obj_corte.cant_bill_tipo_3 = arrCantPiezasPorDenominacion[10];
            obj_corte.cant_bill_tipo_4 = arrCantPiezasPorDenominacion[11];
            obj_corte.cant_bill_tipo_5 = arrCantPiezasPorDenominacion[12];
            //obj_corte.cant_bill_tipo_6 = arrCantPiezasPorDenominacion[13];

            obj_corte.efectivo_moneda = arrCantPiezasPorDenominacion[14];
            obj_corte.efectivo_billete = arrCantPiezasPorDenominacion[15];

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
        private string[] VarByteToUInteger32_comand_08_tipo_moneda_billete(byte[] RecievedDataGlobal)
        {

            string[] arrPiezasDenominaciones = new string[16];
            decimal num_cien = 100;
            uint varCantTemp = 0;

            //EFECTIVO MONEDAS
            varCantTemp = 0;
            for (int i = 43; i >= 40; i--) // para iterar por las posiciones de ReceivedData, donde sabemos viene el monto actual ingresado.
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string efectivo_moneda = (varCantTemp / num_cien).ToString();
            arrPiezasDenominaciones[14] = efectivo_moneda;

            //EFECTIVO BILLETES
            varCantTemp = 0;
            for (int i = 47; i >= 44; i--) // para iterar por las posiciones de ReceivedData, donde sabemos viene el monto actual ingresado.
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string efectivo_billete = (varCantTemp / num_cien).ToString();
            arrPiezasDenominaciones[15] = efectivo_billete;

            //Cantidad Monedas 50 centavos
            varCantTemp <<= 16;                 // se hace el corrimiento de los 16 bits que pone ceros implicitamente, para completar la variable entera de 32 bits.
            for (int i = 49; i >= 48; i--)    // para iterar por las posiciones de ReceivedData, donde sabemos viene el monto actual ingresado.
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            //string cantidad_piezas_50_centavos = (varCantTemp / num_cien).ToString();
            string cantidad_piezas_50_centavos = (varCantTemp).ToString();
            arrPiezasDenominaciones[0] = cantidad_piezas_50_centavos;

            //Cantidad Monedas 1 Peso
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 51; i >= 50; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_1_peso = (varCantTemp).ToString();
            arrPiezasDenominaciones[1] = cantidad_piezas_1_peso;

            //Cantidad Monedas 2 Pesos
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 53; i >= 52; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_2_pesos = (varCantTemp).ToString();
            arrPiezasDenominaciones[2] = cantidad_piezas_2_pesos;

            //Cantidad Monedas 5 Pesos
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 55; i >= 54; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_5_pesos = (varCantTemp).ToString();
            arrPiezasDenominaciones[3] = cantidad_piezas_5_pesos;

            //Cantidad Monedas 10 Pesos
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 57; i >= 56; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_10_pesos = (varCantTemp).ToString();
            arrPiezasDenominaciones[4] = cantidad_piezas_10_pesos;


            /*
             AQUI OTRAS DENOMINACIONES PARA MONEDAS en las posiciones 5 y 6 del arreglo.
             */


            //Cantidad Monedas 20 Pesos
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 63; i >= 62; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_20_pesos = (varCantTemp).ToString();
            arrPiezasDenominaciones[7] = cantidad_piezas_20_pesos;

            //Cantidad Monedas 50 Pesos
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 65; i >= 64; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_50_pesos = (varCantTemp).ToString();
            arrPiezasDenominaciones[8] = cantidad_piezas_50_pesos;

            //Cantidad Monedas 100 Pesos
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 67; i >= 66; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_100_pesos = (varCantTemp).ToString();
            arrPiezasDenominaciones[9] = cantidad_piezas_100_pesos;

            //Cantidad Monedas 200 Pesos
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 69; i >= 68; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_200_pesos = (varCantTemp).ToString();
            arrPiezasDenominaciones[10] = cantidad_piezas_200_pesos;

            //Cantidad Monedas 500 Pesos
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 71; i >= 70; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_500_pesos = (varCantTemp).ToString();
            arrPiezasDenominaciones[11] = cantidad_piezas_500_pesos;

            //Cantidad Monedas 1000 Pesos
            varCantTemp = 0;
            varCantTemp <<= 16;
            for (int i = 73; i >= 72; i--)
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            string cantidad_piezas_1000_pesos = (varCantTemp).ToString();
            arrPiezasDenominaciones[12] = cantidad_piezas_1000_pesos;

            //aqui abajo se pondria otra denominacion en la posicion 13 del arreglo
            ////Cantidad Monedas 5000 Pesos
            //varCantTemp = 0;
            //varCantTemp <<= 16;
            //for (int i = 75; i >= 74; i--)
            //{
            //    varCantTemp <<= 8;
            //    varCantTemp |= RecievedDataGlobal[i];
            //}
            //string cantidad_piezas_5000_pesos = (varCantTemp).ToString();
            //arrPiezasDenominaciones[13] = cantidad_piezas_5000_pesos;


            return arrPiezasDenominaciones;
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

                close_serial_port(); //EMD 2024-05-07
                open_serial_port(); //EMD 2024-05-07
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
        private void dispose_serial_port()
        {
            if (puertoSerie1.IsOpen == false)
            {
                puertoSerie1.Dispose();
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

        #region VALIDACIONES
        private bool validaPuertoCOMConfigurado()
        {
            bool isConfigured = false;

            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_port_name = serv_config_varios.getEntityByClave("PORT_NAME");
            if (cv_port_name.valor != null && cv_port_name.valor != "")
            {
                isConfigured = true;
            }
            else
            {
                isConfigured = false;
            }

            return isConfigured;
        }
        private bool isValidComPortAndConnected()
        {
            bool isValid;
            try
            {
                open_serial_port();
                isValid = true;
            }
            catch (Exception ex)
            {
                isValid = false;
            }
            return isValid;
        }
        private bool validaDispositivoConectadoEnPuertoCOM()
        {
            bool hayDispositivoConectado = false;

            try
            {
                if (puertoSerie1.IsOpen == false)
                {
                    //close_serial_port();
                    //open_serial_port();
                    //puertoSerie1.Dispose();

                    //puertoSerie1.Open();

                    hayDispositivoConectado = true;
                }
                else
                {
                    hayDispositivoConectado = true;
                }
            }
            catch (Exception ex)
            {
                hayDispositivoConectado = true;
                MessageBox.Show(ex.Message);
            }

            return hayDispositivoConectado;
        }

        #endregion



    }
}
