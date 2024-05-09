using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResConfigVarios
    {
        public bool response { get; set; }
        public config_varios[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResConfigVarios()
        {

        }

        public ResConfigVarios(bool response, config_varios[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
