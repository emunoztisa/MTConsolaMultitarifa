using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class RutasController
    {
        private Type T;
        public List<ct_rutas> GetRutas()
        {
            List<ct_rutas> list_temp = new List<ct_rutas>();

            Api<ResRutas> servicio = new Api<ResRutas>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/rutas";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResRutas responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResRutas));
            ResRutas resp = responseGET_withToken;

            //foreach (ResponseLugares.Data data1 in data)
            foreach (ct_rutas item in resp.data)
            {
                ct_rutas reg = new ct_rutas();
                reg.pkRuta = item.pkRuta;
                reg.fkCorredor = item.fkCorredor;
                reg.nombre = item.nombre;
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
