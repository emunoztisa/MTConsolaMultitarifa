using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class OpcionesGeneralesController
    {
        private Type T;

        public List<opciones_generales> GetOpcionesGenerales()
        {
            List<opciones_generales> list_temp = new List<opciones_generales>();

            Api<ResOpcionesGenerales> servicio = new Api<ResOpcionesGenerales>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/opciones_generales";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResOpcionesGenerales responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResOpcionesGenerales));
            ResOpcionesGenerales resp = responseGET_withToken;

            foreach (opciones_generales item in resp.data)
            {
                opciones_generales reg = new opciones_generales();
                reg.pkOpcionGeneral = item.pkOpcionGeneral;
                reg.opcion_general = item.opcion_general;
                reg.valor = item.valor;
                reg.orden = item.orden;
                reg.agrupador = item.agrupador;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }

        public List<opciones_generales> GetOpcionesGeneralesPorUnidad(ct_unidades obj_unidad)
        {
            List<opciones_generales> list_temp = new List<opciones_generales>();

            Api<ResOpcionesGenerales> servicio = new Api<ResOpcionesGenerales>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/opciones_generales_por_unidad";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResOpcionesGenerales responsePOST_withToken = servicio.RequestPost_withToken(base_url, metodo_web, obj_unidad, headers, typeof(ResOpcionesGenerales));
            ResOpcionesGenerales resp = responsePOST_withToken;

            foreach (opciones_generales item in resp.data)
            {
                opciones_generales reg = new opciones_generales();
                reg.pkOpcionGeneral = item.pkOpcionGeneral;
                reg.opcion_general = item.opcion_general;
                reg.valor = item.valor;
                reg.orden = item.orden;
                reg.agrupador = item.agrupador;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }

    }
}
