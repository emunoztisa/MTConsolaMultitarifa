using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResTarifas
    {
        public bool response { get; set; }
        public sy_tarifas[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResTarifas()
        {

        }

        public ResTarifas(bool response, sy_tarifas[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
