using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Requests;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controller
{
    public class BoletosController
    {
        private Type T;

        public List<sy_boletos> GetBoletos()
        {
            List<sy_boletos> list_temp = new List<sy_boletos>();

            Api<ResBoletos> servicio = new Api<ResBoletos>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/boletosAlcancia";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResBoletos responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResBoletos));
            ResBoletos resp = responseGET_withToken;

            foreach (sy_boletos item in resp.data)
            {
                sy_boletos reg = new sy_boletos();
                reg.pkBoleto = item.pkBoleto;
                reg.pkBoletoTISA = item.pkBoletoTISA;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkLugarOrigen = item.fkLugarOrigen;
                reg.fkLugarDestino = item.fkLugarDestino;
                reg.fkStatus = item.fkStatus;
                reg.folio = item.folio;
                reg.total = item.total;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public List<sy_boletos> GetBoletosWithJoins()
        {
            List<sy_boletos> list_temp = new List<sy_boletos>();

            Api<ResBoletos> servicio = new Api<ResBoletos>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/boletosAlcanciaWithJoins";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResBoletos responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResBoletos));
            ResBoletos resp = responseGET_withToken;

            foreach (sy_boletos item in resp.data)
            {
                sy_boletos reg = new sy_boletos();
                reg.pkBoleto = item.pkBoleto;
                reg.pkBoletoTISA = item.pkBoletoTISA;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkLugarOrigen = item.fkLugarOrigen;
                reg.fkLugarDestino = item.fkLugarDestino;
                reg.fkStatus = item.fkStatus;
                reg.folio = item.folio;
                reg.total = item.total;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public ResBoletos_Insert InsertBoleto(sy_boletos obj_boleto)
        {
            Api<ResBoletos_Insert> servicio = new Api<ResBoletos_Insert>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqBoletos req = new ReqBoletos();
            req.pkBoleto = obj_boleto.pkBoleto;
            req.pkBoletoTISA = obj_boleto.pkBoletoTISA;
            req.fkAsignacion = obj_boleto.fkAsignacion;
            req.fkLugarOrigen = obj_boleto.fkLugarOrigen;
            req.fkLugarDestino = obj_boleto.fkLugarDestino;
            req.fkStatus = obj_boleto.fkStatus;
            req.folio = obj_boleto.folio;
            req.total = obj_boleto.total.ToString();
            req.enviado = (int)obj_boleto.enviado;
            req.confirmadoTISA = (int)obj_boleto.confirmadoTISA;
            req.modo = obj_boleto.modo;
            req.created_at = obj_boleto.created_at;
            req.updated_at = obj_boleto.updated_at;
            req.deleted_at = obj_boleto.deleted_at;

            //Consumir servicio de boleto
            //string baseurl = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/boletosAlcancia/register";
            ResBoletos_Insert res = (ResBoletos_Insert)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResBoletos UpdateBoleto(sy_boletos obj_boleto)
        {
            Api<ResBoletos> servicio = new Api<ResBoletos>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqBoletos req = new ReqBoletos();
            req.pkBoleto = obj_boleto.pkBoleto;
            req.pkBoletoTISA = (long)obj_boleto.pkBoletoTISA;
            req.fkAsignacion = (long)obj_boleto.fkAsignacion;
            req.fkLugarOrigen = (long)obj_boleto.fkLugarOrigen;
            req.fkLugarDestino = (long)obj_boleto.fkLugarDestino;
            req.fkStatus = (long)obj_boleto.fkStatus;
            req.folio = obj_boleto.folio;
            req.total = obj_boleto.total.ToString();
            req.enviado = (int)obj_boleto.enviado;
            req.confirmadoTISA = (int)obj_boleto.confirmadoTISA;
            req.created_at = obj_boleto.created_at;
            req.updated_at = obj_boleto.updated_at;
            req.deleted_at = obj_boleto.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/boletosAlcancia/edit";
            ResBoletos res = (ResBoletos)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResBoletos DeleteBoleto(sy_boletos obj_boleto)
        {
            Api<ResBoletos> servicio = new Api<ResBoletos>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqBoletos req = new ReqBoletos();
            req.pkBoleto = obj_boleto.pkBoleto;
            req.pkBoletoTISA = (long)obj_boleto.pkBoletoTISA;
            req.fkAsignacion = (long)obj_boleto.fkAsignacion;
            req.fkLugarOrigen = (long)obj_boleto.fkLugarOrigen;
            req.fkLugarDestino = (long)obj_boleto.fkLugarDestino;
            req.fkStatus = (long)obj_boleto.fkStatus;
            req.folio = obj_boleto.folio;
            req.total = obj_boleto.total.ToString();
            req.enviado = (int)obj_boleto.enviado;
            req.confirmadoTISA = (int)obj_boleto.confirmadoTISA;
            req.created_at = obj_boleto.created_at;
            req.updated_at = obj_boleto.updated_at;
            req.deleted_at = obj_boleto.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/boletosAlcancia/delete";
            ResBoletos res = (ResBoletos)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }

    }
}
