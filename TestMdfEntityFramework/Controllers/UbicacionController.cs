using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Requests;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class UbicacionController
    {
        private Type T;

        public List<sy_ubicacion> GetUbicacion()
        {
            List<sy_ubicacion> list_temp = new List<sy_ubicacion>();

            Api<ResUbicacion> servicio = new Api<ResUbicacion>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/ubicacionConsola";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResUbicacion responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResUbicacion));
            ResUbicacion resp = responseGET_withToken;

            foreach (sy_ubicacion item in resp.data)
            {
                sy_ubicacion reg = new sy_ubicacion();
                reg.pkUbicacion = item.pkUbicacion;
                reg.fkAsignacion = item.fkAsignacion;
                reg.latitud = item.latitud;
                reg.longitud = item.longitud;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public List<sy_ubicacion> GetUbicacionWithJoins()
        {
            List<sy_ubicacion> list_temp = new List<sy_ubicacion>();

            Api<ResUbicacion> servicio = new Api<ResUbicacion>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/ubicacionConsolaWithJoins";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResUbicacion responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResUbicacion));
            ResUbicacion resp = responseGET_withToken;

            foreach (sy_ubicacion item in resp.data)
            {
                sy_ubicacion reg = new sy_ubicacion();
                reg.pkUbicacion = item.pkUbicacion;
                reg.fkAsignacion = item.fkAsignacion;
                reg.latitud = item.latitud;
                reg.longitud = item.longitud;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public ResUbicacion InsertUbicacion(sy_ubicacion obj)
        {
            Api<ResUbicacion> servicio = new Api<ResUbicacion>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqUbicacion req = new ReqUbicacion();
            req.pkUbicacion = obj.pkUbicacion;
            req.fkAsignacion = obj.fkAsignacion;
            req.latitud = obj.latitud != null ? (decimal)obj.latitud : 0;
            req.longitud = obj.longitud != null ? (decimal)obj.longitud : 0;
            req.enviado = (int)obj.enviado;
            req.confirmadoTISA = (int)obj.confirmadoTISA;
            req.modo = obj.modo;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            //string baseurl = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/ubicacionConsola/register";
            ResUbicacion res = (ResUbicacion)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResUbicacion UpdateUbicacion(sy_ubicacion obj)
        {
            Api<ResUbicacion> servicio = new Api<ResUbicacion>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqUbicacion req = new ReqUbicacion();
            req.pkUbicacion = obj.pkUbicacion;
            req.fkAsignacion = obj.fkAsignacion;
            req.latitud = obj.latitud != null ? (decimal)obj.latitud : 0;
            req.longitud = obj.longitud != null ? (decimal)obj.longitud : 0;
            req.enviado = (int)obj.enviado;
            req.confirmadoTISA = (int)obj.confirmadoTISA;
            req.modo = obj.modo;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/ubicacionConsola/edit";
            ResUbicacion res = (ResUbicacion)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResUbicacion DeleteUbicacion(sy_ubicacion obj)
        {
            Api<ResUbicacion> servicio = new Api<ResUbicacion>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqUbicacion req = new ReqUbicacion();
            req.pkUbicacion = obj.pkUbicacion;
            req.fkAsignacion = obj.fkAsignacion;
            req.latitud = obj.latitud != null ? (decimal)obj.latitud : 0;
            req.longitud = obj.longitud != null ? (decimal)obj.longitud : 0;
            req.enviado = (int)obj.enviado;
            req.confirmadoTISA = (int)obj.confirmadoTISA;
            req.modo = obj.modo;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/ubicacionConsola/delete";
            ResUbicacion res = (ResUbicacion)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
    }
}
