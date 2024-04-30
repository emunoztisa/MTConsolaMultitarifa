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
    public class BoletosTarifaFijaController
    {
        private Type T;

        public List<sy_boletos_tarifa_fija> GetBoletos()
        {
            List<sy_boletos_tarifa_fija> list_temp = new List<sy_boletos_tarifa_fija>();

            Api<ResBoletosTarifaFija> servicio = new Api<ResBoletosTarifaFija>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/boletosTarifaFijaAlancia";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResBoletosTarifaFija responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResBoletosTarifaFija));
            ResBoletosTarifaFija resp = responseGET_withToken;

            foreach (sy_boletos_tarifa_fija item in resp.data)
            {
                sy_boletos_tarifa_fija reg = new sy_boletos_tarifa_fija();
                reg.pkBoleto = item.pkBoleto;
                reg.pkBoletoTISA = item.pkBoletoTISA;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkStatus = item.fkStatus;
                reg.folio = item.folio;
                reg.tarifa = item.tarifa;
                reg.cant_pasajeros = item.cant_pasajeros;
                reg.total = item.total;
                reg.totalCobrado = item.totalCobrado;
                reg.totalPagado = item.totalPagado;
                reg.fechaHoraCancelacion = item.fechaHoraCancelacion;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public List<sy_boletos_tarifa_fija> GetBoletosWithJoins()
        {
            List<sy_boletos_tarifa_fija> list_temp = new List<sy_boletos_tarifa_fija>();

            Api<ResBoletosTarifaFija> servicio = new Api<ResBoletosTarifaFija>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/boletosTarifaFijaAlcanciaWithJoins";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResBoletosTarifaFija responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResBoletosTarifaFija));
            ResBoletosTarifaFija resp = responseGET_withToken;

            foreach (sy_boletos_tarifa_fija item in resp.data)
            {
                sy_boletos_tarifa_fija reg = new sy_boletos_tarifa_fija();
                reg.pkBoleto = item.pkBoleto;
                reg.pkBoletoTISA = item.pkBoletoTISA;
                reg.fkAsignacion = item.fkAsignacion;
                reg.fkStatus = item.fkStatus;
                reg.tarifa = item.tarifa;
                reg.cant_pasajeros = item.cant_pasajeros;
                reg.total = item.total;
                reg.totalCobrado = item.totalCobrado;
                reg.totalPagado = item.totalPagado;
                reg.fechaHoraCancelacion = item.fechaHoraCancelacion;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public ResBoletosTarifaFija_Insert InsertBoleto(sy_boletos_tarifa_fija obj_boleto)
        {
            Api<ResBoletosTarifaFija_Insert> servicio = new Api<ResBoletosTarifaFija_Insert>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqBoletosTarifaFija req = new ReqBoletosTarifaFija();
            req.pkBoleto = obj_boleto.pkBoleto;
            req.pkBoletoTISA = obj_boleto.pkBoletoTISA;
            req.fkAsignacion = obj_boleto.fkAsignacion;
            req.fkStatus = obj_boleto.fkStatus;
            req.folio = obj_boleto.folio;

            req.tarifa = obj_boleto.tarifa.ToString();
            req.cant_pasajeros = obj_boleto.cant_pasajeros != null ? (int)obj_boleto.cant_pasajeros : 0;
            req.total = obj_boleto.total.ToString();

            req.totalCobrado = obj_boleto.totalCobrado.ToString();
            req.totalPagado = obj_boleto.totalPagado.ToString();
            req.fechaHoraCancelacion = obj_boleto.fechaHoraCancelacion;
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
            string metodo = "mt/boletosTarifaFijaAlcancia/register";
            ResBoletosTarifaFija_Insert res = (ResBoletosTarifaFija_Insert)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResBoletosTarifaFija UpdateBoleto(sy_boletos_tarifa_fija obj_boleto)
        {
            Api<ResBoletosTarifaFija> servicio = new Api<ResBoletosTarifaFija>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqBoletosTarifaFija req = new ReqBoletosTarifaFija();
            req.pkBoleto = obj_boleto.pkBoleto;
            req.pkBoletoTISA = (long)obj_boleto.pkBoletoTISA;
            req.fkAsignacion = (long)obj_boleto.fkAsignacion;
            req.fkStatus = (long)obj_boleto.fkStatus;
            req.folio = obj_boleto.folio;
            req.tarifa = obj_boleto.tarifa.ToString();
            req.cant_pasajeros = (int)obj_boleto.cant_pasajeros;
            req.total = obj_boleto.total.ToString();
            req.totalCobrado = obj_boleto.totalCobrado.ToString();
            req.totalPagado = obj_boleto.totalPagado.ToString();
            req.fechaHoraCancelacion = obj_boleto.fechaHoraCancelacion;
            req.enviado = (int)obj_boleto.enviado;
            req.confirmadoTISA = (int)obj_boleto.confirmadoTISA;
            req.created_at = obj_boleto.created_at;
            req.updated_at = obj_boleto.updated_at;
            req.deleted_at = obj_boleto.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/boletosTarifaFijaAlcancia/edit";
            ResBoletosTarifaFija res = (ResBoletosTarifaFija)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResBoletosTarifaFija DeleteBoleto(sy_boletos_tarifa_fija obj_boleto)
        {
            Api<ResBoletosTarifaFija> servicio = new Api<ResBoletosTarifaFija>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqBoletosTarifaFija req = new ReqBoletosTarifaFija();
            req.pkBoleto = obj_boleto.pkBoleto;
            req.pkBoletoTISA = (long)obj_boleto.pkBoletoTISA;
            req.fkAsignacion = (long)obj_boleto.fkAsignacion;
            req.fkStatus = (long)obj_boleto.fkStatus;
            req.folio = obj_boleto.folio;
            req.tarifa = obj_boleto.tarifa.ToString();
            req.cant_pasajeros = (int)obj_boleto.cant_pasajeros;
            req.total = obj_boleto.total.ToString();
            req.totalCobrado = obj_boleto.totalCobrado.ToString();
            req.totalPagado = obj_boleto.totalPagado.ToString();
            req.fechaHoraCancelacion = obj_boleto.fechaHoraCancelacion;
            req.enviado = (int)obj_boleto.enviado;
            req.confirmadoTISA = (int)obj_boleto.confirmadoTISA;
            req.created_at = obj_boleto.created_at;
            req.updated_at = obj_boleto.updated_at;
            req.deleted_at = obj_boleto.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/boletosTarifaFijaAlcancia/delete";
            ResBoletosTarifaFija res = (ResBoletosTarifaFija)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
    }
}
