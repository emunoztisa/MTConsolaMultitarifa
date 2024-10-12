using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResCuentaCocos
    {
        public bool response { get; set; }
        public sy_conteo_cuenta_cocos[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResCuentaCocos()
        {

        }

        public ResCuentaCocos(bool response, sy_conteo_cuenta_cocos[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }

    public class ResCuentaCocos_Insert
    {
        public bool response { get; set; }
        public sy_conteo_cuenta_cocos data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResCuentaCocos_Insert()
        {

        }

        public ResCuentaCocos_Insert(bool response, sy_conteo_cuenta_cocos data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
