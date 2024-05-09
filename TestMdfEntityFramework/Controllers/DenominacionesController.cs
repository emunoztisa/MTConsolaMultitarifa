using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class DenominacionesController
    {
        private Type T;

        public List<ct_denominaciones> GetDenominaciones()
        {
            List<ct_denominaciones> list_temp = new List<ct_denominaciones>();

            Api<ResDenominaciones> servicio = new Api<ResDenominaciones>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/denominaciones_por_unidad";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResDenominaciones responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResDenominaciones));
            ResDenominaciones resp = responseGET_withToken;

            foreach (ct_denominaciones item in resp.data)
            {
                ct_denominaciones reg = new ct_denominaciones();
                reg.pkDenominacion = item.pkDenominacion;
                reg.nombre = item.nombre;
                reg.valor = item.valor;
                reg.path_imagen = item.path_imagen;
                reg.bin_imagen = item.bin_imagen;
                reg.posicion = item.posicion;
                reg.status = item.status;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }

        internal List<ct_denominaciones> GetDenominacionesPorUnidad()
        {
            throw new NotImplementedException();
        }
    }
}
