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
    public class CuentaCocosController
    {
        private Type T;

        public List<sy_conteo_cuenta_cocos> GetConteoCuentaCocos()
        {
            List<sy_conteo_cuenta_cocos> list_temp = new List<sy_conteo_cuenta_cocos>();

            Api<ResCuentaCocos> servicio = new Api<ResCuentaCocos>();
            Comun mc = new Comun();

            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/conteoCuentaCocos";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResCuentaCocos responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResCuentaCocos));
            ResCuentaCocos resp = responseGET_withToken;

            foreach (sy_conteo_cuenta_cocos item in resp.data)
            {
                sy_conteo_cuenta_cocos reg = new sy_conteo_cuenta_cocos();
                reg.pkConteoCuentaCocos = item.pkConteoCuentaCocos;
                reg.pkConteoCuentaCocosTISA = item.pkConteoCuentaCocosTISA;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkStatus = item.fkStatus;
                reg.cc1_subidas = item.cc1_subidas;
                reg.cc1_bajadas = item.cc1_bajadas;
                reg.cc2_subidas = item.cc2_subidas;
                reg.cc2_bajadas = item.cc2_bajadas;
                reg.cc3_subidas = item.cc3_subidas;
                reg.cc3_bajadas = item.cc3_bajadas;
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
        public List<sy_conteo_cuenta_cocos> GetConteoCuentaCocosWithJoins()
        {
            List<sy_conteo_cuenta_cocos> list_temp = new List<sy_conteo_cuenta_cocos>();

            Api<ResCuentaCocos> servicio = new Api<ResCuentaCocos>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/conteoCuentaCocosWithJoins";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResCuentaCocos responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResCuentaCocos));
            ResCuentaCocos resp = responseGET_withToken;

            foreach (sy_conteo_cuenta_cocos item in resp.data)
            {
                sy_conteo_cuenta_cocos reg = new sy_conteo_cuenta_cocos();
                reg.pkConteoCuentaCocos = item.pkConteoCuentaCocos;
                reg.pkConteoCuentaCocosTISA = item.pkConteoCuentaCocosTISA;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkStatus = item.fkStatus;
                reg.cc1_subidas = item.cc1_subidas;
                reg.cc1_bajadas = item.cc1_bajadas;
                reg.cc2_subidas = item.cc2_subidas;
                reg.cc2_bajadas = item.cc2_bajadas;
                reg.cc3_subidas = item.cc3_subidas;
                reg.cc3_bajadas = item.cc3_bajadas;
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
        public ResCuentaCocos_Insert InsertConteoCuentaCocos(sy_conteo_cuenta_cocos obj)
        {
            Api<ResCuentaCocos_Insert> servicio = new Api<ResCuentaCocos_Insert>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqCuentaCocos req = new ReqCuentaCocos();
            req.pkConteoCuentaCocos = obj.pkConteoCuentaCocos;
            req.pkConteoCuentaCocosTISA = obj.pkConteoCuentaCocosTISA;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkStatus = obj.fkStatus;
            req.cc1_subidas = obj.cc1_subidas;
            req.cc1_bajadas = obj.cc1_bajadas;
            req.cc2_subidas = obj.cc2_subidas;
            req.cc2_bajadas = obj.cc2_bajadas;
            req.cc3_subidas = obj.cc3_subidas;
            req.cc3_bajadas = obj.cc3_bajadas;
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
            string metodo = "mt/conteoCuentaCocos/register";
            ResCuentaCocos_Insert res = (ResCuentaCocos_Insert)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResCuentaCocos UpdateConteoCuentaCocos(sy_conteo_cuenta_cocos obj)
        {
            Api<ResCuentaCocos> servicio = new Api<ResCuentaCocos>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqCuentaCocos req = new ReqCuentaCocos();
            req.pkConteoCuentaCocos = obj.pkConteoCuentaCocos;
            req.pkConteoCuentaCocosTISA = obj.pkConteoCuentaCocosTISA;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkStatus = obj.fkStatus;
            req.cc1_subidas = obj.cc1_subidas;
            req.cc1_bajadas = obj.cc1_bajadas;
            req.cc2_subidas = obj.cc2_subidas;
            req.cc2_bajadas = obj.cc2_bajadas;
            req.cc3_subidas = obj.cc3_subidas;
            req.cc3_bajadas = obj.cc3_bajadas;
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
            string metodo = "mt/conteoCuentaCocos/edit";
            ResCuentaCocos res = (ResCuentaCocos)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResCuentaCocos DeleteConteoCuentaCocos(sy_conteo_cuenta_cocos obj)
        {
            Api<ResCuentaCocos> servicio = new Api<ResCuentaCocos>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqCuentaCocos req = new ReqCuentaCocos();
            req.pkConteoCuentaCocos = obj.pkConteoCuentaCocos;
            req.pkConteoCuentaCocosTISA = obj.pkConteoCuentaCocosTISA;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkStatus = obj.fkStatus;
            req.cc1_subidas = obj.cc1_subidas;
            req.cc1_bajadas = obj.cc1_bajadas;
            req.cc2_subidas = obj.cc2_subidas;
            req.cc2_bajadas = obj.cc2_bajadas;
            req.cc3_subidas = obj.cc3_subidas;
            req.cc3_bajadas = obj.cc3_bajadas;
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
            string metodo = "mt/conteoCuentaCocos/delete";
            ResCuentaCocos res = (ResCuentaCocos)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
    }
}
