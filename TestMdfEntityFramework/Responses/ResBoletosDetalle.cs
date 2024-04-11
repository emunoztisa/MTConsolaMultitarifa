using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResBoletosDetalle
    {
        public bool response { get; set; }
        public sy_boletos_detalle[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResBoletosDetalle()
        {

        }

        public ResBoletosDetalle(bool response, sy_boletos_detalle[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
