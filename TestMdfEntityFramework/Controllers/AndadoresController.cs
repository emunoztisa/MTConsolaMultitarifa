using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class AndadoresController
    {
        private Type T;
        public List<ct_andadores> GetAndadores()
        {
            List<ct_andadores> list_temp = new List<ct_andadores>();

            Api<ResAndadores> servicio = new Api<ResAndadores>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/andadores";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResAndadores responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResAndadores));
            ResAndadores resp = responseGET_withToken;

            //foreach (ResponseLugares.Data data1 in data)
            foreach (ct_andadores item in resp.data)
            {
                ct_andadores reg = new ct_andadores();
                reg.pkAndador = item.pkAndador;
                reg.fkLugar = item.fkLugar;
                reg.fkStatus = item.fkStatus;
                reg.nombre = item.nombre;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
    }
}
