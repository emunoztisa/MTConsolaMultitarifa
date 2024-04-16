using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResMensajes_Insert
    {
        public bool response { get; set; }
        public sy_mensajes data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResMensajes_Insert()
        {

        }

        public ResMensajes_Insert(bool response, sy_mensajes data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
