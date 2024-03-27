using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResLugares
    {
        public bool response { get; set; }
        public ct_lugares[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResLugares()
        {

        }

        public ResLugares(bool response, ct_lugares[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
