using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class AsignacionesController
    {
        private Type T;
        public List<sy_asignaciones> GetAsignaciones()
        {
            List<sy_asignaciones> list_temp = new List<sy_asignaciones>();

            Api<ResAsignaciones> servicio = new Api<ResAsignaciones>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/asignaciones";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResAsignaciones responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResAsignaciones));
            ResAsignaciones resp = responseGET_withToken;

            //foreach (ResponseLugares.Data data1 in data)
            foreach (sy_asignaciones item in resp.data)
            {
                sy_asignaciones reg = new sy_asignaciones();
                reg.pkAsignacion = item.pkAsignacion;
                reg.fkRuta = item.fkRuta;
                reg.fkUnidad = item.fkUnidad;
                reg.fkOperador = item.fkOperador;
                reg.fkAndador = item.fkAndador;
                reg.fkLiquidacion = item.fkLiquidacion;
                reg.fkStatus = item.fkStatus;
                reg.folio = item.folio;
                reg.fecha = item.fecha;
                reg.hora = item.hora;
                reg.cantidadAsientosDisp = item.cantidadAsientosDisp;
                reg.recurrente = item.recurrente;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }

        public List<sy_asignaciones> GetAsignacionesByUnidad(Int64 pkUnidad)
        {
            List<sy_asignaciones> list_temp = new List<sy_asignaciones>();

            Api<ResAsignaciones> servicio = new Api<ResAsignaciones>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/asignaciones_por_unidad";  // AUN NO SE CREA EL SERVICIO
            string token = mc.GetTokenAdmin();

            sy_asignaciones asig = new sy_asignaciones();
            asig.fkUnidad = pkUnidad;

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResAsignaciones responseGET_withToken = servicio.RequestPost_withToken(base_url, metodo_web, asig, headers, typeof(ResAsignaciones));  //servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResAsignaciones));
            ResAsignaciones resp = responseGET_withToken;

            foreach (sy_asignaciones item in resp.data)
            {
                sy_asignaciones reg = new sy_asignaciones();
                reg.pkAsignacion = item.pkAsignacion;
                reg.fkRuta = item.fkRuta;
                reg.fkUnidad = item.fkUnidad;
                reg.fkOperador = item.fkOperador;
                reg.fkAndador = item.fkAndador;
                reg.fkLiquidacion = item.fkLiquidacion;
                reg.fkStatus = item.fkStatus;
                reg.folio = item.folio;
                reg.fecha = item.fecha;
                reg.hora = item.hora;
                reg.ihora = item.ihora;
                reg.cantidadAsientosDisp = item.cantidadAsientosDisp;
                reg.recurrente = item.recurrente;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
    }
}
