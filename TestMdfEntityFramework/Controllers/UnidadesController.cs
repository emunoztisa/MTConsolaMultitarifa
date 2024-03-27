using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class UnidadesController
    {
        private Type T;
        public List<ct_unidades> GetUnidades()
        {
            List<ct_unidades> list_temp = new List<ct_unidades>();

            Api<ResUnidades> servicio = new Api<ResUnidades>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/unidades";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResUnidades responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResUnidades));
            ResUnidades resp = responseGET_withToken;

            //foreach (ResponseLugares.Data data1 in data)
            foreach (ct_unidades item in resp.data)
            {
                ct_unidades reg = new ct_unidades();
                reg.pkUnidad = item.pkUnidad;
                reg.fkEmpresa = item.fkEmpresa;
                reg.fkCorredor = item.fkCorredor;

                reg.nombre = item.nombre;
                reg.noSerieAVL = item.noSerieAVL;
                reg.economico = item.economico;

                reg.capacidad = item.capacidad;
                reg.validador = item.validador;
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
