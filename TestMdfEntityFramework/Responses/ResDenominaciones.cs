using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResDenominaciones
    {
        public bool response { get; set; }
        public ct_denominaciones[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResDenominaciones()
        {

        }

        public ResDenominaciones(bool response, ct_denominaciones[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
