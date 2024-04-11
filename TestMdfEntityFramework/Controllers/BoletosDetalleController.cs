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
    public class BoletosDetalleController
    {
        private Type T;

        public List<sy_boletos_detalle> GetBoletosDetalle()
        {
            List<sy_boletos_detalle> list_temp = new List<sy_boletos_detalle>();

            Api<ResBoletosDetalle> servicio = new Api<ResBoletosDetalle>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/boletosDetalleAlcancia";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResBoletosDetalle responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResBoletosDetalle));
            ResBoletosDetalle resp = responseGET_withToken;

            foreach (sy_boletos_detalle item in resp.data)
            {
                sy_boletos_detalle reg = new sy_boletos_detalle();
                reg.pkBoletoDetalle = item.pkBoletoDetalle;
                reg.pkBoletoDetalleTISA = item.pkBoletoDetalleTISA;
                reg.fkBoleto = item.fkBoleto;
                reg.fkPerfil = item.fkPerfil;
                reg.fkTarifa = item.fkTarifa;
                reg.fkStatus = item.fkStatus;
                reg.cantidad = item.cantidad;
                reg.subtotal = item.subtotal;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public List<sy_boletos_detalle> GetBoletosDetalleWithJoins()
        {
            List<sy_boletos_detalle> list_temp = new List<sy_boletos_detalle>();

            Api<ResBoletosDetalle> servicio = new Api<ResBoletosDetalle>();
            Comun mc = new Comun();

            //string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_base_url = serv_cv.getEntityByClave("BASE_URL");
            string base_url = cv_base_url.valor;
            string metodo_web = "mt/boletosDetalleAlcanciaWithJoins";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ResBoletosDetalle responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResBoletosDetalle));
            ResBoletosDetalle resp = responseGET_withToken;

            foreach (sy_boletos_detalle item in resp.data)
            {
                sy_boletos_detalle reg = new sy_boletos_detalle();
                reg.pkBoletoDetalle = item.pkBoletoDetalle;
                reg.pkBoletoDetalleTISA = item.pkBoletoDetalleTISA;
                reg.fkBoleto = item.fkBoleto;
                reg.fkPerfil = item.fkPerfil;
                reg.fkTarifa = item.fkTarifa;
                reg.fkStatus = item.fkStatus;
                reg.cantidad = item.cantidad;
                reg.subtotal = item.subtotal;
                reg.enviado = item.enviado;
                reg.confirmadoTISA = item.confirmadoTISA;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public ResBoletosDetalle_Insert InsertBoletoDetalle(sy_boletos_detalle obj_boleto_detalle)
        {
            Api<ResBoletosDetalle_Insert> servicio = new Api<ResBoletosDetalle_Insert>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqBoletosDetalle req = new ReqBoletosDetalle();
            req.pkBoletoDetalle = obj_boleto_detalle.pkBoletoDetalle;
            req.pkBoletoDetalleTISA = obj_boleto_detalle.pkBoletoDetalleTISA;
            req.fkBoleto = obj_boleto_detalle.fkBoleto;
            req.fkPerfil = obj_boleto_detalle.fkPerfil;
            req.fkTarifa = obj_boleto_detalle.fkTarifa;
            req.fkStatus = obj_boleto_detalle.fkStatus;
            req.cantidad = (int)obj_boleto_detalle.cantidad;
            req.subtotal = obj_boleto_detalle.subtotal.ToString();
            req.enviado = (int)obj_boleto_detalle.enviado;
            req.confirmadoTISA = (int)obj_boleto_detalle.confirmadoTISA;
            req.modo = obj_boleto_detalle.modo;
            req.created_at = obj_boleto_detalle.created_at;
            req.updated_at = obj_boleto_detalle.updated_at;
            req.deleted_at = obj_boleto_detalle.deleted_at;

            //Consumir servicio de boleto
            //string baseurl = mc.obtenerValorDeAppConfig("URL_BASE");
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/boletosDetalleAlcancia/register";
            ResBoletosDetalle_Insert res = (ResBoletosDetalle_Insert)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResBoletosDetalle UpdateBoletoDetalle(sy_boletos_detalle obj_boleto_detalle)
        {
            Api<ResBoletosDetalle> servicio = new Api<ResBoletosDetalle>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqBoletosDetalle req = new ReqBoletosDetalle();
            req.pkBoletoDetalle = obj_boleto_detalle.pkBoletoDetalle;
            req.pkBoletoDetalleTISA = (long)obj_boleto_detalle.pkBoletoDetalleTISA;
            req.fkBoleto = (long)obj_boleto_detalle.fkBoleto;
            req.fkPerfil = (long)obj_boleto_detalle.fkPerfil;
            req.fkTarifa = (long)obj_boleto_detalle.fkTarifa;
            req.fkStatus = (long)obj_boleto_detalle.fkStatus;
            req.cantidad = (int)obj_boleto_detalle.cantidad;
            req.subtotal = obj_boleto_detalle.subtotal.ToString();
            req.enviado = (int)obj_boleto_detalle.enviado;
            req.confirmadoTISA = (int)obj_boleto_detalle.confirmadoTISA;
            req.created_at = obj_boleto_detalle.created_at;
            req.updated_at = obj_boleto_detalle.updated_at;
            req.deleted_at = obj_boleto_detalle.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/boletosDetalleAlcancia/edit";
            ResBoletosDetalle res = (ResBoletosDetalle)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResBoletosDetalle DeleteBoletoDetalle(sy_boletos_detalle obj_boleto_detalle)
        {
            Api<ResBoletosDetalle> servicio = new Api<ResBoletosDetalle>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqBoletosDetalle req = new ReqBoletosDetalle();
            req.pkBoletoDetalle = obj_boleto_detalle.pkBoletoDetalle;
            req.pkBoletoDetalleTISA = (long)obj_boleto_detalle.pkBoletoDetalleTISA;
            req.fkBoleto = (long)obj_boleto_detalle.fkBoleto;
            req.fkPerfil = (long)obj_boleto_detalle.fkPerfil;
            req.fkTarifa = (long)obj_boleto_detalle.fkTarifa;
            req.fkStatus = (long)obj_boleto_detalle.fkStatus;
            req.cantidad = (int)obj_boleto_detalle.cantidad;
            req.subtotal = obj_boleto_detalle.subtotal.ToString();
            req.enviado = (int)obj_boleto_detalle.enviado;
            req.confirmadoTISA = (int)obj_boleto_detalle.confirmadoTISA;
            req.created_at = obj_boleto_detalle.created_at;
            req.updated_at = obj_boleto_detalle.updated_at;
            req.deleted_at = obj_boleto_detalle.deleted_at;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("BASE_URL");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/boletosDetalleAlcancia/delete";
            ResBoletosDetalle res = (ResBoletosDetalle)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
    }
}
