using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;

namespace TestMdfEntityFramework.Clases
{
    public class Session
    {
        private static ResLogin res_login { get; set; }

        public Session()
        {

        }

        public void SetResLogin(ResLogin _res_login)
        {
            res_login = _res_login;
        }
        public ResLogin GetResLogin()
        {
            return res_login;
        }

    }

    public class LoginInfo
    {
        private static long PK_USUARIO_LOGUEADO;

        public LoginInfo()
        {

        }

        public void SetPkUsuarioLogueado(long _pk)
        {
            PK_USUARIO_LOGUEADO = _pk;
        }
        public long GetPkUsuarioLogueado()
        {
            return PK_USUARIO_LOGUEADO;
        }
    }
}
