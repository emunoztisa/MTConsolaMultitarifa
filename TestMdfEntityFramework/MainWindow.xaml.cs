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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestMdfEntityFramework.Clases;
using TestMdfEntityFramework.Controllers;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_OnLoad(object sender, RoutedEventArgs e)
        {
            //SincronizaUsuarios();
        }

        private void SincronizaUsuarios()
        {
            //Comun mc = new Comun();

            //// SE OBTIENEN LOS USUARIOS DESDE EL SERVICIO
            //UserController uc = new UserController();
            //List<User> users = uc.GetUsers();

            //ServiceUsers servicio_users = new ServiceUsers();

            //for (int i = 0; i < users.Count; i++)
            //{
            //    //INSERTAR
            //    users us = new users();
            //    us.pkUser = i + 1;
            //    us.user = users[i].email;
            //    us.contrasena = mc.EncriptarCadena(users[i].electoralid);
            //    us.created_at = users[i].created_at;
            //    us.updated_at = users[i].updated_at;
            //    us.deleted_at = users[i].deleted_at;

            //    try
            //    {
            //        servicio_users.addEntity(us);
            //        //MessageBox.Show("Users Sincronizados con Exito");
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, "ATENCION");

            //    }

            //}

        }
    }
}
