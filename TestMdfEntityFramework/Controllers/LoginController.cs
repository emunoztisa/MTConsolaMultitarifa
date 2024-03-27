using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Requests;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    class LoginController
    {
        private string _token;
        private Type T;

        public ResLogin login(String email, String password)
        {
            Api<ResLogin> servicio = new Api<ResLogin>();
            Comun mc = new Comun();

            //Construccion objeto request login
            ReqLogin req = new ReqLogin();
            req.email = email;
            req.password = password;

            //Consumir servicio de Login
            //baseurl = //"http://backtransportistas.tarjetasintegrales.mx:806/api/v1/";
            string baseurl = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo = "login";
            ResLogin rl = (ResLogin)servicio.RequestPOST(baseurl, metodo, req, T);

            return rl;
        }
    }
}
