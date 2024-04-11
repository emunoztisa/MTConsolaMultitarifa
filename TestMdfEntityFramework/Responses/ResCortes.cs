using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResCortes
    {
        public bool response { get; set; }
        public sy_cortes[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResCortes()
        {

        }

        public ResCortes(bool response, sy_cortes[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
