using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResBoletos_Insert
    {
        public bool response { get; set; }
        public sy_boletos data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResBoletos_Insert()
        {

        }

        public ResBoletos_Insert(bool response, sy_boletos data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
