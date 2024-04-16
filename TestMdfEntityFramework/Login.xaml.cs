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
using System.Windows.Shapes;
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

        public Login()
        {
            InitializeComponent();
            SettearValoresProduccion();
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
                        ResLogin resLogin = lc.login(usuario, password_desencriptado);
                        if (resLogin.GetToken() != null && resLogin.GetToken() != "")
                        {
                            user_actual.token = resLogin.token;
                            val = true;
                            serv_users.updEntity(user_actual);
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

        private void LimpiarCamposTexto()
        {
            txtUsuario.Text = "";
            txtContrasena.Password = "";
        }

        #endregion

        #region EVENTOS DE CONTROLES
        private void Login_OnLoad(object sender, RoutedEventArgs e)
        {
            //SINCRONIZAR USUARIOS HACIA LA BASE DE DATOS LOCAL  --  ejemplo: MT_OPE_XXXXX
            SincronizaUsuarios();

            //EVENTOS PARA POPUP OK
            SetPopupDlgCenter();
            InitializeAnimations();
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
                Principal prin = new Principal();
                this.Close();
                prin.Show();
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
            LimpiarCamposTexto();
        }
        private void popupGrid_TouchDown(object sender, TouchEventArgs e)
        {
            ocultarPopupOk();
            LimpiarCamposTexto();
        }
        #endregion

    }
}
