
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework;

namespace TestMdfEntityFramework.Responses
{

    public class ResUsuariosErpTisa
    {
        public bool response { get; set; }
        public ct_usuarios[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResUsuariosErpTisa()
        {

        }

        public ResUsuariosErpTisa(bool response, ct_usuarios[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }



    public class ResUsuarios
    {
        public bool response { get; set; }
        public ct_usuarios[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResUsuarios()
        {

        }
        public ResUsuarios(bool response, ct_usuarios[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }

    public class ResUsuariosPorPk
    {
        public bool response { get; set; }
        public ct_usuarios data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResUsuariosPorPk()
        {

        }
        public ResUsuariosPorPk(bool response, ct_usuarios data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }

    }

    public class ResUsuarios_Insert
    {
        public bool response { get; set; }
        public ct_usuarios data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResUsuarios_Insert()
        {

        }
        public ResUsuarios_Insert(bool response, ct_usuarios data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }

    }

    public class ResUsuarios_Update
    {
        public bool response { get; set; }
        public ct_usuarios data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResUsuarios_Update()
        {

        }
        public ResUsuarios_Update(bool response, ct_usuarios data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }

    }
}
