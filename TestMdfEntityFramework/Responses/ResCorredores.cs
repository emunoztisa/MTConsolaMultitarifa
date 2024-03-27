using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResCorredores
    {
        public bool response { get; set; }
        public ct_corredores[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResCorredores()
        {

        }

        public ResCorredores(bool response, ct_corredores[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
