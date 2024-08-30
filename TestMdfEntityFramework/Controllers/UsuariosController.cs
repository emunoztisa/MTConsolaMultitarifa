using TestMdfEntityFramework.Clases;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestMdfEntityFramework;
using TestMdfEntityFramework.Requests;

namespace TestMdfEntityFramework.Controllers
{
    public class UsuariosController
    {
        private Type T;
        public List<ct_usuarios> Get_MT()
        {
            List<ct_usuarios> list_temp = new List<ct_usuarios>();

            Api<ResUsuariosErpTisa> servicio = new Api<ResUsuariosErpTisa>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/usuarios_consola_multitarifa";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();

            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResUsuariosErpTisa responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResUsuariosErpTisa));
            ResUsuariosErpTisa resp = responseGET_withToken;

            foreach (ct_usuarios item in resp.data)
            {
                ct_usuarios reg = new ct_usuarios();
                reg.pkUsuario = item.pkUsuario;
                reg.fkPuesto = item.fkPuesto;
                reg.fkStatus = item.fkStatus;
                reg.nombre = item.nombre;
                reg.usuario = item.usuario;
                reg.contrasena = item.contrasena;
                reg.token = item.token;
                reg.tipo_usuario = item.tipo_usuario;
                reg.enviado = 1;
                reg.confirmado = 1;
                reg.modo = 1;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                reg.created_id = item.created_id;
                reg.updated_id = item.updated_id;
                reg.deleted_id = item.deleted_id;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public List<ct_usuarios> Get()
        {
            List<ct_usuarios> list_temp = new List<ct_usuarios>();

            Api<ResUsuarios> servicio = new Api<ResUsuarios>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/usuarios";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();

            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResUsuarios responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResUsuarios));
            ResUsuarios resp = responseGET_withToken;

            foreach (ct_usuarios item in resp.data)
            {
                ct_usuarios reg = new ct_usuarios();
                reg.pkUsuario = item.pkUsuario;
                reg.fkPuesto = item.fkPuesto;
                reg.fkStatus = item.fkStatus;
                reg.nombre = item.nombre;
                reg.usuario = item.usuario;
                reg.contrasena = item.contrasena;
                reg.token = item.token;
                reg.tipo_usuario = item.tipo_usuario;
                reg.enviado = item.enviado;
                reg.confirmado = item.confirmado;
                reg.modo = item.modo;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                reg.created_id = item.created_id;
                reg.updated_id = item.updated_id;
                reg.deleted_id = item.deleted_id;
                list_temp.Add(reg);
            }

            return list_temp;
        }
        public List<ct_usuarios> GetPorPk(long pk)
        {
            List<ct_usuarios> list_temp = new List<ct_usuarios>();

            Api<ResUsuarios> servicio = new Api<ResUsuarios>();
            Comun mc = new Comun();

            string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
            string metodo_web = "mt/usuario";
            string token = mc.GetTokenAdmin();

            Dictionary<string, string> headers = new Dictionary<string, string>();

            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            ReqUsuarios req = new ReqUsuarios();
            req.pkUsuario = pk;

            // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
            ResUsuarios responseGET_withToken = servicio.RequestPost_withToken(base_url, metodo_web, req, headers, typeof(ResUsuarios));
            ResUsuarios resp = responseGET_withToken;

            foreach (ct_usuarios item in resp.data)
            {
                ct_usuarios reg = new ct_usuarios();
                reg.pkUsuario = item.pkUsuario;
                reg.fkPuesto = item.fkPuesto;
                reg.fkStatus = item.fkStatus;
                reg.nombre = item.nombre;
                reg.usuario = item.usuario;
                reg.contrasena = item.contrasena;
                reg.token = item.token;
                reg.tipo_usuario = item.tipo_usuario;
                reg.enviado = item.enviado;
                reg.confirmado = item.confirmado;
                reg.modo = item.modo;
                reg.created_at = item.created_at;
                reg.updated_at = item.updated_at;
                reg.deleted_at = item.deleted_at;
                reg.created_id = item.created_id;
                reg.updated_id = item.updated_id;
                reg.deleted_id = item.deleted_id;
                list_temp.Add(reg);
            }

            return list_temp;

        }
        public ResUsuarios_Insert Insert(ct_usuarios obj)
        {
            Api<ResUsuarios_Insert> servicio = new Api<ResUsuarios_Insert>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto request de boleto
            ReqUsuarios req = new ReqUsuarios();
            req.pkUsuario = obj.pkUsuario;
            req.fkPuesto = obj.fkPuesto;
            req.fkStatus = obj.fkStatus;
            req.nombre = obj.nombre;
            req.usuario = obj.usuario;
            req.contrasena = obj.contrasena;
            req.token = obj.token;
            req.tipo_usuario = obj.tipo_usuario;
            req.enviado = obj.enviado;
            req.confirmado = obj.confirmado;
            req.modo = obj.modo;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;
            req.created_id = obj.created_id;
            req.updated_id = obj.updated_id;
            req.deleted_id = obj.deleted_id;

            //Consumir servicio
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("URL_BASE");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/usuario/register";
            ResUsuarios_Insert res = (ResUsuarios_Insert)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResUsuarios_Update Update(ct_usuarios obj)
        {
            Api<ResUsuarios_Update> servicio = new Api<ResUsuarios_Update>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto
            ReqUsuarios req = new ReqUsuarios();
            req.pkUsuario = obj.pkUsuario;
            req.fkPuesto = obj.fkPuesto;
            req.fkStatus = obj.fkStatus;
            req.nombre = obj.nombre;
            req.usuario = obj.usuario;
            req.contrasena = obj.contrasena;
            req.token = obj.token;
            req.tipo_usuario = obj.tipo_usuario;
            req.enviado = obj.enviado;
            req.confirmado = obj.confirmado;
            req.modo = obj.modo;
            req.created_at = obj.created_at;
            req.updated_at = obj.updated_at;
            req.deleted_at = obj.deleted_at;
            req.created_id = obj.created_id;
            req.updated_id = obj.updated_id;
            req.deleted_id = obj.deleted_id;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("URL_BASE");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/usuario/edit";
            ResUsuarios_Update res = (ResUsuarios_Update)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
        public ResUsuarios Delete(ct_usuarios obj)
        {
            Api<ResUsuarios> servicio = new Api<ResUsuarios>();
            Comun mc = new Comun();

            string token = mc.GetTokenAdmin();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list.Add("Bearer " + token);
            headers.Add("Authorization", list[0]);

            //Construccion objeto
            ReqUsuarios req = new ReqUsuarios();
            req.pkUsuario = obj.pkUsuario;
            req.deleted_at = obj.deleted_at;
            req.deleted_id = obj.deleted_id;

            //Consumir servicio de boleto
            ServiceConfigVarios serv_cv = new ServiceConfigVarios();
            config_varios cv_baseurl = serv_cv.getEntityByClave("URL_BASE");
            string baseurl = cv_baseurl.valor;
            string metodo = "mt/usuario/delete";
            ResUsuarios res = (ResUsuarios)servicio.RequestPost_withToken(baseurl, metodo, req, headers, T);

            return res;
        }
    }
}
