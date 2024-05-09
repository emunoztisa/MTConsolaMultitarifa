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

namespace TestMdfEntityFramework
{
    /// <summary>
    /// Lógica de interacción para Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        string ASIGNACION_ACTIVA = "";
        long FK_ASIGNACION_ACTIVA = 0;
        string MODO_APP = "";

        DispatcherTimer timerReloj = new DispatcherTimer();
        DispatcherTimer timerEvaluaMensajes = new DispatcherTimer();
        DispatcherTimer timerSincroniza = new DispatcherTimer();

        public Principal()
        {
            InitializeComponent();
            btnInicio_Click(null, null);
            
            cargar_logo_aplicacion();
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


        private void btnResetPortName_Click(object sender, RoutedEventArgs e)
        {
            ServiceConfigVarios serv_config_varios = new ServiceConfigVarios();
            config_varios cv_port_name = serv_config_varios.getEntityByClave("PORT_NAME");
            if (cv_port_name.valor != null && cv_port_name.valor != "")
            {
                cv_port_name.valor = "";
                serv_config_varios.updEntityByClave(cv_port_name);
            }
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
        private void dispatcherTimerReloj_Tick(object sender, EventArgs e)
        {
            try
            {
                lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
                lblReloj.Text = DateTime.Now.ToString("hh:mm:ss tt");
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

                //Envio de la ubicacion actual de la unidad en latitud y longitud
                ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();
                config_varios cv_ubicacion = serv_conf_varios.getEntityByClave("UBICACION_ACTIVA");
                if(cv_ubicacion.valor == "HABILITADO")
                {
                    Sincronizar_Ubicacion();
                }
                
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
        private void detiene_timers()
        {
            timerReloj.IsEnabled = false;
            timerEvaluaMensajes.IsEnabled = false;
            timerSincroniza.IsEnabled = false;
        }

        #endregion

        #region EVENTOS CONTROLES
        private void Principal_OnLoad(object sender, RoutedEventArgs e)
        {

            inicializa_timer_reloj();
            inicializa_timer_evalua_mensajes();
            inicializa_timer_sincroniza();

            Comun comun = new Comun();
            if (comun.HayConexionInternet())
            {
                SincronizarCatalogos();
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


        }

        private void validar_tipo_usuario()
        {
            ServiceConfigVarios serv_config_varios_user = new ServiceConfigVarios();
            config_varios cv_user = serv_config_varios_user.getEntityByClave("USUARIO_ACTUAL");

            ServiceUsers serv_users = new ServiceUsers();
            users obj_user_actual = serv_users.getEntityByUser(cv_user.valor);

            if(obj_user_actual.tipo_usuario == "INSTALL_CONFIG")
            {
                btnConfiguraciones.IsEnabled = true;
                btnConfiguraciones.Visibility = Visibility.Visible;
            }
            else
            {
                btnConfiguraciones.IsEnabled = false;
                btnConfiguraciones.Visibility = Visibility.Hidden;
            }
        }

        private void Cambia_Imagen_Evalua_Conexion_Internet()
        {
            //Sincronizar los usuarios desde TISA hacia la CONSOLA
            Comun comun = new Comun();
            if (comun.HayConexionInternet())
            {
                imgHayInternet.Source = new BitmapImage(new Uri(@"/SCS/IMG/conectado.png", UriKind.Relative));
            }
            else
            {
                imgHayInternet.Source = new BitmapImage(new Uri(@"/SCS/IMG/desconectado.png", UriKind.Relative));
            }
        }

        private void Principal_OnUnLoad(object sender, RoutedEventArgs e)
        {
            LimpiarUsuarioActualLogueado();

            detiene_timers();

            LimpiarAsignacionActual();

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

        private void LimpiarAsignacionActual()
        {
            // ACTUALIZAR EN LA BASE DE DATOS CON EL USUARIO ACTUAL CONECTADO
            ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();
            config_varios cv_asignacion_actual = new config_varios();
            cv_asignacion_actual.clave = "ASIGNACION_ACTIVA";
            cv_asignacion_actual.valor = "";
            serv_conf_varios.updEntityByClave(cv_asignacion_actual);
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
            DataContext = new Configuracionv2();
        }
        private void btnCobroTarifa_Click(object sender, RoutedEventArgs e)
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
                    if (cv.valor == "MULTITARIFA")
                    {
                        DataContext = new CobroMultitarifaV1();
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

        private void btnReportes_Click(object sender, RoutedEventArgs e)
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
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
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
        }
        private void Sincroniza_Tarifas()
        {
            //Se obtienen del endpoint
            TarifasController uc = new TarifasController();
            List<sy_tarifas> list = uc.GetTarifas();

            //Se insertan en la base local
            ServiceTarifas serv = new ServiceTarifas();

            if ((serv.getEntities()).Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (serv.getEntity(list[i].pkTarifa) != null)
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
        }


        private void Sincroniza_Denominacines()
        {
            //Se obtienen las unidades del endpoint
            DenominacionesController uc = new DenominacionesController();
            List<ct_denominaciones> list = uc.GetDenominacionesPorUnidad(); //PENDIENTE

            //Se insertan las unidades en la base local
            ServiceDenominaciones serv = new ServiceDenominaciones();


            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkDenominacion) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }
        }

        private void Sincroniza_TarifasMontosFijos()
        {
            //Se obtienen las unidades del endpoint
            TarifasMontosFijosController uc = new TarifasMontosFijosController();
            List<ct_tarifas_montos_fijos> list = uc.GetTarifasMontosFijosPorUnidad(); //PENDIENTE

            //Se insertan las unidades en la base local
            ServiceTarifasMontosFijos serv = new ServiceTarifasMontosFijos();


            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkTarifaMontoFijo) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }
        }

        private void Sincroniza_OpcionesGenerales()
        {
            //Se obtienen las unidades del endpoint
            OpcionesGeneralesController uc = new OpcionesGeneralesController();
            List<opciones_generales> list = uc.GetOpcionesGenerales();

            //Se insertan las unidades en la base local
            ServiceOpcionesGenerales serv = new ServiceOpcionesGenerales();


            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkOpcionGeneral) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
            }
        }

        private void Sincroniza_ConfigVarios()
        {
            //Se obtienen las unidades del endpoint
            ConfigVariosController uc = new ConfigVariosController();
            List<config_varios> list = uc.GetConfigVarios();

            //Se insertan las unidades en la base local
            ServiceConfigVarios serv = new ServiceConfigVarios();


            for (int i = 0; i < list.Count; i++)
            {
                if (serv.getEntity(list[i].pkConfigVarios) != null)
                {
                    serv.updEntity(list[i]);
                }
                else
                {
                    serv.addEntity(list[i]);
                }
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
        private void SetearVersionYCopyright()
        {
            ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();

            config_varios cv_version = serv_conf_varios.getEntityByClave("VERSION");
            string version = cv_version.valor;

            config_varios cv_copy = serv_conf_varios.getEntityByClave("COPYRIGHT");
            string copyright = cv_copy.valor;

            txtVersion.Text = version;
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
            ServiceMensajes serv_mensajes = new ServiceMensajes();
            List<sy_mensajes> list_msn_no_reproducidos = serv_mensajes.getEntityNoReproducidos();

            if (list_msn_no_reproducidos.Count > 0)
            {
                imgOpcionMensajes.Source = new BitmapImage(new Uri(@"/SCS/IMG/mensajes_rojo.png", UriKind.Relative));
            }
            else
            {
                imgOpcionMensajes.Source = new BitmapImage(new Uri(@"/SCS/IMG/mensajes.png", UriKind.Relative));
            }
        }


        #endregion


    }
}
