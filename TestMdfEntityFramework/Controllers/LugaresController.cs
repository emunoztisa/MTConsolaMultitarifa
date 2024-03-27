using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class LugaresController
    {
        private Type T;

        public List<ct_lugares> GetLugares()
        {
            List<ct_lugares> list_temp = new List<ct_lugares>();

            Api<ResLugares> servicio = new Api<ResLugares>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/lugares";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResLugares responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResLugares));
            ResLugares resp = responseGET_withToken;

            //foreach (ResponseLugares.Data data1 in data)
            foreach (ct_lugares item in resp.data)
            {
                ct_lugares reg = new ct_lugares();
                reg.pkLugar = item.pkLugar;
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
