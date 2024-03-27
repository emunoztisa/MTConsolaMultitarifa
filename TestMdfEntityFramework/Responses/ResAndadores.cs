using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResAndadores
    {
        public bool response { get; set; }
        public ct_andadores[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResAndadores()
        {

        }

        public ResAndadores(bool response, ct_andadores[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
