using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class LugarRutaController
    {
        private Type T;

        public List<sy_lugar_ruta> GetLugaresRutas()
        {
            List<sy_lugar_ruta> list_temp = new List<sy_lugar_ruta>();

            Api<ResLugarRuta> servicio = new Api<ResLugarRuta>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/lugares_rutas";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResLugarRuta responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResLugarRuta));
            ResLugarRuta resp = responseGET_withToken;

            //foreach (ResponseLugares.Data data1 in data)
            foreach (sy_lugar_ruta item in resp.data)
            {
                sy_lugar_ruta reg = new sy_lugar_ruta();
                reg.pkLugarRuta = item.pkLugarRuta;
                reg.fkLugar = item.fkLugar;
                reg.fkRuta = item.fkRuta;
                reg.orden = item.orden;
                reg.status = item.status;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }

    }
}
