using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TestMdfEntityFramework.Clases;
using TestMdfEntityFramework.Controllers;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Views;
using System.Device.Location;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;
using System.Linq;

namespace TestMdfEntityFramework
{
    /// <summary>
    /// Lógica de interacción para Principal.xaml
    /// </summary>
    [System.Runtime.InteropServices.Guid("7BFFA413-A96F-422C-83F9-5D466749E0FB")]
    public partial class Principal : Window
    {
        public static string ASIGNACION_ACTIVA = "";
        public static long FK_ASIGNACION_ACTIVA = 0;
        string MODO_APP = "";

        //SERIAL PORT - ALCANCIA
        System.IO.Ports.SerialPort puertoSerie_alcancia = new System.IO.Ports.SerialPort();
        String[] listado_puerto = System.IO.Ports.SerialPort.GetPortNames();

        //SERIAL PORT - CUENTA COCOS
        System.IO.Ports.SerialPort puertoSerie_cuenta_cocos = new System.IO.Ports.SerialPort();

        //SERIAL PORT - GPS
        System.IO.Ports.SerialPort puertoSerie_gps = new System.IO.Ports.SerialPort();



        //BUFFERS SEND AND RECIEVED - ALCANCIA
        byte[] BufferSendData = new byte[80];
        byte[] RecievedDataGlobal = new byte[80];

        //BUFFERS SEND AND RECIEVED - CUENTA COCOS
        byte[] BufferSendData_cuenta_cocos = new byte[80];
        byte[] RecievedDataGlobal_cuenta_cocos = new byte[80];

        //BUFFERS SEND AND RECIEVED - GPS
        byte[] BufferSendData_gps = new byte[80];
        byte[] RecievedDataGlobal_gps = new byte[80];

        //TIMERS
        DispatcherTimer timerReloj = new DispatcherTimer();
        DispatcherTimer timerEvaluaMensajes = new DispatcherTimer();
        DispatcherTimer timerSincroniza = new DispatcherTimer();
        DispatcherTimer timerCuentaCocos = new DispatcherTimer();
        DispatcherTimer timerPosicionGPS = new DispatcherTimer();

        public Principal()
        {
            InitializeComponent();
            btnInicio_Click(null, null);

            cargar_logo_aplicacion();


        }

        #region TIMERS
        private void inicializa_timer_reloj()
        {
            // INICIA TIMER QUE ESTARA ACTUALIZANDO EL VALOR DE ESTATUS
            timerReloj.Tick += new EventHandler(dispatcherTimerReloj_Tick);
            timerReloj.Interval = new TimeSpan(0, 0, 1);
            timerReloj.Start();
        }
        private void inicializa_timer_evalua_mensajes()
        {
            // INICIA TIMER QUE ESTARA ACTUALIZANDO EL VALOR DE ESTATUS
            timerEvaluaMensajes.Tick += new EventHandler(dispatcherTimerEvaluaMensajes_Tick);
            timerEvaluaMensajes.Interval = new TimeSpan(0, 0, 10);
            timerEvaluaMensajes.Start();
        }
        private void inicializa_timer_sincroniza()
        {
            // INICIA TIMER QUE ESTARA ACTUALIZANDO EL VALOR DE ESTATUS
            timerSincroniza.Tick += new EventHandler(dispatcherTimerSincroniza_Tick);
            timerSincroniza.Interval = new TimeSpan(0, 1, 0);
            timerSincroniza.Start();
        }
        private void inicializa_timer_cuentacocos()
        {
            // INICIA TIMER QUE ESTARA ACTUALIZANDO EL VALOR DE ESTATUS
            timerCuentaCocos.Tick += new EventHandler(dispatcherTimerCuentaCocos_Tick);
            timerCuentaCocos.Interval = new TimeSpan(0, 0, 10);
            timerCuentaCocos.Start();
        }
        private void inicializa_timer_posicion_gps()
        {
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_frec_gps = serv_cv.getEntityByClave("GPS_FRECUENCIA_SEGUNDOS");
            int segundos = cv_frec_gps.valor != null && cv_frec_gps.valor != "" ? Convert.ToInt32(cv_frec_gps.valor) : 3;

            // INICIA TIMER QUE ESTARA ACTUALIZANDO EL VALOR DE ESTATUS
            timerPosicionGPS.Tick += new EventHandler(dispatcherTimerPosicionGPS_Tick);
            timerPosicionGPS.Interval = new TimeSpan(0, 0, segundos);
            timerPosicionGPS.Start();

           
            
        }

        private void dispatcherTimerReloj_Tick(object sender, EventArgs e)
        {
            try
            {
                lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
                lblReloj.Text = DateTime.Now.ToString("hh:mm:ss tt");

                if (txtAsignacionActivaActual.Text == "")
                {
                    ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
                    config_varios cv_asign = serv_config_varios.getEntityByClave("ASIGNACION_ACTIVA");
                    ASIGNACION_ACTIVA = cv_asign.valor.ToString().Trim();
                    txtAsignacionActivaActual.Text = ASIGNACION_ACTIVA;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EXCEPTION !!!");
            }
        }
        private void dispatcherTimerEvaluaMensajes_Tick(object sender, EventArgs e)
        {
            try
            {
                Comun comun = new Comun();
                if (comun.HayConexionInternet())
                {
                    //Envio los mensajes a TISA
                    Sincronizar_Mensajes();
                }

                //Cambia la imagen de mensajes en la menubar de principal, segun si hay o no mensajes pendientes de reproducir.
                Cambia_Imagen_Mensajes();

                //Cambia la imagen de conexion en caso de haber o no conexion a internet
                Cambia_Imagen_Evalua_Conexion_Internet();

                //Cambia la imagen de conexion serial ALCANCIA en caso de haber o no conexion a la ALCANCIA.
                Cambia_Imagen_Evalua_Conexion_Puerto_Serial_Alcancia();

                //Cambia la imagen de conexion serial CUENTA COCOS en caso de haber o no conexion a la CUENTA COCOS.
                Cambia_Imagen_Evalua_Conexion_Puerto_Serial_CuentaCocos();

                //Cambia la imagen de conexion serial GPS en caso de haber o no conexion al GPS
                Cambia_Imagen_Evalua_Conexion_Puerto_Serial_GPS();


                ////Envio de la ubicacion actual de la unidad en latitud y longitud
                //ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();
                //config_varios cv_ubicacion = serv_conf_varios.getEntityByClave("UBICACION_ACTIVA");
                //if (cv_ubicacion.valor == "HABILITADO")
                //{
                //    Sincronizar_Ubicacion();
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EXCEPTION !!!");
            }
        }
        private void dispatcherTimerSincroniza_Tick(object sender, EventArgs e)
        {
            try
            {
                //Envia los pendientes por enviar hacia TISA.
                // * Boletos tarifa fija
                // * Boletos y Detalle Multitarifa
                // * Cortes
                SincronizaOperacionConsola();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EXCEPTION !!!");
            }
        }
        private void dispatcherTimerCuentaCocos_Tick(object sender, EventArgs e)
        {
            try
            {
                // Ejecutar Comandos para Obtener Info Cuenta Cocos
                // y Guarda en local la info de cuenta cocos
                Guardar_Cuenta_Cocos_DB_Local();

                Task.WaitAll(new Task[] { Task.Delay(200) });

                //Envia los pendientes por enviar hacia TISA.
                // Cuenta Cocos
                SincronizacionTISA.SincronizaConteoCuentaCocos();
                //SincronizacionTISA.SincronizaPosicionGPS_UltimoRegistradoLocal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EXCEPTION !!!");
            }
        }
        private void dispatcherTimerPosicionGPS_Tick(object sender, EventArgs e)
        {
            try
            {
                // Ejecutar Comandos para Obtener Info Cuenta Cocos
                // y Guarda en local la info de cuenta cocos
                Guardar_Posicion_GPS_DB_Local();

                Task.WaitAll(new Task[] { Task.Delay(200) });

                //Envia los pendientes por enviar hacia TISA.
                // Posicion GPS
                SincronizacionTISA.SincronizaPosicionGPS();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.InnerException.ToString().Trim(), "EXCEPTION !!!");
            }
        }

        private void detiene_timers()
        {
            timerReloj.IsEnabled = false;
            timerEvaluaMensajes.IsEnabled = false;
            timerSincroniza.IsEnabled = false;
            timerCuentaCocos.IsEnabled = false;
            timerPosicionGPS.IsEnabled = false;
        }

        #endregion

        #region EVENTOS CONTROLES
        private void Principal_OnLoad(object sender, RoutedEventArgs e)
        {
            ServiceConfigVarios scv = new ServiceConfigVarios();

            config_varios config_alcancia_activa = scv.getEntityByClave("ALCANCIA_ACTIVA");
            config_varios config_cc1_activo = scv.getEntityByClave("CC1_ACTIVO");
            config_varios config_cc2_activo = scv.getEntityByClave("CC2_ACTIVO");
            config_varios config_cc3_activo = scv.getEntityByClave("CC3_ACTIVO");
            config_varios config_gps_activo = scv.getEntityByClave("UBICACION_ACTIVA");


            if (config_alcancia_activa.valor == "HABILITADO")
            {
                configura_puerto_serial();
            }

            if (config_cc1_activo.valor == "HABILITADO" || config_cc2_activo.valor == "HABILITADO" || config_cc3_activo.valor == "HABILITADO")
            {
                configura_puerto_serial_cuenta_cocos();
                inicializa_timer_cuentacocos();
            }

            if (config_gps_activo.valor == "HABILITADO")
            {
                configura_puerto_serial_gps();
                inicializa_timer_posicion_gps();

                //RESET TO DEFAULT MANUFACTURED
                //reset_to_default_manufactured();


                settings_params_gps();
            }

            inicializa_timer_reloj();
            inicializa_timer_evalua_mensajes();
            inicializa_timer_sincroniza();
            

            Comun comun = new Comun();
            if (comun.HayConexionInternet())
            {
                SincronizarCatalogos();
                SincronizarConfiguraciones();
                SincronizaOperacionConsola();
            }

            SetearVersionYCopyright();

            //OBTENER CONFIGURACIONES VARIAS DEL SISTEMA Y OPERACION ACTUAL
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_asign = serv_config_varios.getEntityByClave("ASIGNACION_ACTIVA");
            config_varios cv_modo = serv_config_varios.getEntityByClave("MODO");

            ServiceAsignaciones serv_asign = new ServiceAsignaciones();
            sy_asignaciones asig = serv_asign.getEntityByFolio(cv_asign.valor);

            ASIGNACION_ACTIVA = cv_asign.valor;
            FK_ASIGNACION_ACTIVA = cv_asign.valor != "" ? asig.pkAsignacion : 0;
            MODO_APP = cv_modo.valor;


            //Validar Tipo Usuario Logueado, para mostrar o ocultar la opcion de Configuracion del Menu
            validar_tipo_usuario();

            txtUsuarioActivo.Text = "Usuario: " + setUsuarioActivoLblPrincipal();
            txtAsignacionActivaActual.Text = ASIGNACION_ACTIVA.ToString().Trim();


        }
        private void Principal_OnUnLoad(object sender, RoutedEventArgs e)
        {
            LimpiarUsuarioActualLogueado();
            detiene_timers();
            LimpiarAsignacionActual();


            close_serial_port_cuenta_cocos(); /// 2024-09-19 EMD
            close_serial_port_gps();

            ////////////////////////////////////////////////////////////////////////
            // ESTAS TAREAS SE DETENDRAN AL SALIR DEL FORMULARIO.
            //SincronizacionTISA sTISA = new SincronizacionTISA();
            //sTISA.Task_Sincroniza_Boletos_y_BoletosDetalle_DISPOSE();

            /*
                sTISA.Task_Sincroniza_Boletos_DISPOSE();
                sTISA.Task_Sincroniza_BoletosDetalle_DISPOSE();
            */
            ////////////////////////////////////////////////////////////////////////
        }
        private void Principal_Closing(object sender, CancelEventArgs e)
        {
            Login login = new Login();
            login.Show();
        }
        private void TBShow(object sender, RoutedEventArgs e)
        {
            GridContent.Opacity = 0.5;
        }
        private void TBHide(object sender, RoutedEventArgs e)
        {
            GridContent.Opacity = 1;
        }
        private void btnCerrarClick(object sender, RoutedEventArgs e)
        {
            SetUltimoUsuarioLogueado();
            LimpiarUsuarioActualLogueado();
            Close();
        }
        private void PreviewMouseLeftBottonDownBG(object sender, MouseButtonEventArgs e)
        {
            btnShowHide.IsChecked = false;
        }
        private void btnMinimizarClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnInicio_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new Home();
        }
        private void btnConfiguraciones_Click(object sender, RoutedEventArgs e)
        {
            //DataContext = new Configuracionv2();
            DataContext = new ConfiguracionV4();
        }
        private void btnCobroTarifa_Click(object sender, RoutedEventArgs e)
        {
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios config_varios_alcancia = serv_cv.getEntityByClave("ALCANCIA_ACTIVA");
            if(config_varios_alcancia.valor == "HABILITADO")
            {
                if (HayAsignacionActiva())
                {
                    ServiceConfigVarios scv = new ServiceConfigVarios();
                    config_varios cv = scv.getEntityByClave("TIPO_TARIFA");

                    if (cv != null)
                    {
                        if (cv.valor == "FIJA")
                        {
                            DataContext = new CobroTarifaFija();
                        }
                        if (cv.valor == "MULTITARIFA_IMG")
                        {
                            DataContext = new CobroMultitarifaV1();
                        }
                        if (cv.valor == "MULTITARIFA")
                        {
                            DataContext = new CobroMultitarifaV2();
                        }
                        if (cv.valor == "FIJA_DINAMICA")
                        {
                            DataContext = new CobroTarifaFijaBotones();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("FAVOR DE SELECCIONAR UNA ASIGNACION ANTES DE OPERAR LA UNIDAD DE TRANSPORTE");
                }
            }
            else
            {
                MessageBox.Show("NO ESTA HABILITADA LA ALCANCIA, FAVOR DE CONFIGURAR EL PUERTO COM CORRESPONDIENTE Y HABILITARLA");
            }
        }
        private void btnReportes_Click(object sender, RoutedEventArgs e)
        {
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios config_varios_alcancia = serv_cv.getEntityByClave("ALCANCIA_ACTIVA");
            if (config_varios_alcancia.valor == "HABILITADO")
            {
                if (HayAsignacionActiva())
                {
                    DataContext = new Reportes();
                }
                else
                {
                    MessageBox.Show("FAVOR DE SELECCIONAR UNA ASIGNACION ANTES DE OPERAR LA UNIDAD DE TRANSPORTE");
                }
            }
            else
            {
                MessageBox.Show("NO ESTA HABILITADA LA ALCANCIA, FAVOR DE CONFIGURAR EL PUERTO COM CORRESPONDIENTE Y HABILITARLA");
            }
        }
        private void btnResetPortName_Click(object sender, RoutedEventArgs e)
        {
            //ejecutar_commando_12_reset_cc1();
            //ejecutar_commando_13_acumulado_pasajeros_cc1();


            //ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            //config_varios cv_port_name = serv_config_varios.getEntityByClave("PORT_NAME");
            //if (cv_port_name.valor != null && cv_port_name.valor != "")
            //{
            //    cv_port_name.valor = "";
            //    serv_config_varios.updEntityByClave(cv_port_name);
            //}

            //ejecutar_comando_5_obtener_posicion_gps();

            //if(FK_ASIGNACION_ACTIVA != 0)
            //{
            //    Guardar_Cuenta_Cocos_DB_Local();
            //    SincronizacionTISA.SincronizaConteoCuentaCocos();
            //}




        }
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            SetUltimoUsuarioLogueado();

            //Hacer una funcion para hacer Logout a los servicios de TISA.
            LimpiarUsuarioActualLogueado();

            //Cerrar la ventana.
            Close();
        }
        private void btnAsignacion_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new vwAsignaciones();
        }
        private void btnAcercaDe_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new AcercaDe();
        }
        private void btnCuenta_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new MiCuenta();
        }
        private void btnMensajes_Click(object sender, RoutedEventArgs e)
        {
            if (HayAsignacionActiva())
            {
                DataContext = new Mensajes();
            }
            else
            {
                MessageBox.Show("FAVOR DE SELECCIONAR UNA ASIGNACION ANTES DE OPERAR LA UNIDAD DE TRANSPORTE");
            }
        }

        private string setUsuarioActivoLblPrincipal()
        {
            ServiceConfigVarios serv_config_varios_user = new ServiceConfigVarios();
            config_varios cv_user = serv_config_varios_user.getEntityByClave("USUARIO_ACTUAL");

            ServiceComun_Usuarios serv_usuarios = new ServiceComun_Usuarios();
            ct_usuarios obj_usuario_actual = serv_usuarios.getEntityByUser(cv_user.valor);

            return obj_usuario_actual.usuario;

        }
        #endregion

        #region METODOS SINCRONIZA CATALOGOS TISA -> CONSOLA

        private void SincronizarCatalogos()
        {
            Sincroniza_Empresas();
            Sincroniza_Corredores();
            Sincroniza_Unidades();
            Sincroniza_Lugares();
            Sincroniza_Operadores();
            Sincroniza_Perfiles();
            Sincroniza_Rutas();
            Sincroniza_Tarifas();
            Sincroniza_Andadores();
            Sincroniza_LugarRuta();
            Sincroniza_EmpresaCorredorOperador();
            Sincroniza_Asignaciones();

        }

        private void SincronizarConfiguraciones()
        {
            /* PARA SINCRONIZAR LAS TABLAS DESDE TISA HACIA CONSOLA
                * dbo.config_varios
                * dbo.opciones_generales
                * dbo.ct_tarifas_montos_fijos
                * dbo.ct_denominaciones
             */
            ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();

            config_varios cv_sinc_config_varios = serv_conf_varios.getEntityByClave("SINC_TISA_CONFIG_VARIOS");
            if (cv_sinc_config_varios != null)
            {
                if (cv_sinc_config_varios.valor == "SI")
                {
                    Sincroniza_ConfigVarios();
                }
            }

            config_varios cv_sinc_tarifas_montos_fijos = serv_conf_varios.getEntityByClave("SINC_TISA_TARIFAS_MONTOS_FIJOS");
            if (cv_sinc_tarifas_montos_fijos != null)
            {
                if (cv_sinc_tarifas_montos_fijos.valor == "SI")
                {
                    Sincroniza_TarifasMontosFijos();
                }
            }

            config_varios cv_sinc_denominaciones = serv_conf_varios.getEntityByClave("SINC_TISA_DENOMINACIONES");
            if (cv_sinc_denominaciones != null)
            {
                if (cv_sinc_denominaciones.valor == "SI")
                {
                    Sincroniza_Denominaciones();
                }
            }

            config_varios cv_sinc_opciones_generales = serv_conf_varios.getEntityByClave("SINC_TISA_OPCIONES_GENERALES");
            if (cv_sinc_opciones_generales != null)
            {
                if (cv_sinc_opciones_generales.valor == "SI")
                {
                    Sincroniza_OpcionesGenerales();
                }
            }
        }

        private void Sincroniza_Empresas()
        {
            //Se obtienen las unidades del endpoint
            EmpresasController uc = new EmpresasController();
            List<ct_empresas> list = uc.GetEmpresas();

            //Se insertan las unidades en la base local
            ServiceEmpresas serv = new ServiceEmpresas();
            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkEmpresa) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Empresas();
        }
        private void Sincroniza_Corredores()
        {
            //Se obtienen las unidades del endpoint
            CorredoresController uc = new CorredoresController();
            List<ct_corredores> list = uc.GetCorredores();

            //Se insertan las unidades en la base local
            ServiceCorredores serv = new ServiceCorredores();
            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkCorredor) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Corredores();
        }
        private void Sincroniza_Unidades()
        {
            //Se obtienen las unidades del endpoint
            UnidadesController uc = new UnidadesController();
            List<ct_unidades> list = uc.GetUnidades();

            //Se insertan las unidades en la base local
            ServiceUnidades serv = new ServiceUnidades();
            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkUnidad) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Unidades();
        }
        private void Sincroniza_Andadores()
        {
            //Se obtienen del endpoint
            AndadoresController uc = new AndadoresController();
            List<ct_andadores> list = uc.GetAndadores();

            //Se insertan en la base local
            ServiceAndadores serv = new ServiceAndadores();
            if ((serv.getEntities()).Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (serv.getEntity(list[i].pkAndador) != null)
                    {
                        serv.updEntity(list[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Andadores();
        }
        private void Sincroniza_Lugares()
        {
            //Se obtienen las unidades del endpoint
            LugaresController uc = new LugaresController();
            List<ct_lugares> list = uc.GetLugares();

            //Se insertan las unidades en la base local
            ServiceLugares serv = new ServiceLugares();


            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkLugar) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Lugares();
        }
        private void Sincroniza_Operadores()
        {
            //Se obtienen las unidades del endpoint
            OperadoresController uc = new OperadoresController();
            List<ct_operadores> list = uc.GetOperadores();

            //Se insertan las unidades en la base local
            ServiceOperadores serv = new ServiceOperadores();
            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkOperador) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Operadores();
        }
        private void Sincroniza_Perfiles()
        {
            //Se obtienen las unidades del endpoint
            PerfilesController uc = new PerfilesController();
            List<ct_perfiles> list = uc.GetPerfiles();

            //Se insertan las unidades en la base local
            ServicePerfiles serv = new ServicePerfiles();
            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkPerfil) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Perfiles();
        }
        private void Sincroniza_Rutas()
        {
            //Se obtienen las unidades del endpoint
            RutasController uc = new RutasController();
            List<ct_rutas> list = uc.GetRutas();

            //Se insertan las unidades en la base local
            ServiceRutas serv = new ServiceRutas();
            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkRuta) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Rutas();
        }
        private void Sincroniza_Tarifas()
        {
            //Se obtienen del endpoint
            TarifasController uc = new TarifasController();
            List<sy_tarifas> list = uc.GetTarifas();

            //Se insertan en la base local
            ServiceTarifas serv = new ServiceTarifas();

            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkTarifa) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Tarifas();
        }
        private void Sincroniza_LugarRuta()
        {
            //Se obtienen las unidades del endpoint
            LugarRutaController uc = new LugarRutaController();
            List<sy_lugar_ruta> list = uc.GetLugaresRutas();

            //Se insertan las unidades en la base local
            ServiceLugarRuta serv = new ServiceLugarRuta();
            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkLugarRuta) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_LugarRuta();
        }
        private void Sincroniza_EmpresaCorredorOperador()
        {
            //Se obtienen las unidades del endpoint
            EmpresaCorredorOperadorController uc = new EmpresaCorredorOperadorController();
            List<sy_empresa_corredor_operador> list = uc.GetEmpresaCorredorOperador();

            //Se insertan las unidades en la base local
            ServiceEmpresaCorredorOperador serv = new ServiceEmpresaCorredorOperador();
            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkEmpresaCorredorOperador) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_EmpresaCorredorOperador();
        }
        private void Sincroniza_Asignaciones()
        {
            //se obtiene el nombre de la unidad desde las configuraciones
            ServiceConfigVarios scv = new ServiceConfigVarios();
            config_varios cv_numero_unidad = scv.getEntityByClave("NUMERO_UNIDAD");

            if (cv_numero_unidad != null && cv_numero_unidad.valor != null)
            {
                if (cv_numero_unidad.valor != "")
                {
                    //Se obtiene el pkUnidad desde el Numero de Unidad, configurado para esta consola.
                    ServiceUnidades suni = new ServiceUnidades();
                    ct_unidades unidad = suni.getEntityByName(cv_numero_unidad.valor);

                    //Se obtienen las unidades del endpoint
                    AsignacionesController uc = new AsignacionesController();
                    List<sy_asignaciones> list = uc.GetAsignacionesByUnidad(unidad.pkUnidad);

                    //Se insertan las unidades en la base local
                    ServiceAsignaciones serv = new ServiceAsignaciones();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (serv.getEntity(list[i].pkAsignacion) != null)
                        {
                            serv.updEntity(list[i]);
                        }
                        else
                        {
                            serv.addEntity(list[i]);
                        }
                    }
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Asignaciones();
        }

        private void Sincroniza_TarifasMontosFijos()
        {
            //Obtener el fkUnidad
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_nombre_unidad = serv_config_varios.getEntityByClave("NUMERO_UNIDAD");
            ServiceUnidades serv_unidades = new ServiceUnidades();
            ct_unidades obj_unidad = serv_unidades.getEntityByName(cv_nombre_unidad.valor);

            //Se obtienen las unidades del endpoint
            TarifasMontosFijosController controller = new TarifasMontosFijosController();
            List<ct_tarifas_montos_fijos> list = controller.GetTarifasMontosFijosPorUnidad(obj_unidad);

            //Se insertan las unidades en la base local
            ServiceTarifasMontosFijos serv = new ServiceTarifasMontosFijos();

            for (int i = 0; i < list.Count; i++)
            {
                ct_tarifas_montos_fijos obj_tmf = serv.getEntityByValor(decimal.Parse(list[i].valor).ToString("#0.00"));

                if (obj_tmf != null)
                {
                    if (obj_tmf.valor.ToString().Trim() != decimal.Parse(list[i].valor).ToString("#0.00").Trim())
                    {
                        //en caso de no existir el valor en la base local de la aplicacion, esta tarifa monto fijo se insertara
                        serv.addEntity(list[i]);
                    }
                }
                else
                {
                    //en caso de no existir el valor en la base local de la aplicacion, esta tarifa monto fijo se insertara
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_TarifasMontosFijos();
        }
        private void Sincroniza_Denominaciones()
        {
            //Obtener el fkUnidad
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_nombre_unidad = serv_config_varios.getEntityByClave("NUMERO_UNIDAD");
            ServiceUnidades serv_unidades = new ServiceUnidades();
            ct_unidades obj_unidad = serv_unidades.getEntityByName(cv_nombre_unidad.valor);

            //Se obtienen las unidades del endpoint
            DenominacionesController controller = new DenominacionesController();
            List<ct_denominaciones> list = controller.GetDenominacionesPorUnidad(obj_unidad);

            //Se insertan las denominaciones en la base local
            ServiceDenominaciones serv = new ServiceDenominaciones();

            for (int i = 0; i < list.Count; i++)
            {
                ct_denominaciones obj_deno = serv.getEntityByValor(decimal.Parse(list[i].valor).ToString("#0.00"));

                if (obj_deno != null)
                {
                    if (obj_deno.valor.ToString().Trim() != decimal.Parse(list[i].valor).ToString("#0.00").Trim())
                    {
                        //en caso de no existir el valor en la base local de la aplicacion, esta tarifa monto fijo se insertara
                        serv.addEntity(list[i]);
                    }
                }
                else
                {
                    //en caso de no existir el valor en la base local de la aplicacion, esta tarifa monto fijo se insertara
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_Denominaciones();
        }
        private void Sincroniza_OpcionesGenerales()
        {
            //Obtener el fkUnidad
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_nombre_unidad = serv_config_varios.getEntityByClave("NUMERO_UNIDAD");
            ServiceUnidades serv_unidades = new ServiceUnidades();
            ct_unidades obj_unidad = serv_unidades.getEntityByName(cv_nombre_unidad.valor);

            //Se obtienen las unidades del endpoint
            OpcionesGeneralesController controller = new OpcionesGeneralesController();
            List<opciones_generales> list = controller.GetOpcionesGeneralesPorUnidad(obj_unidad);

            //Se insertan las denominaciones en la base local
            ServiceOpcionesGenerales serv = new ServiceOpcionesGenerales();

            for (int i = 0; i < list.Count; i++)
            {
                opciones_generales obj = serv.getEntityByOpcionGeneral(list[i].opcion_general);

                if (obj != null)
                {
                    serv.updEntityByOpcionGeneral(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_OpcionesGenerales();
        }
        private void Sincroniza_ConfigVarios()
        {
            //Obtener el fkUnidad
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_nombre_unidad = serv_config_varios.getEntityByClave("NUMERO_UNIDAD");
            ServiceUnidades serv_unidades = new ServiceUnidades();
            ct_unidades obj_unidad = serv_unidades.getEntityByName(cv_nombre_unidad.valor);

            //Se obtienen las unidades del endpoint
            ConfigVariosController controller = new ConfigVariosController();
            List<config_varios> list = controller.GetConfigVariosPorUnidad(obj_unidad);

            //Se insertan las denominaciones en la base local
            ServiceConfigVarios serv = new ServiceConfigVarios();

            for (int i = 0; i < list.Count; i++)
            {
                config_varios obj = serv.getEntityByClave(list[i].clave);

                if (obj != null)
                {
                    if (obj.clave != "USUARIO_ACTUAL")
                    {
                        serv.updEntityByClave(list[i]);
                    }
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }

            //Eliminar los registros que se hayan eliminado en TISA
            DeleteLocal_ConfigVarios();
        }


        #endregion

        #region Eliminar los registros en la sincronizacion
        private void DeleteLocal_Empresas()
        {
            EmpresasController controller = new EmpresasController();
            List<ct_empresas> list_controller = controller.GetEmpresas();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkEmpresa);
            }

            ServiceEmpresas serv = new ServiceEmpresas();
            List<ct_empresas> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkEmpresa);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_empresas obj = list_local.Find(q => q.pkEmpresa == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Corredores()
        {
            CorredoresController controller = new CorredoresController();
            List<ct_corredores> list_controller = controller.GetCorredores();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkCorredor);
            }

            ServiceCorredores serv = new ServiceCorredores();
            List<ct_corredores> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkCorredor);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_corredores obj = list_local.Find(q => q.pkCorredor == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Unidades()
        {
            UnidadesController controller = new UnidadesController();
            List<ct_unidades> list_controller = controller.GetUnidades();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkUnidad);
            }

            ServiceUnidades serv = new ServiceUnidades();
            List<ct_unidades> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkUnidad);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_unidades obj = list_local.Find(q => q.pkUnidad == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Andadores()
        {
            AndadoresController controller = new AndadoresController();
            List<ct_andadores> list_controller = controller.GetAndadores();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkAndador);
            }

            ServiceAndadores serv = new ServiceAndadores();
            List<ct_andadores> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkAndador);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_andadores obj = list_local.Find(q => q.pkAndador == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Lugares()
        {
            LugaresController controller = new LugaresController();
            List<ct_lugares> list_controller = controller.GetLugares();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkLugar);
            }

            ServiceLugares serv = new ServiceLugares();
            List<ct_lugares> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkLugar);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_lugares obj = list_local.Find(q => q.pkLugar == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Operadores()
        {
            OperadoresController controller = new OperadoresController();
            List<ct_operadores> list_controller = controller.GetOperadores();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkOperador);
            }

            ServiceOperadores serv = new ServiceOperadores();
            List<ct_operadores> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkOperador);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_operadores obj = list_local.Find(q => q.pkOperador == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Perfiles()
        {
            PerfilesController controller = new PerfilesController();
            List<ct_perfiles> list_controller = controller.GetPerfiles();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkPerfil);
            }

            ServicePerfiles serv = new ServicePerfiles();
            List<ct_perfiles> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkPerfil);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_perfiles obj = list_local.Find(q => q.pkPerfil == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Rutas()
        {
            RutasController controller = new RutasController();
            List<ct_rutas> list_controller = controller.GetRutas();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkRuta);
            }

            ServiceRutas serv = new ServiceRutas();
            List<ct_rutas> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkRuta);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_rutas obj = list_local.Find(q => q.pkRuta == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Tarifas()
        {
            TarifasController controller = new TarifasController();
            List<sy_tarifas> list_controller = controller.GetTarifas();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkTarifa);
            }

            ServiceTarifas serv = new ServiceTarifas();
            List<sy_tarifas> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkTarifa);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                sy_tarifas obj = list_local.Find(q => q.pkTarifa == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_LugarRuta()
        {
            LugarRutaController controller = new LugarRutaController();
            List<sy_lugar_ruta> list_controller = controller.GetLugaresRutas();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkLugarRuta);
            }

            ServiceLugarRuta serv = new ServiceLugarRuta();
            List<sy_lugar_ruta> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkLugarRuta);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                sy_lugar_ruta obj = list_local.Find(q => q.pkLugarRuta == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_EmpresaCorredorOperador()
        {
            EmpresaCorredorOperadorController controller = new EmpresaCorredorOperadorController();
            List<sy_empresa_corredor_operador> list_controller = controller.GetEmpresaCorredorOperador();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkEmpresaCorredorOperador);
            }

            ServiceEmpresaCorredorOperador serv = new ServiceEmpresaCorredorOperador();
            List<sy_empresa_corredor_operador> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkEmpresaCorredorOperador);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                sy_empresa_corredor_operador obj = list_local.Find(q => q.pkEmpresaCorredorOperador == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Asignaciones()
        {
            AsignacionesController controller = new AsignacionesController();
            List<sy_asignaciones> list_controller = controller.GetAsignaciones();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkAsignacion);
            }

            ServiceAsignaciones serv = new ServiceAsignaciones();
            List<sy_asignaciones> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkAsignacion);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                sy_asignaciones obj = list_local.Find(q => q.pkAsignacion == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_TarifasMontosFijos()
        {
            TarifasMontosFijosController controller = new TarifasMontosFijosController();
            List<ct_tarifas_montos_fijos> list_controller = controller.GetTarifasMontosFijos();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkTarifaMontoFijo);
            }

            ServiceTarifasMontosFijos serv = new ServiceTarifasMontosFijos();
            List<ct_tarifas_montos_fijos> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkTarifaMontoFijo);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_tarifas_montos_fijos obj = list_local.Find(q => q.pkTarifaMontoFijo == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_Denominaciones()
        {
            DenominacionesController controller = new DenominacionesController();
            List<ct_denominaciones> list_controller = controller.GetDenominaciones();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkDenominacion);
            }

            ServiceDenominaciones serv = new ServiceDenominaciones();
            List<ct_denominaciones> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkDenominacion);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                ct_denominaciones obj = list_local.Find(q => q.pkDenominacion == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_OpcionesGenerales()
        {
            OpcionesGeneralesController controller = new OpcionesGeneralesController();
            List<opciones_generales> list_controller = controller.GetOpcionesGenerales();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkOpcionGeneral);
            }

            ServiceOpcionesGenerales serv = new ServiceOpcionesGenerales();
            List<opciones_generales> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkOpcionGeneral);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                opciones_generales obj = list_local.Find(q => q.pkOpcionGeneral == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }
        private void DeleteLocal_ConfigVarios()
        {
            ConfigVariosController controller = new ConfigVariosController();
            List<config_varios> list_controller = controller.GetConfigVarios();
            List<long> list_controller_pks = new List<long>();
            foreach (var item in list_controller)
            {
                list_controller_pks.Add(item.pkConfigVarios);
            }

            ServiceConfigVarios serv = new ServiceConfigVarios();
            List<config_varios> list_local = serv.getEntities();
            List<long> list_local_pks = new List<long>();
            foreach (var item in list_local)
            {
                list_local_pks.Add(item.pkConfigVarios);
            }

            List<long> list_to_delete = list_local_pks.Except(list_controller_pks).ToList();
            foreach (long it in list_to_delete)
            {
                config_varios obj = list_local.Find(q => q.pkConfigVarios == it);
                obj.deleted_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                serv.updEntity(obj);
            }
        }

        #endregion

        #region METODOS SINCRONIZA OPERACION CONSOLA -> TISA
        private void SincronizaOperacionConsola()
        {
            SincronizacionTISA.SincronizaBoletosYBoletosDetalle();
            SincronizacionTISA.SincronizaCortes();
            SincronizacionTISA.SincronizaBoletosTarifaFija();

        }

        #endregion

        #region METODOS PROPIOS
        private void SetUltimoUsuarioLogueado()
        {
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_usuario_actual = serv_config_varios.getEntityByClave("USUARIO_ACTUAL");


            config_varios cv_ult_usu_logueado = new config_varios();
            cv_ult_usu_logueado.clave = "ULTIMO_USUARIO_LOGUEADO";
            cv_ult_usu_logueado.valor = cv_usuario_actual.valor;
            serv_config_varios.updEntityByClave(cv_ult_usu_logueado);
        }
        private void SetearVersionYCopyright()
        {
            ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();

            config_varios cv_version = serv_conf_varios.getEntityByClave("VERSION");
            string version = cv_version.valor;

            config_varios cv_copy = serv_conf_varios.getEntityByClave("COPYRIGHT");
            string copyright = cv_copy.valor;

            //txtVersion.Text = version;
            txtCopyright.Text = copyright;
        }
        private void LimpiarUsuarioActualLogueado()
        {
            // ACTUALIZAR EN LA BASE DE DATOS CON EL USUARIO ACTUAL CONECTADO
            ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();
            config_varios cv_usuario_actual = new config_varios();
            cv_usuario_actual.clave = "USUARIO_ACTUAL";
            cv_usuario_actual.valor = "NINGUNO";
            serv_conf_varios.updEntityByClave(cv_usuario_actual);
        }
        private void Sincronizar_Mensajes()
        {
            // DISPOSITIVOS
            // 1 = UNIDAD
            // 2 = TISA

            string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MensajesController mensajes_controller = new MensajesController();

            ServiceMensajes serv_mensajes = new ServiceMensajes();

            //se sincronizan los mensajes desde la unidad hacia TISA
            List<sy_mensajes> list_mensaje_hacia_tisa = serv_mensajes.getEntityNoEnviados();
            foreach (var item in list_mensaje_hacia_tisa)
            {
                ResMensajes_Insert resMensajes_insert = mensajes_controller.InsertMensaje(item);
                if (resMensajes_insert.response == true && resMensajes_insert.status == 200)
                {
                    item.enviado = 1;
                    item.confirmadoTISA = 1;
                    serv_mensajes.updEntity(item);
                }
            }

            //se sincronizan los mensajes desde TISA hacia la unidad
            if (FK_ASIGNACION_ACTIVA != null && FK_ASIGNACION_ACTIVA != 0)
            {
                List<sy_mensajes> list_msn_desde_tisa = mensajes_controller.GetMensajesConsolaDesdeTISA(FK_ASIGNACION_ACTIVA);
                foreach (var item in list_msn_desde_tisa)
                {
                    // se pone la bandera de enviado a 1 para que no se vuelva a consultar de nuevo este mensaje desde TISA
                    item.enviado = 1;
                    mensajes_controller.UpdateMensaje(item);

                    // insertar en la base de datos local
                    serv_mensajes.addEntity(item);
                }
            }


        }
        private void Sincronizar_Ubicacion()
        {
            try
            {
                //UBICACION ACTUAL DE LA UNIDAD
                CLocation myLocation = new CLocation();
                myLocation.GetLocationDataEvent();

                //Task.WaitAll(new Task[] { Task.Delay(1000) });
                //decimal lat = myLocation.latitud;
                //decimal lon = myLocation.longitud;

                ////Enviar la Ubicacion a TISA, por medio del servicio
                //UbicacionController ubi_controller = new UbicacionController();
                //sy_ubicacion ubi = new sy_ubicacion();
                //ubi.fkAsignacion = FK_ASIGNACION_ACTIVA;
                //ubi.latitud = lat;
                //ubi.longitud = lon;
                //ubi.enviado = 1;
                //ubi.confirmadoTISA = 0;
                //ubi.modo = MODO_APP;
                //ResUbicacion res_ubicacion = ubi_controller.InsertUbicacion(ubi);

                ////Insertar la ubicacion en la dblocal en caso de que se haya insertado correctamente en TISA.
                //if (res_ubicacion.response == true && res_ubicacion.status == 200)
                //{
                //    ServiceUbicacion serv_ubicacion = new ServiceUbicacion();
                //    serv_ubicacion.addEntity(ubi);
                //}
            }
            catch (Exception ex)
            {


            }
        }
        private void Cambia_Imagen_Mensajes()
        {
            try
            {
                //string ruta_sonido = @"/SCS/sonidos/shoot2.wav";
                string ruta_sonido = @"shoot2.wav";
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(ruta_sonido);

                ServiceMensajes serv_mensajes = new ServiceMensajes();
                List<sy_mensajes> list_msn_no_reproducidos = serv_mensajes.getEntityNoReproducidos();

                if (list_msn_no_reproducidos.Count > 0)
                {
                    imgOpcionMensajes.Source = new BitmapImage(new Uri(@"/SCS/IMG/mensajes_rojo.png", UriKind.Relative));

                    //reproducimos
                    player.Play();
                }
                else
                {
                    imgOpcionMensajes.Source = new BitmapImage(new Uri(@"/SCS/IMG/mensajes.png", UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void cargar_logo_aplicacion()
        {
            //this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.mt_consola_icon., Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            BitmapSource mt_icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.mt_consola.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            this.Icon = mt_icon;
        }
        private void cargar_logo_aplicacion_v2()
        {
            // Set an icon using code
            Uri iconUri = new Uri(@"C:\mt_con_database\mt_consola.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
        }
        private void validar_tipo_usuario()
        {
            ServiceConfigVarios serv_config_varios_user = new ServiceConfigVarios();
            config_varios cv_user = serv_config_varios_user.getEntityByClave("USUARIO_ACTUAL");

            ServiceComun_Usuarios serv_usuarios = new ServiceComun_Usuarios();
            ct_usuarios obj_usuario_actual = serv_usuarios.getEntityByUser(cv_user.valor);

            switch (obj_usuario_actual.tipo_usuario)
            {
                case "OPERADOR":
                    //BOTON MENU - ASIGNAR TARIFA
                    btnCobroTarifa.IsEnabled = true;
                    btnCobroTarifa.Visibility = Visibility.Visible;
                    //BOTON MENU - CORTES
                    btnReportes.IsEnabled = false;
                    btnReportes.Visibility = Visibility.Hidden;
                    //BOTON MENU - MENSAJES
                    btnMensajes.IsEnabled = true;
                    btnMensajes.Visibility = Visibility.Visible;
                    //BOTON MENU - CONFIGURACIONES
                    btnConfiguraciones.IsEnabled = false;
                    btnConfiguraciones.Visibility = Visibility.Hidden;
                    break;
                case "CORTES":
                    //BOTON MENU - ASIGNAR TARIFA
                    btnCobroTarifa.IsEnabled = false;
                    btnCobroTarifa.Visibility = Visibility.Hidden;
                    //BOTON MENU - CORTES
                    btnReportes.IsEnabled = true;
                    btnReportes.Visibility = Visibility.Visible;
                    //BOTON MENU - MENSAJES
                    btnMensajes.IsEnabled = true;
                    btnMensajes.Visibility = Visibility.Visible;
                    //BOTON MENU - CONFIGURACIONES
                    btnConfiguraciones.IsEnabled = false;
                    btnConfiguraciones.Visibility = Visibility.Hidden;
                    break;
                case "INSTALL_CONFIG":
                    //BOTON MENU - ASIGNAR TARIFA
                    btnCobroTarifa.IsEnabled = true;
                    btnCobroTarifa.Visibility = Visibility.Visible;
                    //BOTON MENU - CORTES
                    btnReportes.IsEnabled = true;
                    btnReportes.Visibility = Visibility.Visible;
                    //BOTON MENU - MENSAJES
                    btnMensajes.IsEnabled = true;
                    btnMensajes.Visibility = Visibility.Visible;
                    //BOTON MENU - CONFIGURACIONES
                    btnConfiguraciones.IsEnabled = true;
                    btnConfiguraciones.Visibility = Visibility.Visible;
                    break;
                case "MT_WEB":
                    //BOTON MENU - ASIGNAR TARIFA
                    btnCobroTarifa.IsEnabled = false;
                    btnCobroTarifa.Visibility = Visibility.Hidden;
                    //BOTON MENU - CORTES
                    btnReportes.IsEnabled = false;
                    btnReportes.Visibility = Visibility.Hidden;
                    //BOTON MENU - MENSAJES
                    btnMensajes.IsEnabled = false;
                    btnMensajes.Visibility = Visibility.Hidden;
                    //BOTON MENU - CONFIGURACIONES
                    btnConfiguraciones.IsEnabled = false;
                    btnConfiguraciones.Visibility = Visibility.Hidden;
                    //BOTON MENU - ASIGNACIONES
                    btnAsignacion.IsEnabled = false;
                    btnAsignacion.Visibility = Visibility.Hidden;
                    break;
                case "MT_TAQUILLA":
                    //BOTON MENU - ASIGNAR TARIFA
                    btnCobroTarifa.IsEnabled = false;
                    btnCobroTarifa.Visibility = Visibility.Hidden;
                    //BOTON MENU - CORTES
                    btnReportes.IsEnabled = false;
                    btnReportes.Visibility = Visibility.Hidden;
                    //BOTON MENU - MENSAJES
                    btnMensajes.IsEnabled = false;
                    btnMensajes.Visibility = Visibility.Hidden;
                    //BOTON MENU - CONFIGURACIONES
                    btnConfiguraciones.IsEnabled = false;
                    btnConfiguraciones.Visibility = Visibility.Hidden;
                    //BOTON MENU - ASIGNACIONES
                    btnAsignacion.IsEnabled = false;
                    btnAsignacion.Visibility = Visibility.Hidden;
                    break;


            }
        }

        private void validar_tipo_usuario_OLD()
        {
            ServiceConfigVarios serv_config_varios_user = new ServiceConfigVarios();
            config_varios cv_user = serv_config_varios_user.getEntityByClave("USUARIO_ACTUAL");

            ServiceUsers serv_users = new ServiceUsers();
            users obj_user_actual = serv_users.getEntityByUser(cv_user.valor);

            switch (obj_user_actual.m_surname)
            {
                case "OPERADOR":
                    //BOTON MENU - ASIGNAR TARIFA
                    btnCobroTarifa.IsEnabled = true;
                    btnCobroTarifa.Visibility = Visibility.Visible;
                    //BOTON MENU - CORTES
                    btnReportes.IsEnabled = false;
                    btnReportes.Visibility = Visibility.Hidden;
                    //BOTON MENU - MENSAJES
                    btnMensajes.IsEnabled = true;
                    btnMensajes.Visibility = Visibility.Visible;
                    //BOTON MENU - CONFIGURACIONES
                    btnConfiguraciones.IsEnabled = false;
                    btnConfiguraciones.Visibility = Visibility.Hidden;
                    break;
                case "CORTES":
                    //BOTON MENU - ASIGNAR TARIFA
                    btnCobroTarifa.IsEnabled = false;
                    btnCobroTarifa.Visibility = Visibility.Hidden;
                    //BOTON MENU - CORTES
                    btnReportes.IsEnabled = true;
                    btnReportes.Visibility = Visibility.Visible;
                    //BOTON MENU - MENSAJES
                    btnMensajes.IsEnabled = true;
                    btnMensajes.Visibility = Visibility.Visible;
                    //BOTON MENU - CONFIGURACIONES
                    btnConfiguraciones.IsEnabled = false;
                    btnConfiguraciones.Visibility = Visibility.Hidden;
                    break;
                case "INSTALL_CONFIG":
                    //BOTON MENU - ASIGNAR TARIFA
                    btnCobroTarifa.IsEnabled = true;
                    btnCobroTarifa.Visibility = Visibility.Visible;
                    //BOTON MENU - CORTES
                    btnReportes.IsEnabled = true;
                    btnReportes.Visibility = Visibility.Visible;
                    //BOTON MENU - MENSAJES
                    btnMensajes.IsEnabled = true;
                    btnMensajes.Visibility = Visibility.Visible;
                    //BOTON MENU - CONFIGURACIONES
                    btnConfiguraciones.IsEnabled = true;
                    btnConfiguraciones.Visibility = Visibility.Visible;
                    break;



            }
        }
        private void Cambia_Imagen_Evalua_Conexion_Internet()
        {
            //Sincronizar los usuarios desde TISA hacia la CONSOLA
            Comun comun = new Comun();
            if (comun.HayConexionInternet())
            {
                imgHayInternet.Source = new BitmapImage(new Uri(@"/SCS/IMG/RED_INTERNET_VERDE.png", UriKind.Relative));
            }
            else
            {
                imgHayInternet.Source = new BitmapImage(new Uri(@"/SCS/IMG/RED_INTERNET_ROJO.png", UriKind.Relative));
            }
        }
        private bool HayAsignacionActiva()
        {
            bool HayAsignacionActiva = false;

            ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();
            config_varios cv_asignacion_actual = serv_conf_varios.getEntityByClave("ASIGNACION_ACTIVA");
            if (cv_asignacion_actual.valor != "")
            {
                HayAsignacionActiva = true;
            }

            return HayAsignacionActiva;
        }
        private void LimpiarAsignacionActual()
        {
            // ACTUALIZAR EN LA BASE DE DATOS CON EL USUARIO ACTUAL CONECTADO
            ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();
            config_varios cv_asignacion_actual = new config_varios();
            cv_asignacion_actual.clave = "ASIGNACION_ACTIVA";
            cv_asignacion_actual.valor = "";
            serv_conf_varios.updEntityByClave(cv_asignacion_actual);
        }

        #endregion



        #region VALIDACIONES CONEXION SERIAL ALCANCIA
        private bool validaPuertoCOMConfigurado_Alcancia()
        {
            bool isConfigured = false;

            ServiceConfigPuertos serv_cp = new ServiceConfigPuertos();
            ct_config_puertos cp = serv_cp.getEntityByNombreDispositivo("ALCANCIA");

            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            if (cp != null && cp.port_name != "")
            {
                isConfigured = true;
            }
            else
            {
                isConfigured = false;
            }

            return isConfigured;
        }
        private bool isValidComPortAndConnected_Alcancia()
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
                if (puertoSerie_alcancia.IsOpen == false)
                {
                    puertoSerie_alcancia.Open();
                    hayDispositivoConectado = true;
                }
                else
                {
                    hayDispositivoConectado = true;
                }
            }
            catch (Exception ex)
            {
                hayDispositivoConectado = false;
                //MessageBox.Show(ex.Message);
            }

            return hayDispositivoConectado;
        }
        public bool HayConexionPuertoSerial_Alcancia()
        {
            bool hayConexionPuertoSerial = false;

            try
            {
                if (validaPuertoCOMConfigurado_Alcancia())
                {
                    if (isValidComPortAndConnected_Alcancia())
                    {
                        //if (validaDispositivoConectadoEnPuertoCOM())
                        //{
                        hayConexionPuertoSerial = true;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                hayConexionPuertoSerial = false;
            }

            return hayConexionPuertoSerial;
        }
        private void Cambia_Imagen_Evalua_Conexion_Puerto_Serial_Alcancia()
        {
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios config_varios_alcancia = serv_cv.getEntityByClave("ALCANCIA_ACTIVA");
            if (config_varios_alcancia.valor == "HABILITADO")
            {
                Comun comun = new Comun();
                if (HayConexionPuertoSerial_Alcancia())
                {
                    imgHayConexionSerial_Alcancia.Source = new BitmapImage(new Uri(@"/SCS/IMG/ALCANCIA_ACTIVA.png", UriKind.Relative));
                }
                else
                {
                    imgHayConexionSerial_Alcancia.Source = new BitmapImage(new Uri(@"/SCS/IMG/puerto_rojo.png", UriKind.Relative));
                }
            }
            else
            {
                imgHayConexionSerial_Alcancia.Source = new BitmapImage(new Uri(@"/SCS/IMG/puerto_gris.png", UriKind.Relative));
            }


        }

        #endregion



        #region VALIDACIONES CONEXION SERIAL CUENTA COCOS
        private bool validaPuertoCOMConfigurado_CuentaCocos()
        {
            bool isConfigured = false;

            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            ServiceConfigPuertos serv_cp = new ServiceConfigPuertos();
            ct_config_puertos cp = serv_cp.getEntityByNombreDispositivo("CUENTA_COCOS");
            if (cp != null && cp.port_name != "")
            {
                isConfigured = true;
            }
            else
            {
                isConfigured = false;
            }

            return isConfigured;
        }
        private bool isValidComPortAndConnected_CuentaCocos()
        {
            bool isValid;
            try
            {
                open_serial_port_cuenta_cocos();
                isValid = true;
            }
            catch (Exception ex)
            {
                isValid = false;
            }
            return isValid;
        }
        public bool HayConexionPuertoSerial_CuentaCocos()
        {
            bool hayConexionPuertoSerial = false;

            try
            {
                if (validaPuertoCOMConfigurado_CuentaCocos())
                {
                    if (isValidComPortAndConnected_CuentaCocos())
                    {
                        //if (validaDispositivoConectadoEnPuertoCOM())
                        //{
                        hayConexionPuertoSerial = true;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                hayConexionPuertoSerial = false;
            }

            return hayConexionPuertoSerial;
        }
        private void Cambia_Imagen_Evalua_Conexion_Puerto_Serial_CuentaCocos()
        {
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios config_varios_cc1 = serv_cv.getEntityByClave("CC1_ACTIVO");
            config_varios config_varios_cc2 = serv_cv.getEntityByClave("CC2_ACTIVO");
            config_varios config_varios_cc3 = serv_cv.getEntityByClave("CC3_ACTIVO");
            if (config_varios_cc1.valor == "HABILITADO" || config_varios_cc2.valor == "HABILITADO" || config_varios_cc3.valor == "HABILITADO")
            {
                Comun comun = new Comun();
                if (HayConexionPuertoSerial_CuentaCocos())
                {
                    imgHayConexionSerial_CuentaCocos.Source = new BitmapImage(new Uri(@"/SCS/IMG/CUENTACOCOS_ACTIVO.png", UriKind.Relative));
                }
                else
                {
                    imgHayConexionSerial_CuentaCocos.Source = new BitmapImage(new Uri(@"/SCS/IMG/puerto_rojo.png", UriKind.Relative));
                }

            }
            else
            {
                imgHayConexionSerial_CuentaCocos.Source = new BitmapImage(new Uri(@"/SCS/IMG/puerto_gris.png", UriKind.Relative));
            }
                
        }
        #endregion


        #region VALIDACIONES CONEXION SERIAL GPS
        private bool validaPuertoCOMConfigurado_GPS()
        {
            bool isConfigured = false;

            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            ServiceConfigPuertos serv_cp = new ServiceConfigPuertos();
            ct_config_puertos cp = serv_cp.getEntityByNombreDispositivo("GPS");
            if (cp != null && cp.port_name != "")
            {
                isConfigured = true;
            }
            else
            {
                isConfigured = false;
            }

            return isConfigured;
        }
        private bool isValidComPortAndConnected_GPS()
        {
            bool isValid;
            try
            {
                open_serial_port_gps();
                isValid = true;
            }
            catch (Exception ex)
            {
                isValid = false;
            }
            return isValid;
        }
        public bool HayConexionPuertoSerial_GPS()
        {
            bool hayConexionPuertoSerial = false;

            try
            {
                if (validaPuertoCOMConfigurado_GPS())
                {
                    if (isValidComPortAndConnected_GPS())
                    {
                        //if (validaDispositivoConectadoEnPuertoCOM())
                        //{
                        hayConexionPuertoSerial = true;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                hayConexionPuertoSerial = false;
            }

            return hayConexionPuertoSerial;
        }
        private void Cambia_Imagen_Evalua_Conexion_Puerto_Serial_GPS()
        {
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios config_varios_actual = serv_cv.getEntityByClave("UBICACION_ACTIVA");
            if (config_varios_actual.valor == "HABILITADO")
            {
                Comun comun = new Comun();
                if (HayConexionPuertoSerial_GPS())
                {
                    imgHayConexionSerial_GPS.Source = new BitmapImage(new Uri(@"/SCS/IMG/gps.png", UriKind.Relative));
                }
                else
                {
                    imgHayConexionSerial_GPS.Source = new BitmapImage(new Uri(@"/SCS/IMG/puerto_rojo.png", UriKind.Relative));
                }

            }
            else
            {
                imgHayConexionSerial_GPS.Source = new BitmapImage(new Uri(@"/SCS/IMG/puerto_gris.png", UriKind.Relative));
            }

               
        }
        #endregion




        #region METODOS CUENTA COCOS

        int cant_subidas = 0;
        int cant_bajadas = 0;
        private void Guardar_Cuenta_Cocos_DB_Local()
        {
            if (FK_ASIGNACION_ACTIVA != 0)
            {
                ServiceConfigVarios serv_cv = new ServiceConfigVarios();
                config_varios cv_cc1 = serv_cv.getEntityByClave("CC1_ACTIVO");
                config_varios cv_cc2 = serv_cv.getEntityByClave("CC2_ACTIVO");
                config_varios cv_cc3 = serv_cv.getEntityByClave("CC3_ACTIVO");

                if (cv_cc1.valor != "DESHABILITADO" || cv_cc2.valor != "DESHABILITADO" || cv_cc3.valor != "DESHABILITADO")
                {
                    sy_conteo_cuenta_cocos obj = new sy_conteo_cuenta_cocos();

                    cant_subidas = 0;
                    cant_bajadas = 0;
                    if (cv_cc1.valor == "HABILITADO")
                    {
                        if (ejecutar_commando_13_acumulado_pasajeros_cc1())
                        {
                            obj.cc1_subidas = cant_subidas;
                            obj.cc1_bajadas = cant_bajadas;
                        }
                    }
                    else
                    {
                        obj.cc1_subidas = cant_subidas;
                        obj.cc1_bajadas = cant_bajadas;
                    }

                    cant_subidas = 0;
                    cant_bajadas = 0;
                    if (cv_cc2.valor == "HABILITADO")
                    {
                        if (ejecutar_commando_13_acumulado_pasajeros_cc2())
                        {
                            obj.cc2_subidas = cant_subidas;
                            obj.cc2_bajadas = cant_bajadas;
                        }
                    }
                    else
                    {
                        obj.cc2_subidas = cant_subidas;
                        obj.cc2_bajadas = cant_bajadas;
                    }

                    cant_subidas = 0;
                    cant_bajadas = 0;
                    if (cv_cc3.valor == "HABILITADO")
                    {
                        if (ejecutar_commando_13_acumulado_pasajeros_cc3())
                        {
                            obj.cc3_subidas = cant_subidas;
                            obj.cc3_bajadas = cant_bajadas;
                        }
                    }
                    else
                    {
                        obj.cc3_subidas = cant_subidas;
                        obj.cc3_bajadas = cant_bajadas;
                    }

                    obj.fecha_hora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF");
                    obj.fkAsignacion = FK_ASIGNACION_ACTIVA;
                    obj.fkStatus = 1;

                    obj.enviado = 0;
                    obj.confirmado = 0;
                    obj.modo = 0;

                    obj.created_at = obj.fecha_hora;

                    ServiceCuentaCocos serv_cc = new ServiceCuentaCocos();
                    serv_cc.addEntity(obj);

                }
            }
            else
            {
                MessageBox.Show("DEBE SELECCIONAR UNA ASIGNACION, PARA QUE SE ENVIE CONTADOR PASAJEROS Y UBICACION GPS", "ATENCION !!!");
            }

        }
        #endregion

        #region METODOS POSICION GPS
        private void Guardar_Posicion_GPS_DB_Local()
        {
            //settings_params_gps();

            sy_posicion_gps obj = escuchar_gps_GNRMC();
            if(obj != null) 
            { 
                insertar_gps_GNRMC_db_local(obj); 
            }
            

        }

        #endregion




        #region COMANDOS - CUENTA COCOS


        private bool ejecutar_commando_12_reset_cc1()
        {
            // #02 30 30 30 31 31 32 30 30 31 33 #03 en el programa serial port monitor
            // 02  0  0  0  1  1  2  0  0  1  3  03
            bool bnd_reset_exitoso = true;
            try
            {
                decimal[] arr = new decimal[12];
                arr[0] = 2;
                arr[1] = '0';   //  1   //  DIRECCION   //  ANSII
                arr[2] = '0';   //  2   //  DIRECCION   //  ANSII
                arr[3] = '0';   //  3   //  DIRECCION   //  ANSII
                arr[4] = '1';   //  4   //  DIRECCION   //  ANSII
                arr[5] = '1';   //  5   //  COMANDO     //  ANSII
                arr[6] = '2';   //  6   //  COMANDO     //  ANSII
                arr[7] = '0';   //  7   //  TAMAÑO DATA //  ANSII
                arr[8] = '0';   //  8   //  TAMAÑO DATA //  ANSII
                arr[9] = '1';   //  11  //  CHECKSUM    //  ANSII
                arr[10] = '3';  //  12  //  CHECKSUM    //  ANSII
                arr[11] = 3;


                ClearBufferSendData_cuenta_cocos();
                ClearBufferRecievedDataGlobal_cuenta_cocos();

                for (int i = 0; i < arr.Length; i++)
                {
                    BufferSendData_cuenta_cocos[i] = decimal.ToByte(arr[i]);
                }

                open_serial_port_cuenta_cocos();
                puertoSerie_cuenta_cocos.Write(BufferSendData_cuenta_cocos, 0, 12);


                Task.WaitAll(new Task[] { Task.Delay(100) });
                puertoSerie_cuenta_cocos.Read(RecievedDataGlobal_cuenta_cocos, 0, 50);

                if (RecievedDataGlobal_cuenta_cocos.Length > 0)
                {
                    int var1 = Convert.ToInt32(
                       ((char)RecievedDataGlobal_cuenta_cocos[9]).ToString()
                        +
                       ((char)RecievedDataGlobal_cuenta_cocos[10]).ToString()
                        );

                    //if (RecievedDataGlobal_cc1[9] == '0'
                    //    && RecievedDataGlobal_cc1[10] == '6')
                    if (var1 == 6)
                    {
                        string msg = "Reset Correcto";

                    }

                }
                else
                {
                    MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando enviado");
                }

            }
            catch (Exception ex)
            {
                bnd_reset_exitoso = false;
            }

            return bnd_reset_exitoso;
        }
        private bool ejecutar_commando_13_acumulado_pasajeros_cc1()
        {
            // #02 30 30 30 31 31 32 30 30 31 33 #03 en el programa serial port monitor
            // 02  0  0  0  1  1  2  0  0  1  3  03
            bool bnd_get_acumulado_pasajeros_exitoso = true;
            try
            {
                decimal[] arr = new decimal[12];
                arr[0] = 2;
                arr[1] = '0';   //  1   //  DIRECCION   //  ANSII
                arr[2] = '0';   //  2   //  DIRECCION   //  ANSII
                arr[3] = '0';   //  3   //  DIRECCION   //  ANSII
                arr[4] = '1';   //  4   //  DIRECCION   //  ANSII
                arr[5] = '1';   //  5   //  COMANDO     //  ANSII
                arr[6] = '3';   //  6   //  COMANDO     //  ANSII
                arr[7] = '0';   //  7   //  TAMAÑO DATA //  ANSII
                arr[8] = '0';   //  8   //  TAMAÑO DATA //  ANSII
                arr[9] = '1';   //  11  //  CHECKSUM    //  ANSII
                arr[10] = '4';  //  12  //  CHECKSUM    //  ANSII
                arr[11] = 3;


                ClearBufferSendData_cuenta_cocos();
                ClearBufferRecievedDataGlobal_cuenta_cocos();

                for (int i = 0; i < arr.Length; i++)
                {
                    BufferSendData_cuenta_cocos[i] = decimal.ToByte(arr[i]);
                }

                open_serial_port_cuenta_cocos();
                puertoSerie_cuenta_cocos.Write(BufferSendData_cuenta_cocos, 0, 12);


                Task.WaitAll(new Task[] { Task.Delay(100) });

                if (puertoSerie_cuenta_cocos.BytesToRead > 0 && puertoSerie_cuenta_cocos.BytesToRead == 44)
                {
                    puertoSerie_cuenta_cocos.Read(RecievedDataGlobal_cuenta_cocos, 0, 60);

                    int var1 = Convert.ToInt32(
                       ((char)RecievedDataGlobal_cuenta_cocos[5]).ToString()
                        +
                       ((char)RecievedDataGlobal_cuenta_cocos[6]).ToString()
                        );

                    //if (RecievedDataGlobal_cc1[9] == '0'
                    //    && RecievedDataGlobal_cc1[10] == '6')
                    if (var1 == 93)
                    {
                        string msg = "Acumulado";
                        cant_subidas = 0;
                        cant_bajadas = 0;

                        int.TryParse(
                   ((char)RecievedDataGlobal_cuenta_cocos[9]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[10]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[11]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[12]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[13]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[14]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[15]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[16]).ToString()
                    , System.Globalization.NumberStyles.HexNumber, null
                    , out cant_subidas);


                        int.TryParse(
                            ((char)RecievedDataGlobal_cuenta_cocos[17]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[18]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[19]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[20]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[21]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[22]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[23]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[24]).ToString()
                        , System.Globalization.NumberStyles.HexNumber, null
                        , out cant_bajadas);



                    }

                }
                else
                {
                    //MessageBox.Show("El cuenta cocos cc1 no esta respondiendo", "ATENCION !!!");
                }
            }
            catch (Exception ex)
            {
                bnd_get_acumulado_pasajeros_exitoso = false;
                MessageBox.Show(ex.Message, "EXCEPTION !!!");
            }

            return bnd_get_acumulado_pasajeros_exitoso;
        }

        private bool ejecutar_commando_12_reset_cc2()
        {
            // #02 30 30 30 31 31 32 30 30 31 33 #03 en el programa serial port monitor
            // 02  0  0  0  1  1  2  0  0  1  3  03
            bool bnd_reset_exitoso = true;
            try
            {
                decimal[] arr = new decimal[12];
                arr[0] = 2;
                arr[1] = '0';   //  1   //  DIRECCION   //  ANSII
                arr[2] = '0';   //  2   //  DIRECCION   //  ANSII
                arr[3] = '0';   //  3   //  DIRECCION   //  ANSII
                arr[4] = '1';   //  4   //  DIRECCION   //  ANSII
                arr[5] = '1';   //  5   //  COMANDO     //  ANSII
                arr[6] = '2';   //  6   //  COMANDO     //  ANSII
                arr[7] = '0';   //  7   //  TAMAÑO DATA //  ANSII
                arr[8] = '0';   //  8   //  TAMAÑO DATA //  ANSII
                arr[9] = '1';   //  11  //  CHECKSUM    //  ANSII
                arr[10] = '3';  //  12  //  CHECKSUM    //  ANSII
                arr[11] = 3;


                ClearBufferSendData_cuenta_cocos();
                ClearBufferRecievedDataGlobal_cuenta_cocos();

                for (int i = 0; i < arr.Length; i++)
                {
                    BufferSendData_cuenta_cocos[i] = decimal.ToByte(arr[i]);
                }

                open_serial_port_cuenta_cocos();
                puertoSerie_cuenta_cocos.Write(BufferSendData_cuenta_cocos, 0, 12);


                Task.WaitAll(new Task[] { Task.Delay(100) });
                puertoSerie_cuenta_cocos.Read(RecievedDataGlobal_cuenta_cocos, 0, 50);

                if (RecievedDataGlobal_cuenta_cocos.Length > 0)
                {
                    int var1 = Convert.ToInt32(
                       ((char)RecievedDataGlobal_cuenta_cocos[9]).ToString()
                        +
                       ((char)RecievedDataGlobal_cuenta_cocos[10]).ToString()
                        );

                    //if (RecievedDataGlobal_cc2[9] == '0'
                    //    && RecievedDataGlobal_cc2[10] == '6')
                    if (var1 == 6)
                    {
                        string msg = "Reset Correcto";

                    }

                }
                else
                {
                    MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando enviado");
                }

            }
            catch (Exception ex)
            {
                bnd_reset_exitoso = false;
            }

            return bnd_reset_exitoso;
        }
        private bool ejecutar_commando_13_acumulado_pasajeros_cc2()
        {
            // #02 30 30 30 31 31 32 30 30 31 33 #03 en el programa serial port monitor
            // 02  0  0  0  1  1  2  0  0  1  3  03
            bool bnd_get_acumulado_pasajeros_exitoso = true;
            try
            {
                decimal[] arr = new decimal[12];
                arr[0] = 2;
                arr[1] = '0';   //  1   //  DIRECCION   //  ANSII
                arr[2] = '0';   //  2   //  DIRECCION   //  ANSII
                arr[3] = '0';   //  3   //  DIRECCION   //  ANSII
                arr[4] = '2';   //  4   //  DIRECCION   //  ANSII
                arr[5] = '1';   //  5   //  COMANDO     //  ANSII
                arr[6] = '3';   //  6   //  COMANDO     //  ANSII
                arr[7] = '0';   //  7   //  TAMAÑO DATA //  ANSII
                arr[8] = '0';   //  8   //  TAMAÑO DATA //  ANSII
                arr[9] = '1';   //  11  //  CHECKSUM    //  ANSII
                arr[10] = '5';  //  12  //  CHECKSUM    //  ANSII
                arr[11] = 3;


                ClearBufferSendData_cuenta_cocos();
                ClearBufferRecievedDataGlobal_cuenta_cocos();

                for (int i = 0; i < arr.Length; i++)
                {
                    BufferSendData_cuenta_cocos[i] = decimal.ToByte(arr[i]);
                }

                open_serial_port_cuenta_cocos();
                puertoSerie_cuenta_cocos.Write(BufferSendData_cuenta_cocos, 0, 12);


                Task.WaitAll(new Task[] { Task.Delay(100) });

                if (puertoSerie_cuenta_cocos.BytesToRead > 0 && puertoSerie_cuenta_cocos.BytesToRead == 44)
                {
                    puertoSerie_cuenta_cocos.Read(RecievedDataGlobal_cuenta_cocos, 0, 60);
                    int var1 = Convert.ToInt32(
                       ((char)RecievedDataGlobal_cuenta_cocos[5]).ToString()
                        +
                       ((char)RecievedDataGlobal_cuenta_cocos[6]).ToString()
                        );

                    //if (RecievedDataGlobal_cc2[9] == '0'
                    //    && RecievedDataGlobal_cc2[10] == '6')
                    if (var1 == 93)
                    {
                        string msg = "Acumulado";
                        cant_subidas = 0;
                        cant_bajadas = 0;

                        int.TryParse(
                   ((char)RecievedDataGlobal_cuenta_cocos[9]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[10]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[11]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[12]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[13]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[14]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[15]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[16]).ToString()
                    , System.Globalization.NumberStyles.HexNumber, null
                    , out cant_subidas);


                        int.TryParse(
                            ((char)RecievedDataGlobal_cuenta_cocos[17]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[18]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[19]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[20]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[21]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[22]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[23]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[24]).ToString()
                        , System.Globalization.NumberStyles.HexNumber, null
                        , out cant_bajadas);
                    }

                }
                else
                {
                    //MessageBox.Show("El cuenta cocos cc2 no esta respondiendo", "ATENCION !!!");
                }



            }
            catch (Exception ex)
            {
                bnd_get_acumulado_pasajeros_exitoso = false;
                MessageBox.Show(ex.Message, "EXCEPTION !!!");
            }

            return bnd_get_acumulado_pasajeros_exitoso;
        }

        private bool ejecutar_commando_12_reset_cc3()
        {
            // #02 30 30 30 31 31 32 30 30 31 33 #03 en el programa serial port monitor
            // 02  0  0  0  1  1  2  0  0  1  3  03
            bool bnd_reset_exitoso = true;
            try
            {
                decimal[] arr = new decimal[12];
                arr[0] = 2;
                arr[1] = '0';   //  1   //  DIRECCION   //  ANSII
                arr[2] = '0';   //  2   //  DIRECCION   //  ANSII
                arr[3] = '0';   //  3   //  DIRECCION   //  ANSII
                arr[4] = '1';   //  4   //  DIRECCION   //  ANSII
                arr[5] = '1';   //  5   //  COMANDO     //  ANSII
                arr[6] = '2';   //  6   //  COMANDO     //  ANSII
                arr[7] = '0';   //  7   //  TAMAÑO DATA //  ANSII
                arr[8] = '0';   //  8   //  TAMAÑO DATA //  ANSII
                arr[9] = '1';   //  11  //  CHECKSUM    //  ANSII
                arr[10] = '3';  //  12  //  CHECKSUM    //  ANSII
                arr[11] = 3;


                ClearBufferSendData_cuenta_cocos();
                ClearBufferRecievedDataGlobal_cuenta_cocos();

                for (int i = 0; i < arr.Length; i++)
                {
                    BufferSendData_cuenta_cocos[i] = decimal.ToByte(arr[i]);
                }

                open_serial_port_cuenta_cocos();
                puertoSerie_cuenta_cocos.Write(BufferSendData_cuenta_cocos, 0, 12);


                Task.WaitAll(new Task[] { Task.Delay(100) });
                puertoSerie_cuenta_cocos.Read(RecievedDataGlobal_cuenta_cocos, 0, 50);

                if (RecievedDataGlobal_cuenta_cocos.Length > 0)
                {
                    int var1 = Convert.ToInt32(
                       ((char)RecievedDataGlobal_cuenta_cocos[9]).ToString()
                        +
                       ((char)RecievedDataGlobal_cuenta_cocos[10]).ToString()
                        );

                    //if (RecievedDataGlobal_cc3[9] == '0'
                    //    && RecievedDataGlobal_cc3[10] == '6')
                    if (var1 == 6)
                    {
                        string msg = "Reset Correcto";

                    }

                }
                else
                {
                    MessageBox.Show("Verifique la estructura del comando enviado", "Error en comando enviado");
                }

            }
            catch (Exception ex)
            {
                bnd_reset_exitoso = false;
            }

            return bnd_reset_exitoso;
        }
        private bool ejecutar_commando_13_acumulado_pasajeros_cc3()
        {
            // #02 30 30 30 31 31 32 30 30 31 33 #03 en el programa serial port monitor
            // 02  0  0  0  1  1  2  0  0  1  3  03
            bool bnd_get_acumulado_pasajeros_exitoso = true;
            try
            {
                decimal[] arr = new decimal[12];
                arr[0] = 2;
                arr[1] = '0';   //  1   //  DIRECCION   //  ANSII
                arr[2] = '0';   //  2   //  DIRECCION   //  ANSII
                arr[3] = '0';   //  3   //  DIRECCION   //  ANSII
                arr[4] = '3';   //  4   //  DIRECCION   //  ANSII
                arr[5] = '1';   //  5   //  COMANDO     //  ANSII
                arr[6] = '3';   //  6   //  COMANDO     //  ANSII
                arr[7] = '0';   //  7   //  TAMAÑO DATA //  ANSII
                arr[8] = '0';   //  8   //  TAMAÑO DATA //  ANSII
                arr[9] = '1';   //  11  //  CHECKSUM    //  ANSII
                arr[10] = '6';  //  12  //  CHECKSUM    //  ANSII
                arr[11] = 3;


                ClearBufferSendData_cuenta_cocos();
                ClearBufferRecievedDataGlobal_cuenta_cocos();

                for (int i = 0; i < arr.Length; i++)
                {
                    BufferSendData_cuenta_cocos[i] = decimal.ToByte(arr[i]);
                }

                open_serial_port_cuenta_cocos();
                puertoSerie_cuenta_cocos.Write(BufferSendData_cuenta_cocos, 0, 12);


                Task.WaitAll(new Task[] { Task.Delay(100) });

                if (puertoSerie_cuenta_cocos.BytesToRead > 0 && puertoSerie_cuenta_cocos.BytesToRead == 44)
                {
                    puertoSerie_cuenta_cocos.Read(RecievedDataGlobal_cuenta_cocos, 0, 60);
                    int var1 = Convert.ToInt32(
                       ((char)RecievedDataGlobal_cuenta_cocos[5]).ToString()
                        +
                       ((char)RecievedDataGlobal_cuenta_cocos[6]).ToString()
                        );

                    //if (RecievedDataGlobal_cc3[9] == '0'
                    //    && RecievedDataGlobal_cc3[10] == '6')
                    if (var1 == 93)
                    {
                        string msg = "Acumulado";
                        cant_subidas = 0;
                        cant_bajadas = 0;

                        int.TryParse(
                   ((char)RecievedDataGlobal_cuenta_cocos[9]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[10]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[11]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[12]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[13]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[14]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[15]).ToString()
                    + ((char)RecievedDataGlobal_cuenta_cocos[16]).ToString()
                    , System.Globalization.NumberStyles.HexNumber, null
                    , out cant_subidas);


                        int.TryParse(
                            ((char)RecievedDataGlobal_cuenta_cocos[17]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[18]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[19]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[20]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[21]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[22]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[23]).ToString()
                        + ((char)RecievedDataGlobal_cuenta_cocos[24]).ToString()
                        , System.Globalization.NumberStyles.HexNumber, null
                        , out cant_bajadas);
                    }

                }
                else
                {
                    //MessageBox.Show("El cuenta cocos cc3 no esta respondiendo", "ATENCION !!!");
                }


            }
            catch (Exception ex)
            {
                bnd_get_acumulado_pasajeros_exitoso = false;
                MessageBox.Show(ex.Message, "EXCEPTION !!!");
            }


            return bnd_get_acumulado_pasajeros_exitoso;
        }

        #endregion

        
        #region COMANDOS - GPS - LISTENER Y CONFIGURACION
        private void reset_to_default_manufactured()
        {
            string str_reset_default = "181 98 6 9 13 0 255 255 0 0 0 0 0 0 255 255 0 0 7 31 158";

            string[] arr_reset_default_str = str_reset_default.Split(' ');
            decimal[] arr_reset_default_decimal = new decimal[21];
            for (int i = 0; i < arr_reset_default_str.Length; i++)
            {
                arr_reset_default_decimal[i] = Convert.ToDecimal(arr_reset_default_str[i]);
                BufferSendData_gps[i] = decimal.ToByte(arr_reset_default_decimal[i]);
            }
            open_serial_port_gps();
            puertoSerie_gps.Write(BufferSendData_gps, 0, 21);
        }
        private void settings_params_gps()
        {
            try
            {
                #region OBTENER CONFIG VARIOS DE GPS
                ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
                config_varios cv_gps_frecuencia_segundos = serv_config_varios.getEntityByClave("GPS_FRECUENCIA_SEGUNDOS");

                // PARAMETRIZAR PARA HABILITAR O DESHABILITAR DEL OUTPUT LOS REGISTROS
                config_varios cv_gps_GPDTM = serv_config_varios.getEntityByClave("GPS_GPDTM");
                config_varios cv_gps_GPGBS = serv_config_varios.getEntityByClave("GPS_GPGBS");
                config_varios cv_gps_GPGGA = serv_config_varios.getEntityByClave("GPS_GPGGA");
                config_varios cv_gps_GPGLL = serv_config_varios.getEntityByClave("GPS_GPGLL");
                config_varios cv_gps_GPGRS = serv_config_varios.getEntityByClave("GPS_GPGRS");
                config_varios cv_gps_GPGSA = serv_config_varios.getEntityByClave("GPS_GPGSA");
                config_varios cv_gps_GPGST = serv_config_varios.getEntityByClave("GPS_GPGST");
                config_varios cv_gps_GPGSV = serv_config_varios.getEntityByClave("GPS_GPGSV");
                config_varios cv_gps_GNRMC = serv_config_varios.getEntityByClave("GPS_GNRMC");
                config_varios cv_gps_GPVTG = serv_config_varios.getEntityByClave("GPS_GPVTG");
                config_varios cv_gps_GPZDA = serv_config_varios.getEntityByClave("GPS_GPZDA");
                #endregion

                ClearBufferSendData_gps();

                #region GPS FRECUENCIA SEGUNDOS
                //B5 62 06 08 06 00 88 13 01 00 01 00 B1 49 B5 62 06 08 00 00 0E 30
                //PARAMETRIZAR PARA QUE OBTENGA LA INFO CADA 5 SEGUNDOS DESDE EL GPS
                string str_1_seg = "181 98 6 8 6 0 232 3 1 0 1 0 1 57";
                string str_3_seg = "181 98 6 8 6 0 184 11 1 0 1 0 217 65 181 98 6 8 0 0 14 48";
                string str_5_seg = "181 98 06 08 06 00 136 19 01 00 01 00 177 73 181 98 06 08 00 00 15 48";
                string str_10_seg = "181 98 6 8 6 0 16 39 1 0 1 0 77 221 181 98 6 8 0 0 14 48";
                string str_20_seg = "181 98 6 8 6 0 32 78 1 0 1 0 132 0 181 98 6 8 0 0 14 48";

                
                int frecuencia_minutos_gps = Convert.ToInt32(cv_gps_frecuencia_segundos.valor);
                switch (frecuencia_minutos_gps)
                {
                    case 1:
                        string[] arr_1_seg_str = str_1_seg.Split(' ');
                        decimal[] arr_1_seg_decimal = new decimal[14];
                        for (int i = 0; i < arr_1_seg_str.Length; i++)
                        {
                            arr_1_seg_decimal[i] = Convert.ToDecimal(arr_1_seg_str[i]);
                            BufferSendData_gps[i] = decimal.ToByte(arr_1_seg_decimal[i]);
                        }
                        open_serial_port_gps();
                        puertoSerie_gps.Write(BufferSendData_gps, 0, 14);
                        break;
                    case 3:
                        string[] arr_3_seg_str = str_3_seg.Split(' ');
                        decimal[] arr_3_seg_decimal = new decimal[22];
                        for (int i = 0; i < arr_3_seg_str.Length; i++)
                        {
                            arr_3_seg_decimal[i] = Convert.ToDecimal(arr_3_seg_str[i]);
                            BufferSendData_gps[i] = decimal.ToByte(arr_3_seg_decimal[i]);
                        }
                        open_serial_port_gps();
                        puertoSerie_gps.Write(BufferSendData_gps, 0, 22);
                        break;
                    case 5:
                        string[] arr_5_seg_str = str_5_seg.Split(' ');
                        decimal[] arr_5_seg_decimal = new decimal[22];
                        for (int i = 0; i < arr_5_seg_str.Length; i++)
                        {
                            arr_5_seg_decimal[i] = Convert.ToDecimal(arr_5_seg_str[i]);
                            BufferSendData_gps[i] = decimal.ToByte(arr_5_seg_decimal[i]);
                        }
                        open_serial_port_gps();
                        puertoSerie_gps.Write(BufferSendData_gps, 0, 22);
                        break;
                    case 10:
                        string[] arr_10_seg_str = str_10_seg.Split(' ');
                        decimal[] arr_10_seg_decimal = new decimal[22];
                        for (int i = 0; i < arr_10_seg_str.Length; i++)
                        {
                            arr_10_seg_decimal[i] = Convert.ToDecimal(arr_10_seg_str[i]);
                            BufferSendData_gps[i] = decimal.ToByte(arr_10_seg_decimal[i]);
                        }
                        open_serial_port_gps();
                        puertoSerie_gps.Write(BufferSendData_gps, 0, 22);
                        break;
                    case 20:
                        string[] arr_20_seg_str = str_20_seg.Split(' ');
                        decimal[] arr_20_seg_decimal = new decimal[22];
                        for (int i = 0; i < arr_20_seg_str.Length; i++)
                        {
                            arr_20_seg_decimal[i] = Convert.ToDecimal(arr_20_seg_str[i]);
                            BufferSendData_gps[i] = decimal.ToByte(arr_20_seg_decimal[i]);
                        }
                        open_serial_port_gps();
                        puertoSerie_gps.Write(BufferSendData_gps, 0, 22);
                        break;
                }
                #endregion

                ClearBufferSendData_gps();

                #region GPDTM
                string str_Enable_GPDTM = "36 69 73 71 80 81 44 68 84 77 42 51 66 13 10 181 98 6 1 3 0 240 10 1 5 36";
                string str_Disable_GPDTM = "36 69 73 71 80 81 44 68 84 77 42 51 66 13 10 181 98 6 1 3 0 240 10 0 4 35";
                decimal[] arr_gps_GPDTM_decimal = new decimal[26];
                if (cv_gps_GPDTM.valor == "SI") 
                {
                    string[] arr_gps_GPDTM_str = str_Enable_GPDTM.Split(' ');
                    for (int i = 0; i < arr_gps_GPDTM_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPDTM_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPDTM_str = str_Disable_GPDTM.Split(' ');
                    for (int i = 0; i < arr_gps_GPDTM_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPDTM_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GPGBS
                string str_Enable_GPGBS = "36 69 73 71 80 81 44 71 66 83 42 51 48 13 10 181 98 6 1 3 0 240 9 1 4 34";
                string str_Disable_GPGBS = "36 69 73 71 80 81 44 71 66 83 42 51 48 13 10 181 98 6 1 3 0 240 9 0 3 33";
                decimal[] arr_gps_GPGBS_decimal = new decimal[26];
                if (cv_gps_GPGBS.valor == "SI")
                {
                    string[] arr_gps_GPGBS_str = str_Enable_GPGBS.Split(' ');
                    for (int i = 0; i < arr_gps_GPGBS_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGBS_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPGBS_str = str_Disable_GPGBS.Split(' ');
                    for (int i = 0; i < arr_gps_GPGBS_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGBS_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GPGGA
                string str_Enable_GPGGA = "36 69 73 71 80 81 44 71 71 65 42 50 55 13 10 181 98 6 1 3 0 240 0 1 251 16";
                string str_Disable_GPGGA = "36 69 73 71 80 81 44 71 71 65 42 50 55 13 10 181 98 6 1 3 0 240 0 0 250 15";
                decimal[] arr_gps_GPGGA_decimal = new decimal[26];
                if (cv_gps_GPGGA.valor == "SI")
                {
                    string[] arr_gps_GPGGA_str = str_Enable_GPGGA.Split(' ');
                    for (int i = 0; i < arr_gps_GPGGA_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGGA_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPGGA_str = str_Disable_GPGGA.Split(' ');
                    for (int i = 0; i < arr_gps_GPGGA_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGGA_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GPGLL
                string str_Enable_GPGLL = "36 69 73 71 80 81 44 71 76 76 42 50 49 13 10 181 98 6 1 3 0 240 1 1 252 18";
                string str_Disable_GPGLL = "36 69 73 71 80 81 44 71 76 76 42 50 49 13 10 181 98 6 1 3 0 240 1 0 251 17";
                decimal[] arr_gps_GPGLL_decimal = new decimal[26];
                if (cv_gps_GPGLL.valor == "SI")
                {
                    string[] arr_gps_GPGLL_str = str_Enable_GPGLL.Split(' ');
                    for (int i = 0; i < arr_gps_GPGLL_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGLL_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPGLL_str = str_Disable_GPGLL.Split(' ');
                    for (int i = 0; i < arr_gps_GPGLL_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGLL_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GPGRS
                string str_Enable_GPGRS = "36 69 73 71 80 81 44 71 82 83 42 50 48 13 10 181 98 6 1 3 0 240 6 1 1 28";
                string str_Disable_GPGRS = "36 69 73 71 80 81 44 71 82 83 42 50 48 13 10 181 98 6 1 3 0 240 6 0 0 27";
                decimal[] arr_gps_GPGRS_decimal = new decimal[26];
                if (cv_gps_GPGRS.valor == "SI")
                {
                    string[] arr_gps_GPGRS_str = str_Enable_GPGRS.Split(' ');
                    for (int i = 0; i < arr_gps_GPGRS_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGRS_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPGRS_str = str_Disable_GPGRS.Split(' ');
                    for (int i = 0; i < arr_gps_GPGRS_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGRS_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GPGSA
                string str_Disable_GPGSA = "36 69 73 71 80 81 44 71 83 65 42 51 51 13 10 181 98 6 1 3 0 240 2 0 252 19";
                string str_Enable_GPGSA = "36 69 73 71 80 81 44 71 83 65 42 51 51 13 10 181 98 6 1 3 0 240 2 1 253 20";
                decimal[] arr_gps_GPGSA_decimal = new decimal[26];
                if (cv_gps_GPGSA.valor == "SI")
                {
                    string[] arr_gps_GPGSA_str = str_Enable_GPGSA.Split(' ');
                    for (int i = 0; i < arr_gps_GPGSA_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGSA_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPGSA_str = str_Disable_GPGSA.Split(' ');
                    for (int i = 0; i < arr_gps_GPGSA_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGSA_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GPGST
                string str_Disable_GPGST = "36 69 73 71 80 81 44 71 83 84 42 50 54 13 10 181 98 6 1 3 0 240 7 0 1 29";
                string str_Enable_GPGST = "36 69 73 71 80 81 44 71 83 84 42 50 54 13 10 181 98 6 1 3 0 240 7 1 2 30";
                decimal[] arr_gps_GPGST_decimal = new decimal[26];
                if (cv_gps_GPGST.valor == "SI")
                {
                    string[] arr_gps_GPGST_str = str_Enable_GPGST.Split(' ');
                    for (int i = 0; i < arr_gps_GPGST_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGST_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPGST_str = str_Disable_GPGST.Split(' ');
                    for (int i = 0; i < arr_gps_GPGST_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGST_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GPGSV
                string str_Disable_GPGSV = "36 69 73 71 80 81 44 71 83 86 42 50 52 13 10 181 98 6 1 3 0 240 3 0 253 21";
                string str_Enable_GPGSV = "36 69 73 71 80 81 44 71 83 86 42 50 52 13 10 181 98 6 1 3 0 240 3 1 254 22";
                decimal[] arr_gps_GPGSV_decimal = new decimal[26];
                if (cv_gps_GPGSV.valor == "SI")
                {
                    string[] arr_gps_GPGSV_str = str_Enable_GPGSV.Split(' ');
                    for (int i = 0; i < arr_gps_GPGSV_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGSV_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPGSV_str = str_Disable_GPGSV.Split(' ');
                    for (int i = 0; i < arr_gps_GPGSV_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPGSV_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GNRMC
                string str_Disable_GNRMC = "36 69 73 71 80 81 44 82 77 67 42 51 65 13 10 181 98 6 1 3 0 240 4 0 254 23";
                string str_Enable_GNRMC = "36 69 73 71 80 81 44 82 77 67 42 51 65 13 10 181 98 6 1 3 0 240 4 1 255 24";
                decimal[] arr_gps_GNRMC_decimal = new decimal[26];
                if (cv_gps_GNRMC.valor == "SI")
                {
                    string[] arr_gps_GNRMC_str = str_Enable_GNRMC.Split(' ');
                    for (int i = 0; i < arr_gps_GNRMC_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GNRMC_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GNRMC_str = str_Disable_GNRMC.Split(' ');
                    for (int i = 0; i < arr_gps_GNRMC_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GNRMC_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GPVTG
                string str_Disable_GPVTG = "36 69 73 71 80 81 44 86 84 71 42 50 51 13 10 181 98 6 1 3 0 240 5 0 255 25";
                string str_Enable_GPVTG = "36 69 73 71 80 81 44 86 84 71 42 50 51 13 10 181 98 6 1 3 0 240 5 1 0 26";
                decimal[] arr_gps_GPVTG_decimal = new decimal[26];
                if (cv_gps_GPVTG.valor == "SI")
                {
                    string[] arr_gps_GPVTG_str = str_Enable_GPVTG.Split(' ');
                    for (int i = 0; i < arr_gps_GPVTG_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPVTG_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPVTG_str = str_Disable_GPVTG.Split(' ');
                    for (int i = 0; i < arr_gps_GPVTG_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPVTG_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

                ClearBufferSendData_gps();

                #region GPZDA
                string str_Disable_GPZDA = "36 69 73 71 80 81 44 90 68 65 42 51 57 13 10 181 98 6 1 3 0 240 8 0 2 31";
                string str_Enable_GPZDA = "36 69 73 71 80 81 44 90 68 65 42 51 57 13 10 181 98 6 1 3 0 240 8 1 3 32";
                decimal[] arr_gps_GPZDA_decimal = new decimal[26];
                if (cv_gps_GPZDA.valor == "SI")
                {
                    string[] arr_gps_GPZDA_str = str_Enable_GPZDA.Split(' ');
                    for (int i = 0; i < arr_gps_GPZDA_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPZDA_str[i]));
                    }
                }
                else
                {
                    string[] arr_gps_GPZDA_str = str_Disable_GPZDA.Split(' ');
                    for (int i = 0; i < arr_gps_GPZDA_str.Length; i++)
                    {
                        BufferSendData_gps[i] = decimal.ToByte(Convert.ToDecimal(arr_gps_GPZDA_str[i]));
                    }
                }
                open_serial_port_gps();
                puertoSerie_gps.Write(BufferSendData_gps, 0, 26);
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private sy_posicion_gps escuchar_gps_GNRMC()
        {
            sy_posicion_gps obj = new sy_posicion_gps();
            try
            {
                //for (int i = 1; i <= 1; i++)
                //{
                string res = puertoSerie_gps.ReadLine();
                if (res.Contains("GNRMC"))
                {
                    // $GNRMC,205832.00,A,2040.26072,N,10322.86300,W,1.051,,021024,,,A*7A

                    string[] arrGNRMC = new string[8];
                    arrGNRMC = res.Split(',');

                    if (arrGNRMC[2].ToString().Trim() == "A")
                    {
                        // LAT
                        if (arrGNRMC[3] != null && arrGNRMC[3].ToString().Trim() != "")
                        {
                            string[] arrLat = new string[2];
                            arrLat = arrGNRMC[3].Split('.');

                            arrLat[0] = Convert.ToInt32(arrLat[0]).ToString("D5");
                            arrLat[1] = Convert.ToInt32(arrLat[1]).ToString("D5");

                            //dddmm.mmmmm
                            string dd_izq = arrLat[0].ToString().Trim().Substring(0, 3);
                            string mm_izq = arrLat[0].ToString().Trim().Substring(3, 2);
                            string mm_der = arrLat[1].ToString().Trim();

                            decimal parte_DECIMAL = (Convert.ToDecimal(mm_izq + "." + mm_der) / 60);

                            // CONVERSION A COMO LO TOMA GOOGLE MAPS
                            decimal izq_der_FINAL_LAT = Convert.ToDecimal(dd_izq) + parte_DECIMAL;

                            double aux_value = Math.Pow(10, 5);
                            decimal value = (Math.Truncate(Convert.ToDecimal(izq_der_FINAL_LAT) * Convert.ToDecimal(aux_value)) / Convert.ToDecimal(aux_value));
                            string izq_der_str_FINAL_LAT = value.ToString();

                            if (arrGNRMC[4].Equals("S")) { izq_der_str_FINAL_LAT = (Convert.ToDecimal(izq_der_str_FINAL_LAT) * -1).ToString(); }

                            Console.WriteLine(izq_der_str_FINAL_LAT);
                            obj.lat = izq_der_str_FINAL_LAT;
                        }

                        // LNG
                        if (arrGNRMC[5] != null && arrGNRMC[5].ToString().Trim() != "")
                        {
                            string[] arrLng = new string[2];
                            arrLng = arrGNRMC[5].Split('.');

                            arrLng[0] = Convert.ToInt32(arrLng[0]).ToString("D5");
                            arrLng[1] = Convert.ToInt32(arrLng[1]).ToString("D5");

                            //dddmm.mmmmm
                            string dd_izq = arrLng[0].ToString().Trim().Substring(0, 3);
                            string mm_izq = arrLng[0].ToString().Trim().Substring(3, 2);
                            string mm_der = arrLng[1].ToString().Trim();

                            decimal parte_DECIMAL = (Convert.ToDecimal(mm_izq + "." + mm_der) / 60);

                            // CONVERSION A COMO LO TOMA GOOGLE MAPS
                            decimal izq_der_FINAL_LNG = Convert.ToDecimal(dd_izq) + parte_DECIMAL;

                            double aux_value = Math.Pow(10, 5);
                            decimal value = (Math.Truncate(Convert.ToDecimal(izq_der_FINAL_LNG) * Convert.ToDecimal(aux_value)) / Convert.ToDecimal(aux_value));
                            string izq_der_str_FINAL_LNG = value.ToString();

                            if (arrGNRMC[6].Equals("W")) { izq_der_str_FINAL_LNG = (Convert.ToDecimal(izq_der_str_FINAL_LNG) * -1).ToString(); }

                            Console.WriteLine(izq_der_str_FINAL_LNG);
                            obj.lng = izq_der_str_FINAL_LNG;
                        }

                        // FECHA
                        string fecha_gps = "";
                        if (arrGNRMC[9] != null && arrGNRMC[9].ToString().Trim() != "")
                        {
                            fecha_gps = "20" + arrGNRMC[9].Substring(4, 2) + "-" + arrGNRMC[9].Substring(2, 2) + "-" + arrGNRMC[9].Substring(0, 2);
                        }
                        // HORA
                        string hora_gps = "";
                        if (arrGNRMC[1] != null && arrGNRMC[1].ToString().Trim() != "")
                        {
                            hora_gps = arrGNRMC[1].Substring(0, 2) + ":" + arrGNRMC[1].Substring(2, 2) + ":" + arrGNRMC[1].Substring(4, 2);
                        }

                        string fecha_hora_gps = fecha_gps + " " + hora_gps;
                        string fecha_hora_gps_correcta = (Convert.ToDateTime(fecha_hora_gps).AddHours(-6)).ToString("yyyy-MM-dd HH:mm:ss.fff");

                            
                        obj.fkAsignacion = FK_ASIGNACION_ACTIVA;
                        obj.fkStatus = 1;
                        obj.fecha_hora = fecha_hora_gps_correcta; //fecha_hora_gps;
                        obj.enviado = 0;
                        obj.confirmado = 0;
                        obj.modo = 0;
                        obj.created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                        obj.updated_at = null;
                        obj.deleted_at = null;


                    }
                    //else
                    //{
                    //    Console.WriteLine("AUN NO CALCULA LA UBICACION EL GPS GNRMC");
                    //}
                        
                }
                    
                //}
                return obj;
            }
            catch (Exception ex)
            {
                return obj;

            }
        }
        private void insertar_gps_GNRMC_db_local(sy_posicion_gps obj)
        {
            ServicePosicionGPS serv_pos_gps = new ServicePosicionGPS();
            if(obj.fkAsignacion != null && obj.fkAsignacion != 0)
            {
                serv_pos_gps.addEntity(obj);
            }
            
        }


        private void escuchar_gps_GNGLL()
        {
            try
            {
                string cadena_gps = "";
                for (int i = 1; i <= 10; i++)
                {
                    string res = puertoSerie_gps.ReadLine();
                    if (res.Contains("GNGLL"))
                    {
                        // $GNGLL,2040.21545,N,10322.85736,W,214904.00,A,A*63

                        string[] arrGNGLL = new string[8];
                        arrGNGLL = res.Split(',');

                        if (arrGNGLL[6].ToString().Trim() == "A")
                        {
                            if (arrGNGLL[1] != null && arrGNGLL[1].ToString().Trim() != "")
                            {
                                string[] arrLat = new string[2];
                                arrLat = arrGNGLL[1].Split('.');

                                arrLat[0] = Convert.ToInt32(arrLat[0]).ToString("D5");
                                arrLat[1] = Convert.ToInt32(arrLat[1]).ToString("D5");

                                //dddmm.mmmmm
                                string dd_izq = arrLat[0].ToString().Trim().Substring(0, 3);
                                string mm_izq = arrLat[0].ToString().Trim().Substring(3, 2);
                                string mm_der = arrLat[1].ToString().Trim();

                                decimal parte_DECIMAL = (Convert.ToDecimal(mm_izq + "." + mm_der) / 60);

                                // CONVERSION A COMO LO TOMA GOOGLE MAPS
                                decimal izq_der_FINAL_LAT = Convert.ToDecimal(dd_izq) + parte_DECIMAL;

                                double aux_value = Math.Pow(10, 5);
                                decimal value = (Math.Truncate(Convert.ToDecimal(izq_der_FINAL_LAT) * Convert.ToDecimal(aux_value)) / Convert.ToDecimal(aux_value));
                                string izq_der_str_FINAL_LAT = value.ToString();

                                if (arrGNGLL[2].Equals("S")) { izq_der_str_FINAL_LAT = (Convert.ToDecimal(izq_der_str_FINAL_LAT) * -1).ToString(); }


                                Console.WriteLine(izq_der_str_FINAL_LAT);
                            }

                            if (arrGNGLL[3] != null && arrGNGLL[3].ToString().Trim() != "")
                            {
                                string[] arrLng = new string[2];
                                arrLng = arrGNGLL[3].Split('.');

                                arrLng[0] = Convert.ToInt32(arrLng[0]).ToString("D5");
                                arrLng[1] = Convert.ToInt32(arrLng[1]).ToString("D5");

                                //dddmm.mmmmm
                                string dd_izq = arrLng[0].ToString().Trim().Substring(0, 3);
                                string mm_izq = arrLng[0].ToString().Trim().Substring(3, 2);
                                string mm_der = arrLng[1].ToString().Trim();

                                decimal parte_DECIMAL = (Convert.ToDecimal(mm_izq + "." + mm_der) / 60);

                                // CONVERSION A COMO LO TOMA GOOGLE MAPS
                                decimal izq_der_FINAL_LNG = Convert.ToDecimal(dd_izq) + parte_DECIMAL;

                                double aux_value = Math.Pow(10, 5);
                                decimal value = (Math.Truncate(Convert.ToDecimal(izq_der_FINAL_LNG) * Convert.ToDecimal(aux_value)) / Convert.ToDecimal(aux_value));
                                string izq_der_str_FINAL_LNG = value.ToString();

                                if (arrGNGLL[4].Equals("W")) { izq_der_str_FINAL_LNG = (Convert.ToDecimal(izq_der_str_FINAL_LNG) * -1).ToString(); }

                                Console.WriteLine(izq_der_str_FINAL_LNG);

                            }
                        }
                        else
                        {
                            Console.WriteLine("AUN NO CALCULA LA UBICACION EL GPS GNGLL");
                        }

                        //cadena_gps = res;
                    }
                }


            }
            catch (Exception ex)
            {


            }
        }

        #endregion



        #region PUERTO SERIAL - ALCANCIA
        public void configura_puerto_serial()
        {
            try
            {
                //ServiceConfigVarios scv = new ServiceConfigVarios();
                //config_varios cv_port_name = scv.getEntityByClave("PORT_NAME");
                //config_varios cv_baud_rate = scv.getEntityByClave("BAUD_RATE");
                //config_varios cv_paridad = scv.getEntityByClave("PARIDAD");
                //config_varios cv_data_bits = scv.getEntityByClave("DATA_BITS");
                //config_varios cv_stop_bits = scv.getEntityByClave("STOP_BITS");
                //config_varios cv_handshake = scv.getEntityByClave("HANDSHAKE");

                ServiceConfigPuertos scp = new ServiceConfigPuertos();
                ct_config_puertos config_puerto = scp.getEntityByNombreDispositivo("ALCANCIA");
                if (config_puerto != null)
                {
                    string cv_port_name = config_puerto.port_name;
                    string cv_baud_rate = config_puerto.baud_rate;
                    string cv_paridad = config_puerto.paridad;
                    string cv_data_bits = config_puerto.data_bits;
                    string cv_stop_bits = config_puerto.stop_bits;
                    string cv_handshake = config_puerto.handshake;

                    if (cv_port_name != "")
                    {
                        this.puertoSerie_alcancia = new System.IO.Ports.SerialPort
                        ("" + cv_port_name
                        , Convert.ToInt32(cv_baud_rate)
                        , cv_paridad == "NONE" ? System.IO.Ports.Parity.None : System.IO.Ports.Parity.Mark
                        , Convert.ToInt32(cv_data_bits)
                        , Convert.ToInt32(cv_stop_bits) == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.None
                        );
                        puertoSerie_alcancia.Handshake = cv_handshake == "NONE" ? System.IO.Ports.Handshake.None : System.IO.Ports.Handshake.XOnXOff;
                    }
                }
                    
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
                if (puertoSerie_alcancia.IsOpen == false)
                {
                    puertoSerie_alcancia.Open();
                }
            }
            catch (Exception ex)
            {
                configura_puerto_serial();

            }
        }
        private void close_serial_port()
        {
            if (puertoSerie_alcancia.IsOpen == true)
            {
                puertoSerie_alcancia.Close();
            }
        }

        #endregion

        #region PUERTO SERIAL - CUENTA COCOS
        void ClearBufferSendData_cuenta_cocos()
        {
            for (int i = 0; i < BufferSendData_cuenta_cocos.Length; i++)
            {
                BufferSendData_cuenta_cocos[i] = 0;
            }
        }
        void ClearBufferRecievedDataGlobal_cuenta_cocos()
        {
            for (int i = 0; i < RecievedDataGlobal_cuenta_cocos.Length; i++)
            {
                RecievedDataGlobal_cuenta_cocos[i] = 0;
            }
        }
        public void configura_puerto_serial_cuenta_cocos()
        {
            try
            {
                ServiceConfigPuertos scp = new ServiceConfigPuertos();
                ct_config_puertos config_puerto = scp.getEntityByNombreDispositivo("CUENTA_COCOS");
                if(config_puerto != null)
                {
                    string cv_port_name = config_puerto.port_name;
                    string cv_baud_rate = config_puerto.baud_rate;
                    string cv_paridad = config_puerto.paridad;
                    string cv_data_bits = config_puerto.data_bits;
                    string cv_stop_bits = config_puerto.stop_bits;
                    string cv_handshake = config_puerto.handshake;

                    if (cv_port_name != "")
                    {
                        this.puertoSerie_cuenta_cocos = new System.IO.Ports.SerialPort
                        ("" + cv_port_name
                        , Convert.ToInt32(cv_baud_rate)
                        , cv_paridad == "NONE" ? System.IO.Ports.Parity.None : System.IO.Ports.Parity.Mark
                        , Convert.ToInt32(cv_data_bits)
                        , Convert.ToInt32(cv_stop_bits) == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.None
                        );
                        puertoSerie_cuenta_cocos.Handshake = cv_handshake == "NONE" ? System.IO.Ports.Handshake.None : System.IO.Ports.Handshake.XOnXOff;

                        close_serial_port_cuenta_cocos();
                        open_serial_port_cuenta_cocos(); //EMD 2024-05-06
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Verifique" + System.Environment.NewLine + "- Alimentación" + System.Environment.NewLine + "- Conexión del puerto", "Error de puerto COMM");
            }
        }
        private void open_serial_port_cuenta_cocos()
        {
            try
            {
                if (puertoSerie_cuenta_cocos.IsOpen == false)
                {
                    puertoSerie_cuenta_cocos.Open();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void close_serial_port_cuenta_cocos()
        {
            if (puertoSerie_cuenta_cocos.IsOpen == true)
            {
                puertoSerie_cuenta_cocos.Close();
            }
        }
        private void dispose_serial_port_cuenta_cocos()
        {
            if (puertoSerie_cuenta_cocos.IsOpen == false)
            {
                puertoSerie_cuenta_cocos.Dispose();
            }
        }
        #endregion


        #region PUERTO SERIAL - GPS
        void ClearBufferSendData_gps()
        {
            for (int i = 0; i < BufferSendData_gps.Length; i++)
            {
                BufferSendData_gps[i] = 0;
            }
        }
        void ClearBufferRecievedDataGlobal_gps()
        {
            for (int i = 0; i < RecievedDataGlobal_gps.Length; i++)
            {
                RecievedDataGlobal_gps[i] = 0;
            }
        }
        public void configura_puerto_serial_gps()
        {
            try
            {
                ServiceConfigPuertos scp = new ServiceConfigPuertos();
                ct_config_puertos config_puerto = scp.getEntityByNombreDispositivo("GPS");
                if (config_puerto != null)
                {
                    string cv_port_name = config_puerto.port_name;
                    string cv_baud_rate = config_puerto.baud_rate;
                    string cv_paridad = config_puerto.paridad;
                    string cv_data_bits = config_puerto.data_bits;
                    string cv_stop_bits = config_puerto.stop_bits;
                    string cv_handshake = config_puerto.handshake;

                    if (cv_port_name != "")
                    {
                        this.puertoSerie_gps = new System.IO.Ports.SerialPort
                        ("" + cv_port_name
                        , Convert.ToInt32(cv_baud_rate)
                        , cv_paridad == "NONE" ? System.IO.Ports.Parity.None : System.IO.Ports.Parity.Mark
                        , Convert.ToInt32(cv_data_bits)
                        , Convert.ToInt32(cv_stop_bits) == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.None
                        );
                        puertoSerie_gps.Handshake = cv_handshake == "NONE" ? System.IO.Ports.Handshake.None : System.IO.Ports.Handshake.XOnXOff;

                        close_serial_port_gps();
                        open_serial_port_gps(); //EMD 2024-05-06
                    }
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Verifique" + System.Environment.NewLine + "- Alimentación" + System.Environment.NewLine + "- Conexión del puerto", "Error de puerto COMM");
            }
        }
        private void open_serial_port_gps()
        {
            try
            {
                if (puertoSerie_gps.IsOpen == false)
                {
                    puertoSerie_gps.Open();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void close_serial_port_gps()
        {
            if (puertoSerie_gps.IsOpen == true)
            {
                puertoSerie_gps.Close();
            }
        }
        private void dispose_serial_port_gps()
        {
            if (puertoSerie_gps.IsOpen == false)
            {
                puertoSerie_gps.Dispose();
            }
        }
        #endregion
    }
}
