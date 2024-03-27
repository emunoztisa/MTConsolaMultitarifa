using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResUnidades
    {
        public bool response { get; set; }
        public ct_unidades[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResUnidades()
        {

        }

        public ResUnidades(bool response, ct_unidades[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
