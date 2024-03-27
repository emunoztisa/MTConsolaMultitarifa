using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResEmpresas
    {
        public bool response { get; set; }
        public ct_empresas[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResEmpresas()
        {

        }

        public ResEmpresas(bool response, ct_empresas[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
