using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class TarifasController
    {
        private Type T;
        public List<sy_tarifas> GetTarifas()
        {
            List<sy_tarifas> list_temp = new List<sy_tarifas>();

            Api<ResTarifas> servicio = new Api<ResTarifas>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/tarifas";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResTarifas responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResTarifas));
            ResTarifas resp = responseGET_withToken;

            //foreach (ResponseLugares.Data data1 in data)
            foreach (sy_tarifas item in resp.data)
            {
                sy_tarifas reg = new sy_tarifas();
                reg.pkTarifa = item.pkTarifa;
                reg.fkLugarOrigen = item.fkLugarOrigen;
                reg.fkLugarDestino = item.fkLugarDestino;
                reg.fkPerfil = item.fkPerfil;
                reg.monto = item.monto;
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
