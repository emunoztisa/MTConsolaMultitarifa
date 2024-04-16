using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestMdfEntityFramework.Controllers;
using TestMdfEntityFramework.EntityServices;
using TestMdfEntityFramework.Responses;

namespace TestMdfEntityFramework.Utils
{
    public class Comun
    {
        public Comun()
        {

        }
        public string obtenerValorDeAppConfigV2(string key)
        {
            try
            {
                //string value = _configuration.GetSection(key).Value; //ConfigurationManager.AppSettings[key];
                string value = ConfigurationManager.AppSettings[key];
                return value;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string caption = "EXCEPTION";
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Error;

                MessageBox.Show(message, caption, button, icon);
                return "";
            }

        }

        public string obtenerValorDeAppConfig(string key)
        {
            try
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                //fileMap.ExeConfigFilename = @"C:\dbconsolaalcancia\App.config";
                //fileMap.ExeConfigFilename = @"C:\dbconsolaalcancia\WPF_ConsolaMultitarifa.exe.config";
                fileMap.ExeConfigFilename = @"C:\mt_con_database\App.config";
                System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                string value = config.AppSettings.Settings[key].Value;
                return value;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string caption = "EXCEPTION";
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Error;

                MessageBox.Show(message, caption, button, icon);
                return "";
            }

        }

        public string GetTokenAdmin()
        {
            Comun mc = new Comun();
            LoginController lc = new LoginController();

            string cuenta_admin = mc.obtenerValorDeAppConfig("ADMIN_CONSOLA_ALCANCIA");
            ServiceUsers serviceUsers = new ServiceUsers();
            users user_admin = serviceUsers.getEntityByUser(cuenta_admin);
            string token = user_admin.token != null ? user_admin.token : "";

            //Loguearse en servicio, en caso de no haber un token para al administrador.
            if(token == "")
            {
                string password_desencriptado = mc.DesencriptarCadena(user_admin.contrasena);
                ResLogin resLogin = lc.login(cuenta_admin, password_desencriptado);
                if (resLogin.GetToken() != null && resLogin.GetToken() != "")
                {
                    user_admin.token = resLogin.token;
                    token = resLogin.token;
                }
            }

            return token;
        }

        public static List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row => {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch (Exception ex) { }
                    }
                }
                return objT;
            }).ToList();
        }
        public string EncriptarCadena(string cadena)
        {
            string cadenaEncriptada = "";
            string LLAVE = obtenerValorDeAppConfig("LLAVE");
            cadenaEncriptada = Encode(LLAVE, cadena);
            return cadenaEncriptada;
        }

        public string DesencriptarCadena(string cadena)
        {
            string cadenaDesencriptada = "";
            string LLAVE = obtenerValorDeAppConfig("LLAVE");
            cadenaDesencriptada = Decode(LLAVE, cadena);
            return cadenaDesencriptada;
        }

        public string Encode(string secretKey, string cadena)
        {
            string encriptacion = "";
            try
            {
                MD5 md5 = MD5.Create();
                byte[] llavePassword = md5.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
                byte[] BytesKey = new byte[16];
                Array.Copy(llavePassword, BytesKey, 16);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = BytesKey;
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = des.CreateEncryptor();
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(cadena);
                byte[] buf = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);

                encriptacion = Convert.ToBase64String(buf);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Algo salió mal");
            }
            return encriptacion;
        }

        public string Decode(string secretKey, string cadenaEncriptada)
        {
            string desencriptacion = "";
            try
            {
                byte[] message = Convert.FromBase64String(cadenaEncriptada);

                MD5 md5 = MD5.Create();
                byte[] digestOfPassword = md5.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
                byte[] keyBytes = new byte[16];
                Array.Copy(digestOfPassword, keyBytes, 16);

                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = keyBytes;
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = des.CreateDecryptor();
                byte[] plainText = decryptor.TransformFinalBlock(message, 0, message.Length);

                desencriptacion = Encoding.UTF8.GetString(plainText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Algo salió mal");
            }
            return desencriptacion;
        }

    }
}
