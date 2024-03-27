using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class CorredoresController
    {
        private Type T;
        public List<ct_corredores> GetCorredores()
        {
            List<ct_corredores> list_temp = new List<ct_corredores>();

            Api<ResCorredores> servicio = new Api<ResCorredores>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/corredores";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResCorredores responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResCorredores));
            ResCorredores resp = responseGET_withToken;

            //foreach (ResponseLugares.Data data1 in data)
            foreach (ct_corredores item in resp.data)
            {
                ct_corredores reg = new ct_corredores();
                reg.pkCorredor = item.pkCorredor;
                reg.fkEmpresa = item.fkEmpresa;
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
