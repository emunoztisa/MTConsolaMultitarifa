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
        private string ESTATUS = "0";
        DispatcherTimer timerEvalua = new DispatcherTimer();


        //DEFINICION DE DELEGADOS
        delegate void delegate_actualizaTxtStatus();
        delegate_actualizaTxtStatus delegado_actualiza_txt_status = null;


        public CobroMultitarifaV1()
        {
            InitializeComponent();

            // Para actualizar el campo de txtStatus en la interfas grafica.
            //delegate_actualizaTxtStatus delegado_actualiza_txt_status = new delegate_actualizaTxtStatus(actualizaTxtStatus);
            delegado_actualiza_txt_status = new delegate_actualizaTxtStatus(actualizaTxtStatus);
            delegado_actualiza_txt_status();

            configura_puerto_serial(); // UTILIZA ENTITY FRAMEWORK CON CONEXION A database.mdf

            // REGISTRAR EVENTO DEL LOS COMBOS
            //cmbPerfilUno.SelectionChanged += new SelectionChangedEventHandler(cmbPerfilUno_SelectionChanged);
            //cmbPerfilDos.SelectionChanged += new SelectionChangedEventHandler(cmbPerfilDos_SelectionChanged);

            //cmbCantUno.SelectionChanged += new SelectionChangedEventHandler(cmbCantUno_SelectionChanged);
            //cmbCantDos.SelectionChanged += new SelectionChangedEventHandler(cmbCantDos_SelectionChanged);
        }

        #region EVENTOS BOTONES Y VENTANAS
        private void CobroMultitarifaV1_Load(object sender, RoutedEventArgs e)
        {
            asign_activa = ObtenerAsignacionActiva();

            LlenaComboLugarOrigen();
            LlenaComboLugarDestino();
            LlenaCombosPerfiles();
            LlenaCombosPerfilesCantidad();

            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();
        }
        private void CobroMultitarifaV1_Unload(object sender, RoutedEventArgs e)
        {
            close_serial_port();
        }
        private void btnCobrar_Click(object sender, RoutedEventArgs e)
        {
            bloqueaCamposMientrasIngresaMonedas();

            inicializa_timer_wait();
            inicializa_timer_evalua();

            const Int32 K_posicionCantidadTarifas = 11;
            //const decimal CantidadDatos = 17;
            const decimal Comando = 1;
            //const byte CantidadTarifas = 1;
            const decimal CRC1 = 193;
            const decimal CRC2 = 194;

            Int32 CantidadDatos = 0;
            Decimal CantidadTarifas = 0;

            //FOLIO BOLETO
            UInt32 folioVenta = 1; //ObtenerUltimoFolioInsertado(); CONSULTA LA BASE DE DATOS database.mdf
            folioVenta += 1;

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


            //PERFIL y CANT UNO
            string tarifa_1 = ObtenerTarifa(1); // YA OBTIENE LA TARIFA CON EL PERFIL y CANTIDAD
            tarifa_1 = tarifa_1.Replace(".", "");
            UInt32 PrecioTarifa1 = Convert.ToUInt32(tarifa_1);
            int cant1 = Convert.ToInt32(cmbCantUno.Text);
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
            int cant2 = Convert.ToInt32(cmbCantDos.Text);
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
            int cant3 = Convert.ToInt32(cmbCantTres.Text);
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
            int cant4 = Convert.ToInt32(cmbCantCuatro.Text);
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
            int cant5 = Convert.ToInt32(cmbCantCinco.Text);
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
            int cant6 = Convert.ToInt32(cmbCantSeis.Text);
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


            //ESCRIBIR EN EL PUERTO COM DE LA ALCANCIA
            open_serial_port();
            puertoSerie1.Write(BufferSendData, 0, K_offsetDatos + CantidadDatos + 2);


        }
        #endregion

        #region ENTITY DB LOCAL
        private UInt32 ObtenerUltimoFolioInsertado()
        {
            //AQUI SE OBTENDRA EL ULTIMO FOLIO DE LA BASE DE DATOS LOCAL
            ServiceBoletosTarifaFija sbtf = new ServiceBoletosTarifaFija();
            UInt32 ultimo_folio = sbtf.getLastEntity();
            return ultimo_folio;
        }
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

        private void InsertarBoleto()
        {
            ServiceBoletos serv_boletos = new ServiceBoletos();
            sy_boletos sb = new sy_boletos();
            sb.fkAsignacion = asign_activa.pkAsignacion;
            sb.fkLugarOrigen = 0;
            sb.fkLugarDestino = 0;
            sb.fkStatus = 1;
            sb.folio = "";
            sb.total = "";
            sb.created_at = "";
            sb.updated_at = "";

            serv_boletos.addEntity(sb);
        }
        private void InsertarBoletoDetalle(int pkBoleto)
        {
            ServiceBoletosDetalles serv_boletos_detalle = new ServiceBoletosDetalles();
            sy_boletos_detalle sbd = new sy_boletos_detalle();
            sbd.fkBoleto = pkBoleto;
            sbd.fkPerfil = 0;
            sbd.fkTarifa = 0;
            sbd.fkStatus = 1;
            sbd.cantidad = 0;
            sbd.subtotal = "";
            sbd.created_at = "";
            sbd.updated_at = "";

            serv_boletos_detalle.addEntity(sbd);
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
        private void recalcular_tarifa()
        {
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

                string cant1 = "";
                string cant2 = "";
                string cant3 = "";
                string cant4 = "";
                string cant5 = "";
                string cant6 = "";

                if (cmbLugarOrigen != null && cmbLugarOrigen.Text != "")
                {
                    lugOri = cmbLugarOrigen.SelectedValue.ToString().Trim();
                }
                if (cmbLugarDestino != null && cmbLugarDestino.Text != "")
                {
                    lugDes = cmbLugarDestino.SelectedValue.ToString().Trim();
                }

                if (cmbPerfilUno != null && cmbPerfilUno.Text != "")
                {
                    perfil1 = cmbPerfilUno.SelectedValue.ToString().Trim();
                }
                if (cmbPerfilDos != null && cmbPerfilDos.Text != "")
                {
                    perfil2 = cmbPerfilDos.SelectedValue.ToString().Trim();
                }
                if (cmbPerfilTres != null && cmbPerfilTres.Text != "")
                {
                    perfil3 = cmbPerfilTres.SelectedValue.ToString().Trim();
                }
                if (cmbPerfilCuatro != null && cmbPerfilCuatro.Text != "")
                {
                    perfil4 = cmbPerfilCuatro.SelectedValue.ToString().Trim();
                }
                if (cmbPerfilCinco != null && cmbPerfilCinco.Text != "")
                {
                    perfil5 = cmbPerfilCinco.SelectedValue.ToString().Trim();
                }
                if (cmbPerfilSeis != null && cmbPerfilSeis.Text != "")
                {
                    perfil6 = cmbPerfilSeis.SelectedValue.ToString().Trim();
                }

                if (cmbCantUno != null && cmbCantUno.Text != "")
                {
                    cant1 = cmbCantUno.SelectedValue.ToString().Trim();
                }
                if (cmbCantDos != null && cmbCantDos.Text != "")
                {
                    cant2 = cmbCantDos.SelectedValue.ToString().Trim();
                }
                if (cmbCantTres != null && cmbCantTres.Text != "")
                {
                    cant3 = cmbCantTres.SelectedValue.ToString().Trim();
                }
                if (cmbCantCuatro != null && cmbCantCuatro.Text != "")
                {
                    cant4 = cmbCantCuatro.SelectedValue.ToString().Trim();
                }
                if (cmbCantCinco != null && cmbCantCinco.Text != "")
                {
                    cant5 = cmbCantCinco.SelectedValue.ToString().Trim();
                }
                if (cmbCantSeis != null && cmbCantSeis.Text != "")
                {
                    cant6 = cmbCantSeis.SelectedValue.ToString().Trim();
                }

                double tarifa_a_cobrar = 0;

                if (lugOri != "" && lugDes != "")
                {
                    //obtener los fk de lugar origen y destino
                    ServiceLugares serv_lug = new ServiceLugares();
                    ct_lugares fkLugarOrigen = serv_lug.getEntityByNombre(lugOri);
                    ct_lugares fkLugarDestino = serv_lug.getEntityByNombre(lugDes);

                    if (perfil1 != "" && cant1 != "")
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil1 = serv_perf.getEntityByNombre(perfil1);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil1.pkPerfil);
                        tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant1);
                    }
                    if (perfil2 != "" && cant2 != "")
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil2 = serv_perf.getEntityByNombre(perfil2);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil2.pkPerfil);
                        tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant2);
                    }
                    if (perfil3 != "" && cant3 != "")
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil3 = serv_perf.getEntityByNombre(perfil3);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil3.pkPerfil);
                        tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant3);
                    }
                    if (perfil4 != "" && cant4 != "")
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil4 = serv_perf.getEntityByNombre(perfil4);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil4.pkPerfil);
                        tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant4);
                    }
                    if (perfil5 != "" && cant5 != "")
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil5 = serv_perf.getEntityByNombre(perfil5);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil5.pkPerfil);
                        tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant5);
                    }
                    if (perfil6 != "" && cant6 != "")
                    {
                        //obtener los fk de perfil
                        ServicePerfiles serv_perf = new ServicePerfiles();
                        ct_perfiles fkPerfil6 = serv_perf.getEntityByNombre(perfil6);

                        //sumar monto a tarifa a cobrar
                        sy_tarifas st = serv_tarifas.getEntityByLugarOriLugarDesAndPerfil(fkLugarOrigen.pkLugar, fkLugarDestino.pkLugar, fkPerfil6.pkPerfil);
                        tarifa_a_cobrar += Convert.ToDouble(st.monto) * Convert.ToDouble(cant6);
                    }
                }

                lblMontoCalculado.Text = tarifa_a_cobrar.ToString().Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LimpiarCamposDespuesDeVentaBoleto()
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

        #endregion

        #region EVENTOS SELECTION CHANGED EN COMBOBOXES
        private void cmbPerfilUno_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbPerfilDos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbPerfilTres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbPerfilCuatro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbPerfilCinco_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbPerfilSeis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbCantUno_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbCantDos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbCantTres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbCantCuatro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbCantCinco_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbCantSeis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbLugarOrigen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
        }

        private void cmbLugarDestino_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recalcular_tarifa();
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
                    Task.WaitAll(new Task[] { Task.Delay(500) });

                    RecievedData = puertoSerie1.ReadExisting();

                    int status = Convert.ToInt32(RecievedData[5]);
                    ESTATUS = status.ToString();

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
                mostrarPopupOk();

                //actualizaTxtStatus();
                delegado_actualiza_txt_status();

                detiene_timers();

                LimpiarCamposDespuesDeVentaBoleto();
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

        #region METODOS - BUFFERS
        void ClearBufferSendData()
        {
            for (int i = 0; i < BufferSendData.Length; i++)
            {
                BufferSendData[i] = 0;
            }
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

        #region METODOS - DELEGADOS
        private void actualizaTxtStatus()
        {
            txtStatus.Text = ESTATUS;
        }

        #endregion

    }
}
