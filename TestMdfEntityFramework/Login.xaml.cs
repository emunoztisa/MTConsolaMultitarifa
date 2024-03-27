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
        public Login()
        {
            InitializeComponent();


            SettearValoresPruebas();
        }
        private void Login_OnLoad(object sender, RoutedEventArgs e)
        {
            //SINCRONIZAR USUARIOS HACIA LA BASE DE DATOS LOCAL  --  ejemplo: MT_OPE_XXXXX
            //SincronizaUsuarios();


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
                            //ACTUALIZAR
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

            /*
            ServiceUsers servUsers = new ServiceUsers();
            users us = new users();
            us.user = usuario;
            users user = servUsers.getEntityByUser(us);

            string password_desencriptado = "";
            if (user != null && user.user != "")
            {
                password_desencriptado = mc.DesencriptarCadena(user.contrasena);
            }
            */

            ResLogin resLogin = lc.login(usuario, password /*password_desencriptado*/);
            if (resLogin.GetToken() != null && resLogin.GetToken() != "")
            {
                val = true; 
            }
            
            return val;
        }

        #endregion


        #region EVENTOS DE BOTONES
        //EVENTOS DE BOTONES
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
                MessageBox.Show("El usuario no existe o no tiene permisos para acceder","ATENCION");
            }
        }
        #endregion

    }
}
