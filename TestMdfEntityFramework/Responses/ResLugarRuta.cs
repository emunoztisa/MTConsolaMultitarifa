using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResLugarRuta
    {
        public bool response { get; set; }
        public sy_lugar_ruta[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResLugarRuta()
        {

        }

        public ResLugarRuta(bool response, sy_lugar_ruta[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
