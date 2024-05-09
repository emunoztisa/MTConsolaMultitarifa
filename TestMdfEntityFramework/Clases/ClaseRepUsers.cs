using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Clases
{
    public class ClaseRepUsers
    {
        public string usuario  { get; set; }
        public string password { get; set; }

        public ClaseRepUsers()
        {

        }

        public ClaseRepUsers(string usuario, string password)
        {
            this.usuario = usuario;
            this.password = password;
        }

        public string GetUsuario()
        {
            return usuario;
        }
        public void SetUsuario(string val)
        {
            usuario = val;
        }

        public string GetPassword()
        {
            return password;
        }
        public void SetPassword(string val)
        {
            password = val;
        }
    }
}
