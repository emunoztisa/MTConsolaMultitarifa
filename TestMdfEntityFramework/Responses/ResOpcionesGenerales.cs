using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResOpcionesGenerales
    {
        public bool response { get; set; }
        public opciones_generales[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResOpcionesGenerales()
        {

        }

        public ResOpcionesGenerales(bool response, opciones_generales[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
