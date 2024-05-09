using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TestMdfEntityFramework;
using TestMdfEntityFramework.Clases;
using TestMdfEntityFramework.Controllers;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;


namespace TestMdfEntityFramework
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        //POPUP OK
        private double left, top, right, bottom, centerX, centerY;
        private DoubleAnimation bottomToCenterAnimiation, topToCenterAnimation,
            leftToCenterAnimation, rightToCenterAnimation;
        private Storyboard bottomToCenterStoryboard, topToCenterStoryboard,
            leftToCenterStoryboard, rightToCenterStoryboard;

        //TIMERS
        DispatcherTimer timerEvaluaConexionInternet = new DispatcherTimer();

        public Login()
        {
            InitializeComponent();
            SettearValoresProduccion();
            //SettearValoresPruebas();
            //SettearValoresPruebasInstallConfig();

            cargar_logo_home();
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

        private void cargar_logo_home()
        {
            //Cargar el logo home que se configuro
            ServiceConfigVarios serv_config_varios_apariencia = new ServiceConfigVarios();
            config_varios cv_apariencia_logo_home = serv_config_varios_apariencia.getEntityByClave("LOGO_HOME");

            ServiceImagenesSubidas serv_img_subidas = new ServiceImagenesSubidas();
            ct_imagenes_subidas obj_img_sub = serv_img_subidas.getEntityByName(cv_apariencia_logo_home.valor);

            ImageSource img_src = ByteToImage(obj_img_sub.imagen);

            imgLogoHome.Source = img_src;
        }
        public static ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }

        #region METODOS y FUNCIONES
        private void SincronizaUsuarios()
        {
            Comun mc = new Comun();

            // SE OBTIENEN LOS USUARIOS DESDE EL SERVICIO
            UserController uc = new UserController();
            List<User> users_list = uc.GetUsers();
            string cuenta_admin = mc.obtenerValorDeAppConfig("ADMIN_CONSOLA_ALCANCIA");

            ServiceUsers servicio_users = new ServiceUsers();
            try
            {
                for (int i = 0; i < users_list.Count; i++)
                {
                    users us = new users();
                    us.pkUser = i + 1;
                    us.user = users_list[i].email;
                    us.contrasena = mc.EncriptarCadena(users_list[i].electoralid);
                    us.created_at = users_list[i].created_at;
                    us.updated_at = users_list[i].updated_at;
                    us.deleted_at = users_list[i].deleted_at;

                    users user_existente = servicio_users.getEntityByUser(users_list[i].email);
                    if (user_existente != null && user_existente.user != "")
                    {
                        //YA EXISTE EL USUARIO EN LA DB LOCAL
                        //UPDATE
                        if (users_list[i].email != cuenta_admin)
                        {

                            //ACTUALIZAR usuario con todo el token ya seteado.
                            servicio_users.updEntity(us);
                        }
                    }
                    else
                    {
                        //NO EXISTE EL USUARIO EN LA DB LOCAL
                        //INSERTAR
                        servicio_users.addEntity(us);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ATENCION");

            }


        }
        private bool validaUsuario()
        {
            bool val = false;

            //AQUI LAS VALIDACIONES PARA SABER SI DEBE O NO ACCEDER EL USUARIO
            //if(txtUsuario.Text.ToString().Trim() == "admin" && txtContrasena.Password.ToString().Trim() == "root"){val = true;}

            Comun mc = new Comun();

            LoginController lc = new LoginController();

            string usuario = txtUsuario.Text.ToString().Trim();
            string password = txtContrasena.Password.ToString().Trim();

            ServiceUsers serv_users = new ServiceUsers();
            users user_actual = serv_users.getEntityByUser(usuario);

            if (user_actual != null && user_actual.user != "")
            {
                if(password == mc.DesencriptarCadena(user_actual.contrasena))
                {
                    // ACTUALIZAR EN LA BASE DE DATOS CON EL USUARIO ACTUAL CONECTADO
                    ServiceConfigVarios serv_conf_varios = new ServiceConfigVarios();
                    config_varios cv_usuario_actual = new config_varios();
                    cv_usuario_actual.clave = "USUARIO_ACTUAL";
                    cv_usuario_actual.valor = user_actual.user;
                    serv_conf_varios.updEntityByClave(cv_usuario_actual);

                    val = true;

                    //Obtiene el token desde TISA en caso de no tener seteado ninguno en la base de datos local.
                    if (user_actual.token == null && user_actual.token != "")
                    {
                        string password_desencriptado = mc.DesencriptarCadena(user_actual.contrasena);
                        Comun comun = new Comun();
                        if (comun.HayConexionInternet())
                        {
                            ResLogin resLogin = lc.login(usuario, password_desencriptado);
                            if (resLogin.GetToken() != null && resLogin.GetToken() != "")
                            {
                                //validacion si tiene el perfil que se requiere para poder mostrar la opcion de configuracion
                                int cont = 0;
                                foreach (var item in resLogin.perfiles)
                                {
                                    if (item.fkRole == 95 || item.fkRole == 96)
                                    {
                                        cont++;
                                    }
                                }

                                if (cont >= 2) { user_actual.tipo_usuario = "INSTALL_CONFIG"; }
                                else { user_actual.tipo_usuario = "OPERADOR"; }

                                user_actual.token = resLogin.token;
                                val = true;
                                serv_users.updEntity(user_actual);


                            }
                        }
                    }
                }
            }
            
            return val;
        }
        private void SettearValoresProduccion()
        {
            txtUsuario.Text = "";
            txtContrasena.Password = "";
            txtUsuario.Focus();
        }
        private void SettearValoresPruebas()
        {
            txtUsuario.Text = "mt_con_00001";
            txtContrasena.Password = "mt_con_00001";
            btnEntrar.Focus();
        }
        private void SettearValoresPruebasInstallConfig()
        {
            txtUsuario.Text = "mt_con_install_config";
            txtContrasena.Password = "12321";
            btnEntrar.Focus();
        }

        private void LimpiarCamposTexto()
        {
            txtUsuario.Text = "";
            txtContrasena.Password = "";
        }

        #endregion

        #region EVENTOS DE CONTROLES
        private void Login_OnLoad(object sender, RoutedEventArgs e)
        {
            inicializa_timer_evalua_mensajes();

            //SINCRONIZAR USUARIOS HACIA LA BASE DE DATOS LOCAL  --  ejemplo: MT_OPE_XXXXX
            Comun comun = new Comun();
            if (comun.HayConexionInternet())
            {
                SincronizaUsuarios();
            }
            
            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();
        }
        private void OnUnload(object sender, RoutedEventArgs e)
        {
            detiene_timers();
        }
        private void btnCerrarClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnMinimizarClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            if (validaUsuario())
            {
                Comun comun = new Comun();
                if (comun.HayConexionInternet())
                {
                    Principal prin = new Principal();
                    this.Close();
                    prin.Show();
                }
                else
                {
                    txtMensajePopup.Text = "NO HAY CONEXION INTERNET";
                    txtMensajePopup.FontSize = 10;
                    imgPopup.Source = new BitmapImage(new Uri(@"/SCS/IMG/equis_roja.png", UriKind.Relative));
                    mostrarPopupOk();
                }
            }
            else
            {
                txtMensajePopup.Text = "El usuario no existe o no tiene permisos para acceder";
                txtMensajePopup.FontSize = 10;
                imgPopup.Source = new BitmapImage(new Uri(@"/SCS/IMG/equis_roja.png", UriKind.Relative));
                mostrarPopupOk();

                //MessageBox.Show("El usuario no existe o no tiene permisos para acceder","ATENCION");
            }
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
            //LimpiarCamposTexto();
        }
        private void popupGrid_TouchDown(object sender, TouchEventArgs e)
        {
            ocultarPopupOk();
            //LimpiarCamposTexto();
        }
        #endregion

        #region TIMERS
        private void inicializa_timer_evalua_mensajes()
        {
            // INICIA TIMER QUE ESTARA ACTUALIZANDO EL VALOR DE ESTATUS
            timerEvaluaConexionInternet.Tick += new EventHandler(dispatcherTimerEvaluaMensajes_Tick);
            timerEvaluaConexionInternet.Interval = new TimeSpan(0, 0, 10);
            timerEvaluaConexionInternet.Start();
        }
        private void dispatcherTimerEvaluaMensajes_Tick(object sender, EventArgs e)
        {
            try
            {
                //Sincronizar los usuarios desde TISA hacia la CONSOLA
                Comun comun = new Comun();
                if (comun.HayConexionInternet())
                {
                    SincronizaUsuarios();
                }

                //Cambia la imagen de mensajes en la menubar de principal, segun si hay o no mensajes pendientes de reproducir.
                Cambia_Imagen_Evalua_Conexion_Internet();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EXCEPTION !!!");
            }
        }
       
        private void detiene_timers()
        {

            timerEvaluaConexionInternet.IsEnabled = false;
            
        }

        #endregion

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

    }
}
