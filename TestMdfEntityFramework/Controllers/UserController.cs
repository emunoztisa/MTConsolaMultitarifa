using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Clases;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Responses;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.Controllers
{
    public class UserController
    {
        private string _token;
        private Type T;

        private bool LoginAdmin()
        {
            bool loginSuccess = false;
            try
            {
                Api<ResUsers> servicio = new Api<ResUsers>();
                Comun mc = new Comun();

                string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
                //string metodo_web = "mt/usuariosConsolaAlcancia";

                string cuenta_admin = mc.obtenerValorDeAppConfig("ADMIN_CONSOLA_ALCANCIA");
                string pass_admin = mc.obtenerValorDeAppConfig("ADMIN_CONSOLA_ALCANCIA_PASS");

                // Enviar solicitud para loguearse como administrador y obtener token
                LoginController lc = new LoginController();
                ResLogin resLogin = lc.login(cuenta_admin, pass_admin);

                // Guardar las credenciales de administrador en la base de datos local: user, contraseña y token, para posteriores consultas.
                if (resLogin.response)
                {
                    users us = new users();
                    us.user = cuenta_admin;
                    us.contrasena = pass_admin != null && pass_admin != "" ? mc.EncriptarCadena(pass_admin) : "";
                    us.token = resLogin.GetToken();
                    us.created_at = "";
                    us.updated_at = "";
                    us.deleted_at = "";

                    //creamos la conexion a la base mdf
                    ServiceUsers serviceUsers = new ServiceUsers();
                    serviceUsers.addEntity(us);

                    loginSuccess = true;
                }
                return loginSuccess;

            }
            catch (Exception ex)
            {
                return loginSuccess;
            }
        }
        private bool ValidaExisteAdminEnDbLocal()
        {
            bool val = false;
            Comun mc = new Comun();

            string cuenta_admin = mc.obtenerValorDeAppConfig("ADMIN_CONSOLA_ALCANCIA");

            ServiceUsers serviceUsers = new ServiceUsers();
            users user_existe = serviceUsers.getEntityByUser(cuenta_admin);
            
            if (user_existe != null && user_existe.user != "")
            {
                val = true;
            }
            return val;
        }
        private string GetTokenAdmin()
        {
            Comun mc = new Comun();
            string cuenta_admin = mc.obtenerValorDeAppConfig("ADMIN_CONSOLA_ALCANCIA");
            ServiceUsers serviceUsers = new ServiceUsers();
            users user_admin = serviceUsers.getEntityByUser(cuenta_admin);
            string token = user_admin.token != null ? user_admin.token : "";
            return token;
        }
        public List<User> GetUsers()
        {
            List<User> list_temp = new List<User>();
            // Validar si ya hay informacion en la base de datos local de un usuario administrador de consola alcancia.
            // En caso de no existir se realiza el login sino se consulta de la base local los datos del admin.
            if (ValidaExisteAdminEnDbLocal())
            {
                //SI EXISTE ADMIN
                Api<ResUsers> servicio = new Api<ResUsers>();
                Comun mc = new Comun();

                string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
                string metodo_web = "mt/usuariosConsolaAlcancia";
                string token = GetTokenAdmin();

                Dictionary<string, string> headers = new Dictionary<string, string>();
                List<string> list = new List<string>();
                list.Add("Bearer " + token);
                headers.Add("Authorization", list[0]);

                // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
                ResUsers responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResUsers));
                ResUsers resp = responseGET_withToken;

                //foreach (ResponseLugares.Data data1 in data)
                foreach (User item in resp.data)
                {
                    User reg = new User();
                    reg.email = item.email; // user
                    reg.electoralid = item.electoralid; // contrasena
                    reg.password = item.password; //
                    reg.m_surname = item.m_surname;
                    reg.created_at = item.created_at;
                    reg.updated_at = item.updated_at;
                    reg.deleted_at = item.deleted_at;
                    list_temp.Add(reg);
                }
            }
            else
            {  // NO EXISTE ADMIN
                bool loginAdmin = LoginAdmin();
                if (loginAdmin)
                {
                    if (ValidaExisteAdminEnDbLocal())
                    {
                        //SI EXISTE ADMIN
                        Api<ResUsers> servicio = new Api<ResUsers>();
                        Comun mc = new Comun();

                        string base_url = mc.obtenerValorDeAppConfig("URL_BASE");
                        string metodo_web = "mt/usuariosConsolaAlcancia";
                        string token = GetTokenAdmin();

                        Dictionary<string, string> headers = new Dictionary<string, string>();
                        List<string> list = new List<string>();
                        list.Add("Bearer " + token);
                        headers.Add("Authorization", list[0]);

                        // Realizar el request al servicio de solo los usuarios de multitarifa consola alcancia
                        ResUsers responseGET_withToken = servicio.RequestGet_withToken(base_url, metodo_web, headers, typeof(ResUsers));
                        ResUsers resp = responseGET_withToken;

                        //foreach (ResponseLugares.Data data1 in data)
                        foreach (User item in resp.data)
                        {
                            User reg = new User();
                            reg.email = item.email; // user
                            reg.electoralid = item.electoralid; // contrasena
                            reg.password = item.password; //
                            reg.m_surname = item.m_surname;
                            reg.created_at = item.created_at;
                            reg.updated_at = item.updated_at;
                            reg.deleted_at = item.deleted_at;
                            list_temp.Add(reg);
                        }
                    }

                }
            }


            return list_temp;
        }

        //private string GetTokenAdmin()
        //{
        //    return "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyMiIsImp0aSI6IjM0NTJjYTVkZjhiYmFiMWRmMzRhZmJiN2U5ODgwODIyYWIzNTVhYzIwMWIxYzNmNDQ4N2NkOTBhMTViM2U4NGQxZDcwZjQ4YzQzNjYyMTg4IiwiaWF0IjoxNzEwMzkwNDk4LCJuYmYiOjE3MTAzOTA0OTgsImV4cCI6MTc0MTkyNjQ5OCwic3ViIjoiMjEiLCJzY29wZXMiOltdfQ.m3B73bhJG8SbGYREegil0qDipaGoRp3Ur31LDI41RPM7jq-rh2b4WuRQAU_4QojcMmV5y0p7jDg7UeUUQkWeNCVNFnPHMjI6_x0DchD1SJXTo-DAQ3CjU_6LlSwdEn0i87-uKQ_sGm68ucqAanDuYdTR-8vtYlA0-5mUgilahBGUm2cFosQuR6Ui9xwEf7Q9ICUPgL0gOxwInnLsDGJTJ52WF6qc0LHobE9LAukQlToTkL-qah681TypNSc36kDjh1lHtfh6vrOMI3DZLPYTjT8jcnqo9hu91-OZLGeJZRNgO_KaSHn7O8v1dRpVNq2gDKTHwDha9hJDUZLEYdeiGtQjsiaaEzRWqKUrqFYvFkSROww1OIbj6X-m5r9iJeBhd9EcxV5jtv1WWFo0OJ1tUGBaPOfOAQiqbnXVVh6Vm6xcCdgntMJInZq0wy02mFu0Xj4zLmBppW05-uAVrnglgG1-_c5EWUn2IIbxYeP7RqHbwsd6RdIsKCTYyZAUpST-0EWvtvRr-uMFk34eEbIqREC7-7ytn-eMjukgGYcMiW104sD2KS5b2LyNr4kNqIhn8fouxLrK_lU8fJrOpZ93NAxDwFH5vgcKSks9XD79gATEqbHhccwt_XeuxQBoQI_X7tGS_Na0rWmRoGm7LNB_6n1gwqiEhWGS-6xEQCKa7Go";
        //}
    }
}
