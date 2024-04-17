using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResUbicacion
    {
        public bool response { get; set; }
        public sy_ubicacion[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResUbicacion()
        {

        }

        public ResUbicacion(bool response, sy_ubicacion[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
