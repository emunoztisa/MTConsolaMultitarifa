using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class TarifasMontosFijosController
    {
        private Type T;

        public List<ct_tarifas_montos_fijos> GetTarifasMontosFijos()
        {
            List<ct_tarifas_montos_fijos> list_temp = new List<ct_tarifas_montos_fijos>();

            Api<ResTarifasMontosFijos> servicio = new Api<ResTarifasMontosFijos>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/tarifas_montos_fijos";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResTarifasMontosFijos responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResTarifasMontosFijos));
            ResTarifasMontosFijos resp = responseGET_withToken;

            foreach (ct_tarifas_montos_fijos item in resp.data)
            {
                ct_tarifas_montos_fijos reg = new ct_tarifas_montos_fijos();
                reg.pkTarifaMontoFijo = item.pkTarifaMontoFijo;
                reg.valor = item.valor;
                reg.texto = item.texto;
                reg.descripcion = item.descripcion;
                reg.orden = item.orden;
                reg.status = item.status;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }

        public List<ct_tarifas_montos_fijos> GetTarifasMontosFijosPorUnidad(ct_unidades obj_unidad)
        {
            List<ct_tarifas_montos_fijos> list_temp = new List<ct_tarifas_montos_fijos>();

            Api<ResTarifasMontosFijos> servicio = new Api<ResTarifasMontosFijos>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/tarifas_montos_fijos_por_unidad";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResTarifasMontosFijos responsePOST_withToken = servicio.RequestPost_withToken(base_url, metodo_web, obj_unidad, headers, typeof(ResTarifasMontosFijos));
            ResTarifasMontosFijos resp = responsePOST_withToken;

            foreach (ct_tarifas_montos_fijos item in resp.data)
            {
                ct_tarifas_montos_fijos reg = new ct_tarifas_montos_fijos();
                reg.pkTarifaMontoFijo = item.pkTarifaMontoFijo;
                reg.valor = item.valor;
                reg.texto = item.texto;
                reg.descripcion = item.descripcion;
                reg.orden = item.orden;
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
