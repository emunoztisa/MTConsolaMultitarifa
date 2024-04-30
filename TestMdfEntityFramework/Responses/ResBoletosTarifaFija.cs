using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResBoletosTarifaFija
    {
        public bool response { get; set; }
        public sy_boletos_tarifa_fija[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResBoletosTarifaFija()
        {

        }

        public ResBoletosTarifaFija(bool response, sy_boletos_tarifa_fija[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
