using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Views
{
    /// <summary>
    /// Lógica de interacción para CobroMultitarifaV1.xaml
    /// </summary>
    public partial class CobroMultitarifaV1 : UserControl
    {
        sy_asignaciones asign_activa = null;

        //CONSTANTES DE CONTROL DE FLUJO EN EL PROGRAMA
        //private string tarifa_seleccionada;

        const decimal ByteInicio = 1;
        const decimal AddressConsola = 1;
        const decimal AddressAlcancia = 2;

        const Int32 K_offsetDatos = 4;
        const Int32 K_posicionCantidadDatos = 3;


        //BUFFERS SEND AND RECIEVED
        byte[] BufferSendData = new byte[80];
        byte[] RecievedDataGlobal = new byte[80];

        //ARREGLO DE CANTIDAD DE PIEZAS POR DENOMINACION
        int[] arrPiezasDenominaciones = new int[13];

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

        #region DEFINICION DE VARIABLES PARA EL PROCESO DE MULTITARIFA
        string lugOri = "";
        string lugDes = "";

        string perfil1 = "";
        string perfil2 = "";
        string perfil3 = "";
        string perfil4 = "";
        string perfil5 = "";
        string perfil6 = "";

        int cant1 = 0;
        int cant2 = 0;
        int cant3 = 0;
        int cant4 = 0;
        int cant5 = 0;
        int cant6 = 0;
        #endregion

        int CANTIDAD_VECES_INSERTA_BOLETO = 0;


        public CobroMultitarifaV1()
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

            //configura_puerto_serial(); // UTILIZA ENTITY FRAMEWORK CON CONEXION A database.mdf

        }

        #region EVENTOS BOTONES Y VENTANAS
        private void CobroMultitarifaV1_Load(object sender, RoutedEventArgs e)
        {
            if (validaPuertoCOMConfigurado())
            {
                configura_puerto_serial();
            }

            asign_activa = ObtenerAsignacionActiva();

            LlenaComboLugarOrigen();
            LlenaComboLugarDestino();
            LlenaCombosPerfiles();
            LlenaCombosPerfilesCantidad();

            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();

            btnCancelarVenta.IsEnabled = false;

            ////delegado para limpiar los campos
            //delegado_limpiar_campos_despues_de_venta_boleto = new delegate_limpiarCamposDespuesDeVentaBoleto(LimpiarCamposDespuesDeVentaBoleto);
            //delegado_limpiar_campos_despues_de_venta_boleto();

        }
        private void CobroMultitarifaV1_Unload(object sender, RoutedEventArgs e)
        {
            close_serial_port();
            dispose_serial_port();
        }

        //private void btnCobrar_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        bloqueaCamposMientrasIngresaMonedas();

        //        //inicializa_timer_wait();
        //        //inicializa_timer_evalua();

        //        const Int32 K_posicionCantidadTarifas = 11;
        //        //const decimal CantidadDatos = 17;
        //        const decimal Comando = 1;
        //        //const byte CantidadTarifas = 1;
        //        const decimal CRC1 = 193;
        //        const decimal CRC2 = 194;

        //        Int32 CantidadDatos = 0;
        //        Decimal CantidadTarifas = 0;

        //        //FOLIO BOLETO
        //        UInt32 folioVenta = 1;
        //        folioVenta += 1;

        //        ////FOLIO BOLETO
        //        //string folioVenta = ObtenerSiguienteFolioAInsertarEnDbLocal();  //CONSULTA LA BASE DE DATOS database.mdf

        //        DateTime varFechaHora = DateTime.Now;

        //        byte[] BCDDateTime = ToBCD_DT(varFechaHora);

        //        ClearBufferSendData();

        //        BufferSendData[0] = decimal.ToByte(ByteInicio);
        //        BufferSendData[1] = decimal.ToByte(AddressConsola);
        //        BufferSendData[2] = decimal.ToByte(AddressAlcancia);

        //        BufferSendData[4] = decimal.ToByte(Comando);
        //        CantidadDatos += 1;

        //        // PREGUNTAR A GILBERTO
        //        varInteger32ToByte(K_offsetDatos + CantidadDatos, folioVenta);
        //        CantidadDatos += 4;


        //        //COLOCA FECHA EN EL BUFFER
        //        BufferSendData[9] = BCDDateTime[0];
        //        BufferSendData[10] = BCDDateTime[1];
        //        BufferSendData[11] = BCDDateTime[2];
        //        BufferSendData[12] = BCDDateTime[3];
        //        BufferSendData[13] = BCDDateTime[4];
        //        BufferSendData[14] = BCDDateTime[5];
        //        CantidadDatos += 6;

        //        // Se incrementa la variable de "CantidadDatos" para compensar la posición de las tarifas
        //        //BufferSendData[K_offsetDatos + K_posicionCantidadTarifas] = decimal.ToByte(CantidadTarifas);
        //        CantidadDatos += 1;


        //        //PERFIL y CANT UNO
        //        string tarifa_1 = ObtenerTarifa(1); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
        //        tarifa_1 = tarifa_1.Replace(".", "");
        //        UInt32 PrecioTarifa1 = Convert.ToUInt32(tarifa_1);
        //        int cant1 = cmbCantUno.Text != "" ? Convert.ToInt32(cmbCantUno.Text) : 0;
        //        if (!(cant1 == 0) && !(PrecioTarifa1 == 0))
        //        {
        //            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant1);
        //            CantidadDatos += 1;

        //            varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa1);
        //            CantidadDatos += 4;
        //            CantidadTarifas += 1;
        //        }

        //        //PERFIL y CANT DOS
        //        string tarifa_2 = ObtenerTarifa(2); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
        //        tarifa_2 = tarifa_2.Replace(".", "");
        //        UInt32 PrecioTarifa2 = Convert.ToUInt32(tarifa_2);
        //        int cant2 = cmbCantDos.Text != "" ? Convert.ToInt32(cmbCantDos.Text) : 0;
        //        if (!(cant2 == 0) && !(PrecioTarifa2 == 0))
        //        {
        //            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant2);
        //            CantidadDatos += 1;

        //            varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa2);
        //            CantidadDatos += 4;
        //            CantidadTarifas += 1;
        //        }

        //        //PERFIL y CANT TRES
        //        string tarifa_3 = ObtenerTarifa(3); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
        //        tarifa_3 = tarifa_3.Replace(".", "");
        //        UInt32 PrecioTarifa3 = Convert.ToUInt32(tarifa_3);
        //        int cant3 = cmbCantTres.Text != "" ? Convert.ToInt32(cmbCantTres.Text) : 0;
        //        if (!(cant3 == 0) && !(PrecioTarifa3 == 0))
        //        {
        //            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant3);
        //            CantidadDatos += 1;

        //            varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa3);
        //            CantidadDatos += 4;
        //            CantidadTarifas += 1;
        //        }

        //        //PERFIL y CANT CUATRO
        //        string tarifa_4 = ObtenerTarifa(4); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
        //        tarifa_4 = tarifa_4.Replace(".", "");
        //        UInt32 PrecioTarifa4 = Convert.ToUInt32(tarifa_4);
        //        int cant4 = cmbCantCuatro.Text != "" ? Convert.ToInt32(cmbCantCuatro.Text) : 0;
        //        if (!(cant4 == 0) && !(PrecioTarifa4 == 0))
        //        {
        //            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant4);
        //            CantidadDatos += 1;

        //            varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa4);
        //            CantidadDatos += 4;
        //            CantidadTarifas += 1;
        //        }

        //        //PERFIL y CANT CINCO
        //        string tarifa_5 = ObtenerTarifa(5); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
        //        tarifa_5 = tarifa_5.Replace(".", "");
        //        UInt32 PrecioTarifa5 = Convert.ToUInt32(tarifa_5);
        //        int cant5 = cmbCantCinco.Text != "" ? Convert.ToInt32(cmbCantCinco.Text) : 0;
        //        if (!(cant5 == 0) && !(PrecioTarifa5 == 0))
        //        {
        //            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant5);
        //            CantidadDatos += 1;

        //            varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa5);
        //            CantidadDatos += 4;
        //            CantidadTarifas += 1;
        //        }

        //        //PERFIL y CANT SEIS
        //        string tarifa_6 = ObtenerTarifa(6); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
        //        tarifa_6 = tarifa_6.Replace(".", "");
        //        UInt32 PrecioTarifa6 = Convert.ToUInt32(tarifa_6);
        //        int cant6 = cmbCantSeis.Text != "" ? Convert.ToInt32(cmbCantSeis.Text) : 0;
        //        if (!(cant6 == 0) && !(PrecioTarifa6 == 0))
        //        {
        //            BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant6);
        //            CantidadDatos += 1;

        //            varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa6);
        //            CantidadDatos += 4;
        //            CantidadTarifas += 1;
        //        }


        //        //La cantidad de datos se incremento previo a copiar las tarifas y cantidad de usuarios.
        //        BufferSendData[K_offsetDatos + K_posicionCantidadTarifas] = decimal.ToByte(CantidadTarifas);

        //        BufferSendData[K_posicionCantidadDatos] = (byte)(CantidadDatos);

        //        BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
        //        BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);


        //        ClearBufferRecievedDataGlobal();

        //        //ESCRIBIR EN EL PUERTO COM DE LA ALCANCIA
        //        open_serial_port();
        //        puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);

        //        //PRUEBAS GIL
        //        //solicitar_status_alcancia_commando_05();
        //        //solicitar_total_cobrado_ultima_venta_commando_02();

        //        Task.WaitAll(new Task[] { Task.Delay(200) });
        //        puertoSerie1.Read(RecievedDataGlobal, 0, 32);

        //        if (RecievedDataGlobal[5] == 0)
        //        {
        //            // Entrara aqui solo si la respuesta a la asignacion de tarifa es: 00
        //            // Comando recibido correctamente.

        //            //ClearBufferRecievedDataGlobal();

        //            inicializa_timer_wait();
        //            inicializa_timer_evalua();
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}

        private void btnCobrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //recalcular_tarifa(sender);

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


                //PERFIL y CANT UNO
                string tarifa_1 = ObtenerTarifa(1); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
                tarifa_1 = tarifa_1.Replace(".", "");
                UInt32 PrecioTarifa1 = Convert.ToUInt32(tarifa_1);
                int cant1 = cmbCantUno.Text != "" ? Convert.ToInt32(cmbCantUno.Text) : 0;
                if (!(cant1 == 0) && !(PrecioTarifa1 == 0))
                {
                    BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant1);
                    CantidadDatos += 1;

                    varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa1);
                    CantidadDatos += 4;
                    CantidadTarifas += 1;
                }

                //PERFIL y CANT DOS
                string tarifa_2 = ObtenerTarifa(2); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
                tarifa_2 = tarifa_2.Replace(".", "");
                UInt32 PrecioTarifa2 = Convert.ToUInt32(tarifa_2);
                int cant2 = cmbCantDos.Text != "" ? Convert.ToInt32(cmbCantDos.Text) : 0;
                if (!(cant2 == 0) && !(PrecioTarifa2 == 0))
                {
                    BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant2);
                    CantidadDatos += 1;

                    varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa2);
                    CantidadDatos += 4;
                    CantidadTarifas += 1;
                }

                //PERFIL y CANT TRES
                string tarifa_3 = ObtenerTarifa(3); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
                tarifa_3 = tarifa_3.Replace(".", "");
                UInt32 PrecioTarifa3 = Convert.ToUInt32(tarifa_3);
                int cant3 = cmbCantTres.Text != "" ? Convert.ToInt32(cmbCantTres.Text) : 0;
                if (!(cant3 == 0) && !(PrecioTarifa3 == 0))
                {
                    BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant3);
                    CantidadDatos += 1;

                    varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa3);
                    CantidadDatos += 4;
                    CantidadTarifas += 1;
                }

                //PERFIL y CANT CUATRO
                string tarifa_4 = ObtenerTarifa(4); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
                tarifa_4 = tarifa_4.Replace(".", "");
                UInt32 PrecioTarifa4 = Convert.ToUInt32(tarifa_4);
                int cant4 = cmbCantCuatro.Text != "" ? Convert.ToInt32(cmbCantCuatro.Text) : 0;
                if (!(cant4 == 0) && !(PrecioTarifa4 == 0))
                {
                    BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant4);
                    CantidadDatos += 1;

                    varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa4);
                    CantidadDatos += 4;
                    CantidadTarifas += 1;
                }

                //PERFIL y CANT CINCO
                string tarifa_5 = ObtenerTarifa(5); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
                tarifa_5 = tarifa_5.Replace(".", "");
                UInt32 PrecioTarifa5 = Convert.ToUInt32(tarifa_5);
                int cant5 = cmbCantCinco.Text != "" ? Convert.ToInt32(cmbCantCinco.Text) : 0;
                if (!(cant5 == 0) && !(PrecioTarifa5 == 0))
                {
                    BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant5);
                    CantidadDatos += 1;

                    varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa5);
                    CantidadDatos += 4;
                    CantidadTarifas += 1;
                }

                //PERFIL y CANT SEIS
                string tarifa_6 = ObtenerTarifa(6); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
                tarifa_6 = tarifa_6.Replace(".", "");
                UInt32 PrecioTarifa6 = Convert.ToUInt32(tarifa_6);
                int cant6 = cmbCantSeis.Text != "" ? Convert.ToInt32(cmbCantSeis.Text) : 0;
                if (!(cant6 == 0) && !(PrecioTarifa6 == 0))
                {
                    BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(cant6);
                    CantidadDatos += 1;

                    varInteger32ToByte(K_offsetDatos + CantidadDatos, PrecioTarifa6);
                    CantidadDatos += 4;
                    CantidadTarifas += 1;
                }


                //La cantidad de datos se incremento previo a copiar las tarifas y cantidad de usuarios.
                BufferSendData[K_offsetDatos + K_posicionCantidadTarifas] = decimal.ToByte(CantidadTarifas);

                BufferSendData[K_posicionCantidadDatos] = (byte)(CantidadDatos);

                BufferSendData[K_offsetDatos + CantidadDatos] = decimal.ToByte(CRC1);
                BufferSendData[K_offsetDatos + CantidadDatos + 1] = decimal.ToByte(CRC2);


                ClearBufferRecievedDataGlobal();

                //close_serial_port();
                //dispose_serial_port();

                //ESCRIBIR EN EL PUERTO COM DE LA ALCANCIA
                Task.WaitAll(new Task[] { Task.Delay(100) });
                open_serial_port();
                puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);

                //PRUEBAS GIL
                //solicitar_status_alcancia_commando_05();
                //solicitar_total_cobrado_ultima_venta_commando_02();

                Task.WaitAll(new Task[] { Task.Delay(200) });
                puertoSerie1.Read(RecievedDataGlobal, 0, 32);

                if (RecievedDataGlobal[5] == 0)
                {
                    // Entrara aqui solo si la respuesta a la asignacion de tarifa es: 00
                    // Comando recibido correctamente.

                    //ClearBufferRecievedDataGlobal();

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

        #region ENTITY DB LOCAL
        //private UInt32 ObtenerUltimoFolioInsertado()
        //{
        //    //AQUI SE OBTENDRA EL ULTIMO FOLIO DE LA BASE DE DATOS LOCAL
        //    ServiceBoletosTarifaFija sbtf = new ServiceBoletosTarifaFija();
        //    UInt32 ultimo_folio = sbtf.getLastEntity();
        //    return ultimo_folio;
        //}
        private int getCantidadPersonasPerfil()
        {
            ServiceConfigVarios scv = new ServiceConfigVarios();
            config_varios cv_cantidad = scv.getEntityByClave("CANTIDAD_PERSONAS_PERFIL");

            return Convert.ToInt32(cv_cantidad.valor);
        }
        private List<string> getCatalogoPerfiles()
        {
            ServicePerfiles serv = new ServicePerfiles();
            List<ct_perfiles> list = serv.getEntities();
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                list2.Add(list[i].nombre);
            }
            return list2;

        }
        private List<string> getCatalogoLugares_PorRuta()
        {
            ServiceLugares serv = new ServiceLugares();
            List<ct_lugares> list_l = serv.getEntities();

            ServiceLugarRuta slr = new ServiceLugarRuta();
            List<sy_lugar_ruta> list_lr = slr.getEntityByFkRuta(asign_activa.fkRuta);

            List<string> list2 = new List<string>();

            foreach (var item_lr in list_lr)
            {
                foreach (var item_l in list_l)
                {
                    if (item_lr.fkLugar == item_l.pkLugar)
                    {
                        list2.Add(item_l.nombre);
                    }
                }

            }
            return list2;
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
        private sy_asignaciones ObtenerAsignacionActivaAutomatica()
        {
            string day = DateTime.Now.ToString("yyyy-MM-dd");
            int hour = Convert.ToInt32((DateTime.Now.ToString("HH") + DateTime.Now.ToString("mm")).ToString());

            ServiceAsignaciones serv_asign = new ServiceAsignaciones();
            sy_asignaciones asign_activa = serv_asign.getEntityByDayAndHour(day, hour);
            return asign_activa;
        }

        private void InsertarBoleto(uint total_cobrado, uint total_pagado)
        {
            try
            {
                //obtener el modo de la aplicacion
                ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
                config_varios cv_modo = serv_config_varios.getEntityByClave("MODO");
                string MODO_APP = cv_modo.valor;

                ServiceTarifas serv_tarifas = new ServiceTarifas();

                List<sy_boletos_detalle> list_boletos_detalle = new List<sy_boletos_detalle>();
                Int64 pkLastBoletoInserted = 0;

                decimal total = 0;
                string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (lugOri != "" && lugDes != "")
                {
                    //obtener los fk de lugar origen y destino
                    ServiceLugares serv_lug = new ServiceLugares();
                    ct_lugares fkLugarOrigen = serv_lug.getEntityByNombre(lugOri);
                    ct_lugares fkLugarDestino = serv_lug.getEntityByNombre(lugDes);

                    // Validacion si hay por lo menos un perfil y cantidad para insertar el boleto
                    if ((perfil1 != "" && cant1 != 0) || (perfil2 != "" && cant2 != 0) ||
                        (perfil3 != "" && cant3 != 0) || (perfil4 != "" && cant4 != 0) ||
                        (perfil5 != "" && cant5 != 0) || (perfil6 != "" && cant6 != 0)) {

                        //Obtener el Folio siguiente para el boleto a insertar
                        string siguienteFolioAInsertar = ObtenerSiguienteFolioAInsertarEnDbLocal();

                        //Crear los objetos de boletos y detalle.
                        ServiceBoletos serv_boletos = new ServiceBoletos();
                        sy_boletos sb = new sy_boletos();
                        sb.pkBoleto = 0;
                        sb.pkBoletoTISA = null;
                        sb.fkAsignacion = asign_activa.pkAsignacion;
                        sb.fkLugarOrigen = fkLugarOrigen.pkLugar;
                        sb.fkLugarDestino = fkLugarDestino.pkLugar;
                        sb.fkStatus = 1;
                        sb.folio = siguienteFolioAInsertar;
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
                        serv_boletos.addEntity(sb);

                        // Obtener ultimo boleto insertado
                        sy_boletos sy_bol = serv_boletos.getEntityLast();
                        pkLastBoletoInserted = sy_bol.pkBoleto;
                    }

                    ServiceBoletosDetalles serv_boletos_detalle = new ServiceBoletosDetalles();

                    for (int i = 1; i <= 6; i++)
                    {
                        sy_boletos_detalle sbd = new sy_boletos_detalle();
                        sbd.fkBoleto = pkLastBoletoInserted;
                        sbd.fkStatus = 1;
                        sbd.enviado = 0;
                        sbd.confirmadoTISA = 0;
                        sbd.modo = MODO_APP;
                        sbd.created_at = fecha_actual;


                        int num_perfil = i;
                        decimal tarifa_perfil_cantidad = 0;

                        switch (num_perfil)
                        {
                            case 1:
                                if (perfil1 != "" && cant1 != 0)
                                {
                                    //obtener los fk de perfil
                                    ServicePerfiles serv_perf = new ServicePerfiles();
                                    ct_perfiles fkPerfil1 = serv_perf.getEntityByNombre(perfil1);

                                    //sumar monto a tarifa a cobrar
                                    sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil1.pkPerfil);
                                    tarifa_perfil_cantidad = st.monto * cant1;

                                    //construccion de la parte correspondiente para el objeto sy_boleto_detalle
                                    sbd.fkPerfil = fkPerfil1.pkPerfil;
                                    sbd.fkTarifa = st.pkTarifa;
                                    sbd.cantidad = cant1;
                                    sbd.subtotal = tarifa_perfil_cantidad;

                                    total += tarifa_perfil_cantidad;
                                }

                                break;
                            case 2:
                                if (perfil2 != "" && cant2 != 0)
                                {
                                    //obtener los fk de perfil
                                    ServicePerfiles serv_perf = new ServicePerfiles();
                                    ct_perfiles fkPerfil2 = serv_perf.getEntityByNombre(perfil2);

                                    //sumar monto a tarifa a cobrar
                                    sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil2.pkPerfil);
                                    tarifa_perfil_cantidad = st.monto * cant2;

                                    //construccion de la parte correspondiente para el objeto sy_boleto_detalle
                                    sbd.fkPerfil = fkPerfil2.pkPerfil;
                                    sbd.fkTarifa = st.pkTarifa;
                                    sbd.cantidad = cant2;
                                    sbd.subtotal = tarifa_perfil_cantidad;

                                    total += tarifa_perfil_cantidad;
                                }
                                break;
                            case 3:
                                if (perfil3 != "" && cant3 != 0)
                                {
                                    //obtener los fk de perfil
                                    ServicePerfiles serv_perf = new ServicePerfiles();
                                    ct_perfiles fkPerfil3 = serv_perf.getEntityByNombre(perfil3);

                                    //sumar monto a tarifa a cobrar
                                    sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil3.pkPerfil);
                                    tarifa_perfil_cantidad = st.monto * cant3;

                                    //construccion de la parte correspondiente para el objeto sy_boleto_detalle
                                    sbd.fkPerfil = fkPerfil3.pkPerfil;
                                    sbd.fkTarifa = st.pkTarifa;
                                    sbd.cantidad = cant3;
                                    sbd.subtotal = tarifa_perfil_cantidad;

                                    total += tarifa_perfil_cantidad;
                                }
                                break;
                            case 4:
                                if (perfil4 != "" && cant4 != 0)
                                {
                                    //obtener los fk de perfil
                                    ServicePerfiles serv_perf = new ServicePerfiles();
                                    ct_perfiles fkPerfil4 = serv_perf.getEntityByNombre(perfil4);

                                    //sumar monto a tarifa a cobrar
                                    sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil4.pkPerfil);
                                    tarifa_perfil_cantidad = st.monto * cant4;

                                    //construccion de la parte correspondiente para el objeto sy_boleto_detalle
                                    sbd.fkPerfil = fkPerfil4.pkPerfil;
                                    sbd.fkTarifa = st.pkTarifa;
                                    sbd.cantidad = cant4;
                                    sbd.subtotal = tarifa_perfil_cantidad;

                                    total += tarifa_perfil_cantidad;
                                }
                                break;
                            case 5:
                                if (perfil5 != "" && cant5 != 0)
                                {
                                    //obtener los fk de perfil
                                    ServicePerfiles serv_perf = new ServicePerfiles();
                                    ct_perfiles fkPerfil5 = serv_perf.getEntityByNombre(perfil5);

                                    //sumar monto a tarifa a cobrar
                                    sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil5.pkPerfil);
                                    tarifa_perfil_cantidad = st.monto * cant5;

                                    //construccion de la parte correspondiente para el objeto sy_boleto_detalle
                                    sbd.fkPerfil = fkPerfil5.pkPerfil;
                                    sbd.fkTarifa = st.pkTarifa;
                                    sbd.cantidad = cant5;
                                    sbd.subtotal = tarifa_perfil_cantidad;

                                    total += tarifa_perfil_cantidad;
                                }
                                break;
                            case 6:
                                if (perfil6 != "" && cant6 != 0)
                                {
                                    //obtener los fk de perfil
                                    ServicePerfiles serv_perf = new ServicePerfiles();
                                    ct_perfiles fkPerfil6 = serv_perf.getEntityByNombre(perfil6);

                                    //sumar monto a tarifa a cobrar
                                    sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil6.pkPerfil);
                                    tarifa_perfil_cantidad = st.monto * cant6;

                                    //construccion de la parte correspondiente para el objeto sy_boleto_detalle
                                    sbd.fkPerfil = fkPerfil6.pkPerfil;
                                    sbd.fkTarifa = st.pkTarifa;
                                    sbd.cantidad = cant6;
                                    sbd.subtotal = tarifa_perfil_cantidad;

                                    total += tarifa_perfil_cantidad;
                                }
                                break;
                        }

                        list_boletos_detalle.Add(sbd);


                    }

                    // Actualizar el campo total del boleto
                    ServiceBoletos serv_boletos_ = new ServiceBoletos();
                    sy_boletos sb_ = serv_boletos_.getEntity(pkLastBoletoInserted);
                    sb_.totalCobrado = total_cobrado; // total
                    sb_.totalPagado = total_pagado;
                    serv_boletos_.updEntity(sb_);

                }

                InsertarBoletoDetalle(pkLastBoletoInserted, list_boletos_detalle);

                list_boletos_detalle.Clear();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

            ServiceBoletos serv_boletos = new ServiceBoletos();
            sy_boletos last_boleto = serv_boletos.getEntityLast();
            siguienteFolioAInsertar =
                prefijo_folio_consola
                + "_"
                + unidad.pkUnidad.ToString("D6")
                + "_"
                + (last_boleto != null ? (last_boleto.pkBoleto + 1).ToString("D6") : 1.ToString("D6"));

            return siguienteFolioAInsertar;
        }
        private void InsertarBoletoDetalle(Int64 pkBoleto, List<sy_boletos_detalle> list_sbd)
        {
            ServiceBoletosDetalles serv_boletos_detalle = new ServiceBoletosDetalles();

            foreach (var it in list_sbd)
            {
                if (it.fkPerfil != null && it.fkPerfil != 0 && it.fkTarifa != null && it.fkTarifa != 0)
                {
                    sy_boletos_detalle sbd = new sy_boletos_detalle();
                    sbd.fkBoleto = pkBoleto;
                    sbd.fkPerfil = it.fkPerfil;
                    sbd.fkTarifa = it.fkTarifa;
                    sbd.fkStatus = it.fkStatus;
                    sbd.cantidad = it.cantidad;
                    sbd.subtotal = it.subtotal;
                    sbd.enviado = it.enviado;
                    sbd.confirmadoTISA = it.confirmadoTISA;
                    sbd.modo = it.modo;
                    sbd.created_at = it.created_at;
                    sbd.updated_at = it.updated_at;

                    serv_boletos_detalle.addEntity(sbd);
                }
            }
        }

        #endregion

        #region METODOS PROPIOS
        private void LlenaComboLugarOrigen()
        {
            List<string> list = getCatalogoLugares_PorRuta();
            foreach (var imp in list)
            {
                cmbLugarOrigen.Items.Add(imp);
            }
        }
        private void LlenaComboLugarDestino()
        {
            List<string> list = getCatalogoLugares_PorRuta();
            foreach (var imp in list)
            {
                cmbLugarDestino.Items.Add(imp);
            }
        }
        private void LlenaCombosPerfiles()
        {
            List<string> list = getCatalogoPerfiles();
            foreach (var imp in list)
            {
                cmbPerfilUno.Items.Add(imp);
                cmbPerfilDos.Items.Add(imp);
                cmbPerfilTres.Items.Add(imp);
                cmbPerfilCuatro.Items.Add(imp);
                cmbPerfilCinco.Items.Add(imp);
                cmbPerfilSeis.Items.Add(imp);
            }
        }
        private void LlenaCombosPerfilesCantidad()
        {
            int cant = getCantidadPersonasPerfil();
            for (int i = 1; i <= cant; i++)
            {
                cmbCantUno.Items.Add(i);
                cmbCantDos.Items.Add(i);
                cmbCantTres.Items.Add(i);
                cmbCantCuatro.Items.Add(i);
                cmbCantCinco.Items.Add(i);
                cmbCantSeis.Items.Add(i);
            }
        }
        private string ObtenerTarifa(int num_perfil)
        {
            string tarifa_perfil_cantidad = "";
            try
            {
                ServiceTarifas serv_tarifas = new ServiceTarifas();

                string lugOri = "";
                string lugDes = "";

                string perfil1 = "";
                string perfil2 = "";
                string perfil3 = "";
                string perfil4 = "";
                string perfil5 = "";
                string perfil6 = "";


                if (cmbLugarOrigen != null && cmbLugarOrigen.Text != "")
                {
                    lugOri = cmbLugarOrigen.SelectedValue.ToString().Trim();
                }
                if (cmbLugarDestino != null && cmbLugarDestino.Text != "")
                {
                    lugDes = cmbLugarDestino.SelectedValue.ToString().Trim();
                }

                if (lugOri != "" && lugDes != "")
                {
                    //obtener los fk de lugar origen y destino
                    ServiceLugares serv_lug = new ServiceLugares();
                    ct_lugares fkLugarOrigen = serv_lug.getEntityByNombre(lugOri);
                    ct_lugares fkLugarDestino = serv_lug.getEntityByNombre(lugDes);

                    switch (num_perfil)
                    {
                        case 1:
                            if (cmbPerfilUno != null && cmbPerfilUno.Text != "")
                            {
                                perfil1 = cmbPerfilUno.SelectedValue.ToString().Trim();
                            }
                            if (perfil1 != "")
                            {
                                //obtener los fk de perfil
                                ServicePerfiles serv_perf = new ServicePerfiles();
                                ct_perfiles fkPerfil1 = serv_perf.getEntityByNombre(perfil1);

                                //sumar monto a tarifa a cobrar
                                sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil1.pkPerfil);
                                tarifa_perfil_cantidad = Convert.ToDouble(st.monto).ToString();
                            }

                            break;
                        case 2:
                            if (cmbPerfilDos != null && cmbPerfilDos.Text != "")
                            {
                                perfil2 = cmbPerfilDos.SelectedValue.ToString().Trim();
                            }
                            if (perfil2 != "")
                            {
                                //obtener los fk de perfil
                                ServicePerfiles serv_perf = new ServicePerfiles();
                                ct_perfiles fkPerfil2 = serv_perf.getEntityByNombre(perfil2);

                                //sumar monto a tarifa a cobrar
                                sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil2.pkPerfil);
                                tarifa_perfil_cantidad = Convert.ToDouble(st.monto).ToString();
                            }
                            break;
                        case 3:
                            if (cmbPerfilTres != null && cmbPerfilTres.Text != "")
                            {
                                perfil3 = cmbPerfilTres.SelectedValue.ToString().Trim();
                            }
                            if (perfil3 != "")
                            {
                                //obtener los fk de perfil
                                ServicePerfiles serv_perf = new ServicePerfiles();
                                ct_perfiles fkPerfil3 = serv_perf.getEntityByNombre(perfil3);

                                //sumar monto a tarifa a cobrar
                                sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil3.pkPerfil);
                                tarifa_perfil_cantidad = Convert.ToDouble(st.monto).ToString();
                            }
                            break;
                        case 4:
                            if (cmbPerfilCuatro != null && cmbPerfilCuatro.Text != "")
                            {
                                perfil4 = cmbPerfilCuatro.SelectedValue.ToString().Trim();
                            }
                            if (perfil4 != "")
                            {
                                //obtener los fk de perfil
                                ServicePerfiles serv_perf = new ServicePerfiles();
                                ct_perfiles fkPerfil4 = serv_perf.getEntityByNombre(perfil4);

                                //sumar monto a tarifa a cobrar
                                sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil4.pkPerfil);
                                tarifa_perfil_cantidad = Convert.ToDouble(st.monto).ToString();
                            }
                            break;
                        case 5:
                            if (cmbPerfilCinco != null && cmbPerfilCinco.Text != "")
                            {
                                perfil5 = cmbPerfilCinco.SelectedValue.ToString().Trim();
                            }
                            if (perfil5 != "")
                            {
                                //obtener los fk de perfil
                                ServicePerfiles serv_perf = new ServicePerfiles();
                                ct_perfiles fkPerfil5 = serv_perf.getEntityByNombre(perfil5);

                                //sumar monto a tarifa a cobrar
                                sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil5.pkPerfil);
                                tarifa_perfil_cantidad = Convert.ToDouble(st.monto).ToString();
                            }
                            break;
                        case 6:
                            if (cmbPerfilSeis != null && cmbPerfilSeis.Text != "")
                            {
                                perfil6 = cmbPerfilSeis.SelectedValue.ToString().Trim();
                            }
                            if (perfil6 != "")
                            {
                                //obtener los fk de perfil
                                ServicePerfiles serv_perf = new ServicePerfiles();
                                ct_perfiles fkPerfil6 = serv_perf.getEntityByNombre(perfil6);

                                //sumar monto a tarifa a cobrar
                                sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil6.pkPerfil);
                                tarifa_perfil_cantidad = Convert.ToDouble(st.monto).ToString();
                            }
                            break;
                    }
                }

                tarifa_perfil_cantidad += ".00"; ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return tarifa_perfil_cantidad;
        }
        private void recalcular_tarifa(object sender)
        {
            try
            {
                //string nombreBoton = "";

                string nombreCombo = "";
                string textSelected = "";

                //if ((Button)sender != null) 
                //{
                //    nombreBoton = ((ComboBox)sender).Name;
                //}
                //else if((ComboBox)sender != null)
                //{
                    var currentSelectedIndex = ((ComboBox)sender).SelectedIndex;
                    textSelected = ((ComboBox)sender).Items[currentSelectedIndex].ToString();
                    nombreCombo = ((ComboBox)sender).Name;

                //}


                switch (nombreCombo)
                {
                    case "cmbLugarOrigen":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            lugOri = ((ComboBox)sender).SelectedValue.ToString().Trim();
                        }
                        break;
                    case "cmbLugarDestino":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            lugDes = ((ComboBox)sender).SelectedValue.ToString().Trim();
                        }
                        break;
                    case "cmbPerfilUno":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            perfil1 = ((ComboBox)sender).SelectedValue.ToString().Trim();
                        }
                        break;
                    case "cmbPerfilDos":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            perfil2 = ((ComboBox)sender).SelectedValue.ToString().Trim();
                        }
                        break;
                    case "cmbPerfilTres":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            perfil3 = ((ComboBox)sender).SelectedValue.ToString().Trim();
                        }
                        break;
                    case "cmbPerfilCuatro":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            perfil4 = ((ComboBox)sender).SelectedValue.ToString().Trim();
                        }
                        break;
                    case "cmbPerfilCinco":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            perfil5 = ((ComboBox)sender).SelectedValue.ToString().Trim();
                        }
                        break;
                    case "cmbPerfilSeis":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            perfil6 = ((ComboBox)sender).SelectedValue.ToString().Trim();
                        }
                        break;
                    case "cmbCantUno":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            cant1 = Convert.ToInt32(((ComboBox)sender).SelectedValue);
                        }
                        break;
                    case "cmbCantDos":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            cant2 = Convert.ToInt32(((ComboBox)sender).SelectedValue);
                        }
                        break;
                    case "cmbCantTres":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            cant3 = Convert.ToInt32(((ComboBox)sender).SelectedValue);
                        }
                        break;
                    case "cmbCantCuatro":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            cant4 = Convert.ToInt32(((ComboBox)sender).SelectedValue);
                        }
                        break;
                    case "cmbCantCinco":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            cant5 = Convert.ToInt32(((ComboBox)sender).SelectedValue);
                        }
                        break;
                    case "cmbCantSeis":
                        if (((ComboBox)sender) != null && textSelected != "")
                        {
                            cant6 = Convert.ToInt32(((ComboBox)sender).SelectedValue);
                        }
                        break;
                }

                ServiceTarifas serv_tarifas = new ServiceTarifas();

                double tarifa_a_cobrar = 0;

                if (lugOri != "" && lugDes != "")
                {
                    //obtener los fk de lugar origen y destino
                    ServiceLugares serv_lug = new ServiceLugares();
                    ct_lugares fkLugarOrigen = serv_lug.getEntityByNombre(lugOri);
                    ct_lugares fkLugarDestino = serv_lug.getEntityByNombre(lugDes);

                    if (perfil1 != "" && cant1 != 0)
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil1 = serv_perf.getEntityByNombre(perfil1);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil1.pkPerfil);
                        if (st != null)
                        {
                            tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant1);
                        }
                        else
                        {
                            MessageBox.Show("No hay una Tarifa Asignada en sistema para la combinacion en posicion 1 en perfil y Lugares Origen Destino seleccionados actualmente", "ATENCION !!!");
                        }
                    }
                    if (perfil2 != "" && cant2 != 0)
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil2 = serv_perf.getEntityByNombre(perfil2);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil2.pkPerfil);
                        if (st != null)
                        {
                            tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant2);
                        }
                        else
                        {
                            MessageBox.Show("No hay una Tarifa Asignada en sistema para la combinacion en posicion 2 en perfil y Lugares Origen Destino seleccionados actualmente", "ATENCION !!!");
                        }
                    }
                    if (perfil3 != "" && cant3 != 0)
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil3 = serv_perf.getEntityByNombre(perfil3);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil3.pkPerfil);
                        if (st != null)
                        {
                            tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant3);
                        }
                        else
                        {
                            MessageBox.Show("No hay una Tarifa Asignada en sistema para la combinacion en posicion 3 en perfil y Lugares Origen Destino seleccionados actualmente", "ATENCION !!!");
                        }
                    }
                    if (perfil4 != "" && cant4 != 0)
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil4 = serv_perf.getEntityByNombre(perfil4);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil4.pkPerfil);
                        if (st != null)
                        {
                            tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant4);
                        }
                        else
                        {
                            MessageBox.Show("No hay una Tarifa Asignada en sistema para la combinacion en posicion 4 en perfil y Lugares Origen Destino seleccionados actualmente", "ATENCION !!!");
                        }
                    }
                    if (perfil5 != "" && cant5 != 0)
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil5 = serv_perf.getEntityByNombre(perfil5);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil5.pkPerfil);
                        if (st != null)
                        {
                            tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant5);
                        }
                        else
                        {
                            MessageBox.Show("No hay una Tarifa Asignada en sistema para la combinacion en posicion 5 en perfil y Lugares Origen Destino seleccionados actualmente", "ATENCION !!!");
                        }
                    }
                    if (perfil6 != "" && cant6 != 0)
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil6 = serv_perf.getEntityByNombre(perfil6);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil6.pkPerfil);
                        if (st != null)
                        {
                            tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant6);
                        }
                        else
                        {
                            MessageBox.Show("No hay una Tarifa Asignada en sistema para la combinacion en posicion 6 en perfil y Lugares Origen Destino seleccionados actualmente", "ATENCION !!!");
                        }
                    }
                }

                lblMontoCalculado.Text = tarifa_a_cobrar.ToString().Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private uint obtenerMontoIngresadoActual(byte[] RecievedDataGlobal)
        {
            uint monto_actual_ingresado = 0;
            monto_actual_ingresado = VarByteToUInteger32_comand_05(RecievedDataGlobal);
            return monto_actual_ingresado;
        }

        #endregion

        #region EVENTOS SELECTION CHANGED EN COMBOBOXES
        private void cmbPerfilUno_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var currentSelectedIndex = cmbPerfilUno.SelectedIndex;
            //string textSelected = ((ComboBox)sender).Items[currentSelectedIndex].ToString();
            //string nombreCombo = ((ComboBox)sender).Name;

            recalcular_tarifa(sender);
        }
        private void cmbPerfilDos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbPerfilTres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbPerfilCuatro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbPerfilCinco_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbPerfilSeis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }

        private void cmbCantUno_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbCantDos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbCantTres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbCantCuatro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbCantCinco_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbCantSeis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }

        private void cmbLugarOrigen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        private void cmbLugarDestino_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa(sender);
        }
        #endregion

        #region METODOS - CONTROLES INTERFAS GRAFICA
        private void bloqueaCamposMientrasIngresaMonedas()
        {
            cmbLugarOrigen.IsEnabled = false;
            cmbLugarDestino.IsEnabled = false;

            cmbPerfilUno.IsEnabled = false;
            cmbPerfilDos.IsEnabled = false;
            cmbPerfilTres.IsEnabled = false;
            cmbPerfilCuatro.IsEnabled = false;
            cmbPerfilCinco.IsEnabled = false;
            cmbPerfilSeis.IsEnabled = false;

            cmbCantUno.IsEnabled = false;
            cmbCantDos.IsEnabled = false;
            cmbCantTres.IsEnabled = false;
            cmbCantCuatro.IsEnabled = false;
            cmbCantCinco.IsEnabled = false;
            cmbCantSeis.IsEnabled = false;

            btnCobrar.IsEnabled = false;
            btnCancelarVenta.IsEnabled = true;

        }
        private void desbloqueaCampos()
        {
            cmbLugarOrigen.IsEnabled = true;
            cmbLugarDestino.IsEnabled = true;

            cmbPerfilUno.IsEnabled = true;
            cmbPerfilDos.IsEnabled = true;
            cmbPerfilTres.IsEnabled = true;
            cmbPerfilCuatro.IsEnabled = true;
            cmbPerfilCinco.IsEnabled = true;
            cmbPerfilSeis.IsEnabled = true;

            cmbCantUno.IsEnabled = true;
            cmbCantDos.IsEnabled = true;
            cmbCantTres.IsEnabled = true;
            cmbCantCuatro.IsEnabled = true;
            cmbCantCinco.IsEnabled = true;
            cmbCantSeis.IsEnabled = true;

            btnCobrar.IsEnabled = true;
            btnCancelarVenta.IsEnabled = false;
        }
        private void popupGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            ocultarPopupOk();
        }
        private void popupGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ocultarPopupOk();
            LimpiarCamposDespuesDeVentaBoleto();

        }
        private void popupGrid_TouchDown(object sender, TouchEventArgs e)
        {
            ocultarPopupOk();
            LimpiarCamposDespuesDeVentaBoleto();
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
                btnCobrar.IsEnabled = false;
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

                close_serial_port();
                //dispose_serial_port();

                open_serial_port(); //EMD 2024-05-06

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
                btnCobrar.IsEnabled = false;
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
        private void dispose_serial_port()
        {
            if (puertoSerie1.IsOpen == false)
            {
                puertoSerie1.Dispose();
            }
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


                    // solicitar_status_alcancia_commando_05(); // EMD 2024-05-06
                    ejecutar_commando_07_status_desgloce_depositado_monedas_billetes();

                    //string RecievedData;
                    timerWait.IsEnabled = true;

                    // delay para esperar 8 de respuesta
                    // 30 funciona en debug de visual studio
                    // 100 para app instalada en raspberry
                    //Task.WaitAll(new Task[] { Task.Delay(30) });
                    Task.WaitAll(new Task[] { Task.Delay(50) });

                    //RecievedData = puertoSerie1.ReadExisting();

                    //puertoSerie1.Read(RecievedDataGlobal, 0, 32);
                    puertoSerie1.Read(RecievedDataGlobal, 0, 50);

                    int status = Convert.ToInt32(RecievedDataGlobal[5]);
                    ESTATUS = status.ToString();

                    //uint monto_ingresado = obtenerMontoIngresadoActual(RecievedDataGlobal);
                    decimal monto_ingresado = obtenerMontoIngresadoActual(RecievedDataGlobal);
                    decimal num_cien = 100;
                    MONTO_INGRESADO = (monto_ingresado / num_cien).ToString();

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
                    uint total_cobrado = 0;
                    uint total_pagado = 0;

                    //Task.WaitAll(new Task[] { Task.Delay(100) });
                    //puertoSerie1.Read(RecievedDataGlobal, 0, 50);

                    //if (RecievedDataGlobal[5] == 0) // si el comando de tarifa se completo correctamente.
                    //{

                        // TODO: ejecutar_commando_02_ultima_venta para obtener los totales cobrado y pagado
                        ejecutar_commando_02_ultima_venta();

                    ClearBufferRecievedDataGlobal(); //EMD 2024-06-05

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
        #endregion

        #region METODOS - ALCANCIA
        private void solicitar_status_alcancia()
        {
            //timerPoll.Enabled = false;
            string BufferSendData = "\x0001" + "\x0002" + "\x0001" + "\x0001" + "\x0000" + "\x0041" + "\x0042";
            puertoSerie1.Write(BufferSendData);
            //timerWait.Enabled = true;
        }
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

            //close_serial_port();
            //dispose_serial_port();

            open_serial_port();
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

        private void btnCancelarVenta_Click(object sender, RoutedEventArgs e)
        {
            ejecutar_commando_06_cancelar_venta();

            ClearBufferRecievedDataGlobal();

            Task.WaitAll(new Task[] { Task.Delay(200) });
            puertoSerie1.Read(RecievedDataGlobal, 0, 50);

            if (RecievedDataGlobal != null)
            {
                insert_boleto_cancelado_en_db_local(RecievedDataGlobal);
                Task.WaitAll(new Task[] { Task.Delay(100) });

                desbloqueaCampos();

                txtMensajePopup.Text = "VENTA CANCELADA";
                mostrarPopupOk();

                //actualizaTxtStatus();
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

            //close_serial_port();
            //dispose_serial_port();

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);
        }

        private void agregar_folio_to_buffer_send_data(string folio)
        {
            int n = 5;
            for (int i = 0; i < 17; i++)
            {
                BufferSendData[n++] = (byte)folio[i];
            }
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

            sy_boletos obj_boleto = new sy_boletos();

            obj_boleto.pkBoletoTISA = null;
            obj_boleto.fkAsignacion = asign_activa.pkAsignacion;
            obj_boleto.fkLugarOrigen = 0;
            obj_boleto.fkLugarDestino = 0;
            obj_boleto.fkStatus = 2; // 2 = Status Cancelado.
            obj_boleto.enviado = 0;
            obj_boleto.confirmadoTISA = 0;
            obj_boleto.modo = MODO_APP;
            obj_boleto.created_at = fecha_actual;
            obj_boleto.updated_at = null;
            obj_boleto.deleted_at = null;

            obj_boleto.folio = GetFolioBoletoString(RecievedDataGlobal_local);

            string varStrFechaHora = StringToBCD(22);
            string fecha = varStrFechaHora.Substring(4, 2) + '/' + varStrFechaHora.Substring(2, 2) + '/' + "20" + varStrFechaHora.Substring(0, 2);
            string hora = varStrFechaHora.Substring(6, 2) + ':' + varStrFechaHora.Substring(8, 2) + ':' + varStrFechaHora.Substring(10, 2);

            obj_boleto.fechaHoraCancelacion = fecha + " " + hora;
            
            uint varTotalCobrado = VarByteToUInteger32(31);
            uint varTotalPagado = VarByteToUInteger32(35);

            string strTotalCobrado = varTotalCobrado.ToString();
            string strTotalPagado = varTotalPagado.ToString();

            obj_boleto.totalCobrado = strTotalCobrado != null && strTotalCobrado != "" ? (varTotalCobrado / 100) : 0;
            obj_boleto.totalPagado = strTotalPagado != null && strTotalPagado != "" ? (varTotalPagado / 100) : 0;

            ServiceBoletos serv_boletos = new ServiceBoletos();
            serv_boletos.addEntity(obj_boleto);
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
        private uint VarByteToUInteger32_comand_07(byte[] RecievedDataGlobal)
        {
            uint varCantTemp = 0;
            for (int i = 9; i >= 6; i--) // para iterar por las posiciones de ReceivedData, donde sabemos viene el monto actual ingresado.
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            return varCantTemp;
        }

        private uint VarByteToUInteger32_comand_07_tipo_moneda_billete(byte[] RecievedDataGlobal, int posicion)
        {
            //for (int i = 19; i >= 18; i--) aqui inicia el desgloce de cantidad de piezas de monedas y billetes ingresados.
            int x = posicion;

            uint varCantTemp = 0;
            for (int i = x; i >= (x-1); i--) // para iterar por las posiciones de ReceivedData, donde sabemos viene el monto actual ingresado.
            {
                varCantTemp <<= 8;
                varCantTemp |= RecievedDataGlobal[i];
            }
            varCantTemp <<= 16; // se hace el corrimiento de los 16 bits que pone ceros implicitamente, para completar la variable entera de 32 bits.
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

        #region METODOS - DELEGADOS
        private void actualizaTxtStatus()
        {
            txtStatus.Text = ESTATUS;
        }
        private void actualizalblMontoIngresado()
        {
            lblMontoIngresado.Text = MONTO_INGRESADO;
        }

        private void LimpiarCamposDespuesDeVentaBoleto()
        {
            try
            {
                //CobroMultitarifaV1_Load(null,null);

                //cmbLugarOrigen.Items.Clear();
                //cmbLugarDestino.Items.Clear();

                //cmbPerfilUno.Items.Clear();
                //cmbPerfilDos.Items.Clear();
                //cmbPerfilTres.Items.Clear();
                //cmbPerfilCuatro.Items.Clear();
                //cmbPerfilCinco.Items.Clear();
                //cmbPerfilSeis.Items.Clear();

                //cmbCantUno.Items.Clear();
                //cmbCantDos.Items.Clear();
                //cmbCantTres.Items.Clear();
                //cmbCantCuatro.Items.Clear();
                //cmbCantCinco.Items.Clear();
                //cmbCantSeis.Items.Clear();

                lblMontoCalculado.Text = "";
                lblMontoIngresado.Text = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private void LimpiarCamposDespuesDeVentaBoletoV3()
        {
            try
            {
                cmbLugarOrigen.SelectedIndex = -1;
                cmbLugarDestino.SelectedIndex = -1;

                cmbPerfilUno.SelectedIndex = -1;
                cmbPerfilDos.SelectedIndex = -1;
                cmbPerfilTres.SelectedIndex = -1;
                cmbPerfilCuatro.SelectedIndex = -1;
                cmbPerfilCinco.SelectedIndex = -1;
                cmbPerfilSeis.SelectedIndex = -1;

                cmbCantUno.SelectedIndex = -1;
                cmbCantDos.SelectedIndex = -1;
                cmbCantTres.SelectedIndex = -1;
                cmbCantCuatro.SelectedIndex = -1;
                cmbCantCinco.SelectedIndex = -1;
                cmbCantSeis.SelectedIndex = -1;

                lblMontoCalculado.Text = "";
                lblMontoIngresado.Text = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private void LimpiarCamposDespuesDeVentaBoletoV2()
        {
            try
            {
                

                cmbLugarOrigen.Text = "";
                cmbLugarDestino.Text = "";

                cmbPerfilUno.Text = "";
                cmbPerfilDos.Text = "";
                cmbPerfilTres.Text = "";
                cmbPerfilCuatro.Text = "";
                cmbPerfilCinco.Text = "";
                cmbPerfilSeis.Text = "";

                cmbCantUno.Text = "";
                cmbCantDos.Text = "";
                cmbCantTres.Text = "";
                cmbCantCuatro.Text = "";
                cmbCantCinco.Text = "";
                cmbCantSeis.Text = "";

                lblMontoCalculado.Text = "";
                lblMontoIngresado.Text = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        #endregion

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

            //close_serial_port();
            //dispose_serial_port();

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);
        }

        private void ejecutar_commando_07_status_desgloce_depositado_monedas_billetes()
        {
            const byte ByteInicio = 1;
            const byte AddressConsola = 1;
            const byte AddressAlcancia = 2;
            const byte cantidadDatos = 1;
            const byte numeroComando = 7;
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

            ClearBufferRecievedDataGlobal();

            //close_serial_port();
            //dispose_serial_port();

            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, 7);
            //timerWait.Enabled = true;
        }
        private uint getTotalCobradoBoleto()
        {
            uint total_cobrado = 0;

            uint varTotalCobrado = VarByteToUInteger32(31);
            string strTotalCobrado = varTotalCobrado.ToString();
            total_cobrado = strTotalCobrado != null && strTotalCobrado != "" ? (varTotalCobrado / 100) : 0;

            return total_cobrado;
        }
        private uint getTotalPagadoBoleto()
        {
            uint total_pagado = 0;

            uint varTotalPagado = VarByteToUInteger32(35);
            string strTotalPagado = varTotalPagado.ToString();
            total_pagado = strTotalPagado != null && strTotalPagado != "" ? (varTotalPagado / 100) : 0;

            return total_pagado;
        }
    }
}
