using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResEmpresaCorredorOperador
    {
        public bool response { get; set; }
        public sy_empresa_corredor_operador[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResEmpresaCorredorOperador()
        {

        }

        public ResEmpresaCorredorOperador(bool response, sy_empresa_corredor_operador[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
