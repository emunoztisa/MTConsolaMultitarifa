using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResAsignaciones
    {
        public bool response { get; set; }
        public sy_asignaciones[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResAsignaciones()
        {

        }

        public ResAsignaciones(bool response, sy_asignaciones[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
