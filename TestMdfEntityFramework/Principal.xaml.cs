using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Input;
using TestMdfEntityFramework.Clases;
using TestMdfEntityFramework.Controllers;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Views;

namespace TestMdfEntityFramework
{
    /// <summary>
    /// Lógica de interacción para Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        public Principal()
        {
            InitializeComponent();
            btnInicio_Click(null, null);
        }

        #region EVENTOS CONTROLES
        private void Principal_OnLoad(object sender, RoutedEventArgs e)
        {
            SincronizarCatalogos();
            SetearVersionYCopyright();
            SincronizaOperacionConsola();

            ////////////////////////////////////////////////////////////////////////
            // ESTAS TAREAS SE EJECUTARAN EN SEGUNDO PLANO EN HILOS INDEPENDIENTES
            //SincronizacionTISA sTISA = new SincronizacionTISA();
            //sTISA.Task_Sincroniza_Boletos_y_BoletosDetalle_START();

            /*
                sTISA.Task_Sincroniza_Boletos_START();
                sTISA.Task_Sincroniza_BoletosDetalle_START();
            */

            ////////////////////////////////////////////////////////////////////////
        }
        private void Principal_OnUnLoad(object sender, RoutedEventArgs e)
        {
            LimpiarUsuarioActualLogueado();

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
            }
        }
        private void btnReportes_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new Reportes();
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



        #endregion

        #region METODOS SINCRONIZA OPERACION CONSOLA -> TISA
        private void SincronizaOperacionConsola()
        {
            SincronizacionTISA.SincronizaBoletosYBoletosDetalle();
            SincronizacionTISA.SincronizaCortes();
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

        #endregion



    }
}
