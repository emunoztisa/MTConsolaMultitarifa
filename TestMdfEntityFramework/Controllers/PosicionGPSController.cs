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
    public class PosicionGPSController
    {
        private Type T;

        public List<sy_posicion_gps> GetPosicionesGPS()
        {
            List<sy_posicion_gps> list_temp = new List<sy_posicion_gps>();

            Api<ResPosicionGPS> servicio = new Api<ResPosicionGPS>();
            Comun mc = new Comun();

            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/posicionesGPS";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResPosicionGPS responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResPosicionGPS));
            ResPosicionGPS resp = responseGET_withToken;

            foreach (sy_posicion_gps item in resp.data)
            {
                sy_posicion_gps reg = new sy_posicion_gps();
                reg.pkPosicionGPS = item.pkPosicionGPS;
                reg.pkPosicionGPSTISA = item.pkPosicionGPSTISA;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkStatus = item.fkStatus;
                reg.lat = item.lat;
                reg.lng = item.lng;
                reg.fecha_hora = item.fecha_hora;
                reg.enviado = item.enviado;
                reg.confirmado = item.confirmado;
                reg.modo = item.modo;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public List<sy_posicion_gps> GetPosicionesGPSsWithJoins()
        {
            List<sy_posicion_gps> list_temp = new List<sy_posicion_gps>();

            Api<ResPosicionGPS> servicio = new Api<ResPosicionGPS>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/posicionesGPSWithJoins";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResPosicionGPS responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResPosicionGPS));
            ResPosicionGPS resp = responseGET_withToken;

            foreach (sy_posicion_gps item in resp.data)
            {
                sy_posicion_gps reg = new sy_posicion_gps();
                reg.pkPosicionGPS = item.pkPosicionGPS;
                reg.pkPosicionGPSTISA = item.pkPosicionGPSTISA;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkStatus = item.fkStatus;
                reg.lat = item.lat;
                reg.lng = item.lng;
                reg.fecha_hora = item.fecha_hora;
                reg.enviado = item.enviado;
                reg.confirmado = item.confirmado;
                reg.modo = item.modo;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public ResPosicionGPS_Insert InsertPosicionGPS(sy_posicion_gps obj)
        {
            Api<ResPosicionGPS_Insert> servicio = new Api<ResPosicionGPS_Insert>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ReqPosicionGPS req = new ReqPosicionGPS();
            req.pkPosicionGPS = obj.pkPosicionGPS;
            req.pkPosicionGPSTISA = obj.pkPosicionGPSTISA;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkStatus = obj.fkStatus;
            req.lat = obj.lat;
            req.lng = obj.lng;
            req.fecha_hora = obj.fecha_hora;
            req.enviado = obj.enviado;
            req.confirmado = obj.confirmado;
            req.modo = obj.modo;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/posicionGPS/register";
            ResPosicionGPS_Insert res = (ResPosicionGPS_Insert)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResPosicionGPS UpdatePosicionGPS(sy_posicion_gps obj)
        {
            Api<ResPosicionGPS> servicio = new Api<ResPosicionGPS>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqPosicionGPS req = new ReqPosicionGPS();
            req.pkPosicionGPS = obj.pkPosicionGPS;
            req.pkPosicionGPSTISA = obj.pkPosicionGPSTISA;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkStatus = obj.fkStatus;
            req.lat = obj.lat;
            req.lng = obj.lng;
            req.fecha_hora = obj.fecha_hora;
            req.enviado = obj.enviado;
            req.confirmado = obj.confirmado;
            req.modo = obj.modo;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/posicionGPS/edit";
            ResPosicionGPS res = (ResPosicionGPS)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResPosicionGPS DeletePosicionGPS(sy_posicion_gps obj)
        {
            Api<ResPosicionGPS> servicio = new Api<ResPosicionGPS>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqPosicionGPS req = new ReqPosicionGPS();
            req.pkPosicionGPS = obj.pkPosicionGPS;
            req.pkPosicionGPSTISA = obj.pkPosicionGPSTISA;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkStatus = obj.fkStatus;
            req.lat = obj.lat;
            req.lng = obj.lng;
            req.fecha_hora = obj.fecha_hora;
            req.enviado = obj.enviado;
            req.confirmado = obj.confirmado;
            req.modo = obj.modo;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/posicionGPS/delete";
            ResPosicionGPS res = (ResPosicionGPS)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
    }
}
