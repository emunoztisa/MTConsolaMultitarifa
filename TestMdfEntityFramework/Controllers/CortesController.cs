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
    public class CortesController
    {
        private Type T;
        public List<sy_cortes> GetCortes()
        {
            List<sy_cortes> list_temp = new List<sy_cortes>();

            Api<ResCortes> servicio = new Api<ResCortes>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/cortesAlcancia";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResCortes responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResCortes));
            ResCortes resp = responseGET_withToken;

            foreach (sy_cortes item in resp.data)
            {
                sy_cortes reg = new sy_cortes();
                reg.pkCorte = item.pkCorte;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkLugarOrigen = item.fkLugarOrigen;
                reg.fkLugarDestino = item.fkLugarDestino;
                reg.fkStatus = item.fkStatus;
                reg.folio = item.folio;
                reg.fecha = item.fecha;
                reg.hora = item.hora;
                reg.total_efectivo_acumulado = item.total_efectivo_acumulado;
                reg.total_tarifas = item.total_tarifas;
                reg.total_efectivo_rst = item.total_efectivo_rst;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public List<sy_cortes> GetCortesWithJoins()
        {
            List<sy_cortes> list_temp = new List<sy_cortes>();

            Api<ResCortes> servicio = new Api<ResCortes>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/cortesAlcanciaWithJoins";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResCortes responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResCortes));
            ResCortes resp = responseGET_withToken;

            foreach (sy_cortes item in resp.data)
            {
                sy_cortes reg = new sy_cortes();
                reg.pkCorte = item.pkCorte;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkLugarOrigen = item.fkLugarOrigen;
                reg.fkLugarDestino = item.fkLugarDestino;
                reg.fkStatus = item.fkStatus;
                reg.folio = item.folio;
                reg.fecha = item.fecha;
                reg.hora = item.hora;
                reg.total_efectivo_acumulado = item.total_efectivo_acumulado;
                reg.total_tarifas = item.total_tarifas;
                reg.total_efectivo_rst = item.total_efectivo_rst;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public ResCortes_Insert InsertCorte(sy_cortes obj)
        {
            Api<ResCortes_Insert> servicio = new Api<ResCortes_Insert>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqCortes req = new ReqCortes();
            req.pkCorte = obj.pkCorte;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkLugarOrigen = obj.fkLugarOrigen;
            req.fkLugarDestino = obj.fkLugarDestino;
            req.fkStatus = obj.fkStatus;
            req.folio = obj.folio;
            req.fecha = obj.fecha;
            req.hora = obj.hora;
            req.total_efectivo_acumulado = (decimal)obj.total_efectivo_acumulado;
            req.total_tarifas = (decimal)obj.total_tarifas;
            req.total_efectivo_rst = (decimal)obj.total_efectivo_rst;

            req.efectivo_moneda = obj.efectivo_moneda;
            req.efectivo_billete = obj.efectivo_billete;

            req.cant_mon_tipo_0 = obj.cant_mon_tipo_0;
            req.cant_mon_tipo_1 = obj.cant_mon_tipo_1;
            req.cant_mon_tipo_2 = obj.cant_mon_tipo_2;
            req.cant_mon_tipo_3 = obj.cant_mon_tipo_3;
            req.cant_mon_tipo_4 = obj.cant_mon_tipo_4;
            req.cant_mon_tipo_5 = obj.cant_mon_tipo_5;
            req.cant_mon_tipo_6 = obj.cant_mon_tipo_6;

            req.cant_bill_tipo_0 = obj.cant_bill_tipo_0;
            req.cant_bill_tipo_1 = obj.cant_bill_tipo_1;
            req.cant_bill_tipo_2 = obj.cant_bill_tipo_2;
            req.cant_bill_tipo_3 = obj.cant_bill_tipo_3;
            req.cant_bill_tipo_4 = obj.cant_bill_tipo_4;
            req.cant_bill_tipo_5 = obj.cant_bill_tipo_5;
            req.cant_bill_tipo_6 = obj.cant_bill_tipo_6;

            req.enviado = obj.enviado != null ? (int)obj.enviado : 0;
            req.confirmadoTISA = obj.confirmadoTISA != null ? (int)obj.confirmadoTISA : 0;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            //string baseurl = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/corteAlcancia/register";
            ResCortes_Insert res = (ResCortes_Insert)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResCortes UpdateCorte(sy_cortes obj)
        {
            Api<ResCortes> servicio = new Api<ResCortes>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqCortes req = new ReqCortes();
            req.pkCorte = obj.pkCorte;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkLugarOrigen = obj.fkLugarOrigen;
            req.fkLugarDestino = obj.fkLugarDestino;
            req.fkStatus = obj.fkStatus;
            req.folio = obj.folio;
            req.fecha = obj.fecha;
            req.hora = obj.hora;
            req.total_efectivo_acumulado = (decimal)obj.total_efectivo_acumulado;
            req.total_tarifas = (decimal)obj.total_tarifas;
            req.total_efectivo_rst = (decimal)obj.total_efectivo_rst;
            req.enviado = (int)obj.enviado;
            req.confirmadoTISA = (int)obj.confirmadoTISA;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/corteAlcancia/edit";
            ResCortes res = (ResCortes)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResCortes DeleteCorte(sy_cortes obj)
        {
            Api<ResCortes> servicio = new Api<ResCortes>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqCortes req = new ReqCortes();
            req.pkCorte = obj.pkCorte;
            req.fkAsignacion = obj.fkAsignacion;
            req.fkLugarOrigen = obj.fkLugarOrigen;
            req.fkLugarDestino = obj.fkLugarDestino;
            req.fkStatus = obj.fkStatus;
            req.folio = obj.folio;
            req.fecha = obj.fecha;
            req.hora = obj.hora;
            req.total_efectivo_acumulado = (decimal)obj.total_efectivo_acumulado;
            req.total_tarifas = (decimal)obj.total_tarifas;
            req.total_efectivo_rst = (decimal)obj.total_efectivo_rst;
            req.enviado = (int)obj.enviado;
            req.confirmadoTISA = (int)obj.confirmadoTISA;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/corteAlcancia/delete";
            ResCortes res = (ResCortes)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
    }
}
