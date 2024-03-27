using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Clases;

namespace TestMdfEntityFramework.Responses
{
    public class ResUsers
    {
        public bool response { get; set; }
        public User[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResUsers()
        {

        }

        public ResUsers(bool response, User[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
