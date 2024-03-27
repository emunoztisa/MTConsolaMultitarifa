using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class PerfilesController
    {
        private Type T;

        public List<ct_perfiles> GetPerfiles()
        {
            List<ct_perfiles> list_temp = new List<ct_perfiles>();

            Api<ResPerfiles> servicio = new Api<ResPerfiles>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/perfiles";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResPerfiles responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResPerfiles));
            ResPerfiles resp = responseGET_withToken;

            //foreach (ResponseLugares.Data data1 in data)
            foreach (ct_perfiles item in resp.data)
            {
                ct_perfiles reg = new ct_perfiles();
                reg.pkPerfil = item.pkPerfil;
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
