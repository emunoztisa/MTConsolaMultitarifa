using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class ConfigVariosController
    {
        private Type T;

        public List<config_varios> GetConfigVarios()
        {
            List<config_varios> list_temp = new List<config_varios>();

            Api<ResConfigVarios> servicio = new Api<ResConfigVarios>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/config_varios";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();

            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResConfigVarios responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResConfigVarios));
            ResConfigVarios resp = responseGET_withToken;

            foreach (config_varios item in resp.data)
            {
                config_varios reg = new config_varios();
                reg.pkConfigVarios = item.pkConfigVarios;
                reg.clave = item.clave;
                reg.valor = item.valor;
                reg.descripcion = item.descripcion;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }

        public List<config_varios> GetConfigVariosPorUnidad(ct_unidades obj_unidad)
        {
            List<config_varios> list_temp = new List<config_varios>();

            Api<ResConfigVarios> servicio = new Api<ResConfigVarios>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/config_varios_por_unidad";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResConfigVarios responsePOST_withToken = servicio.RequestPost_withToken(base_url, metodo_web, obj_unidad, headers, typeof(ResConfigVarios));
            ResConfigVarios resp = responsePOST_withToken;

            foreach (config_varios item in resp.data)
            {
                config_varios reg = new config_varios();
                reg.pkConfigVarios = item.pkConfigVarios;
                reg.clave = item.clave;
                reg.valor = item.valor;
                reg.descripcion = item.descripcion;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }

    }
}
