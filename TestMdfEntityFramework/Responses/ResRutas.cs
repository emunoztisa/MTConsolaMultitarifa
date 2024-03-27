using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResRutas
    {
        public bool response { get; set; }
        public ct_rutas[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResRutas()
        {

        }

        public ResRutas(bool response, ct_rutas[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
