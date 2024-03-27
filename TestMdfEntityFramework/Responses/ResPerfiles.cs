using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResPerfiles
    {
        public bool response { get; set; }
        public ct_perfiles[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResPerfiles()
        {

        }

        public ResPerfiles(bool response, ct_perfiles[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
