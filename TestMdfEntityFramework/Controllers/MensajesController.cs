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
    public class MensajesController
    {
        public List<sy_mensajes> GetMensajes()
        {
            List<sy_mensajes> list_temp = new List<sy_mensajes>();

            Api<ResMensajes> servicio = new Api<ResMensajes>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/mensajesConsola";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResMensajes responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResMensajes));
            ResMensajes resp = responseGET_withToken;

            foreach (sy_mensajes item in resp.data)
            {
                sy_mensajes reg = new sy_mensajes();
                reg.pkMensaje = item.pkMensaje;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkStatus = item.fkStatus;
                reg.mensaje = item.mensaje;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.modo = item.modo;
                reg.dispositivo_origen = item.dispositivo_origen;
                reg.dispositivo_destino = item.dispositivo_destino;
                reg.reproducido = item.reproducido;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public List<sy_mensajes> GetMensajesWithJoins()
        {
            List<sy_mensajes> list_temp = new List<sy_mensajes>();

            Api<ResMensajes> servicio = new Api<ResMensajes>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/mensajesConsolaWithJoins";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResMensajes responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResMensajes));
            ResMensajes resp = responseGET_withToken;

            foreach (sy_mensajes item in resp.data)
            {
                sy_mensajes reg = new sy_mensajes();
                reg.pkMensaje = item.pkMensaje;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkStatus = item.fkStatus;
                reg.mensaje = item.mensaje;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.modo = item.modo;
                reg.dispositivo_origen = item.dispositivo_origen;
                reg.dispositivo_destino = item.dispositivo_destino;
                reg.reproducido = item.reproducido;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public ResMensajes_Insert InsertMensaje(sy_mensajes obj)
        {
            Api<ResMensajes_Insert> servicio = new Api<ResMensajes_Insert>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqMensajes req = new ReqMensajes();
            req.pkMensaje = obj.pkMensaje;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkStatus = obj.fkStatus;
            req.mensaje = obj.mensaje;
            req.enviado = (int)obj.enviado;
            req.confirmadoTISA = (int)obj.confirmadoTISA;
            req.modo = obj.modo;
            req.dispositivo_origen = obj.dispositivo_origen == null ? 0 : (int)obj.dispositivo_origen;
            req.dispositivo_destino = obj.dispositivo_destino == null ? 0 : (int)obj.dispositivo_destino;
            req.reproducido = obj.reproducido == null ? 0 : (int)obj.reproducido;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            //string baseurl = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/mensajeConsola/register";
            ResMensajes_Insert res = (ResMensajes_Insert)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResMensajes UpdateMensaje(sy_mensajes obj)
        {
            Api<ResMensajes> servicio = new Api<ResMensajes>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqMensajes req = new ReqMensajes();
            req.pkMensaje = obj.pkMensaje;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkStatus = obj.fkStatus;
            req.mensaje = obj.mensaje;
            req.enviado = (int)obj.enviado;
            req.confirmadoTISA = (int)obj.confirmadoTISA;
            req.modo = obj.modo;
            req.dispositivo_origen = obj.dispositivo_origen == null ? 0 : (int)obj.dispositivo_origen;
            req.dispositivo_destino = obj.dispositivo_destino == null ? 0 : (int)obj.dispositivo_destino;
            req.reproducido = obj.reproducido == null ? 0 : (int)obj.reproducido;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/mensajeConsola/edit";
            ResMensajes res = (ResMensajes)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResMensajes DeleteMensaje(sy_mensajes obj)
        {
            Api<ResMensajes> servicio = new Api<ResMensajes>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqMensajes req = new ReqMensajes();
            req.pkMensaje = obj.pkMensaje;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkStatus = obj.fkStatus;
            req.mensaje = obj.mensaje;
            req.enviado = (int)obj.enviado;
            req.confirmadoTISA = (int)obj.confirmadoTISA;
            req.modo = obj.modo;
            req.dispositivo_origen = obj.dispositivo_origen == null ? 0 : (int)obj.dispositivo_origen;
            req.dispositivo_destino = obj.dispositivo_destino == null ? 0 : (int)obj.dispositivo_destino;
            req.reproducido = obj.reproducido == null ? 0 : (int)obj.reproducido;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/mensajeConsola/delete";
            ResMensajes res = (ResMensajes)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
    }
}
