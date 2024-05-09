using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResTarifasMontosFijos
    {
        public bool response { get; set; }
        public ct_tarifas_montos_fijos[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResTarifasMontosFijos()
        {

        }

        public ResTarifasMontosFijos(bool response, ct_tarifas_montos_fijos[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
