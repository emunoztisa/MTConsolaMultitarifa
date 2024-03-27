using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResLogin
    {
        public bool response { get; set; }
        public string street { get; set; }
        public string token { get; set; }


        public ResLogin()
        {

        }

        public ResLogin(bool response, string street, string token)
        {
            this.response = response;
            this.street = street;
            this.token = token;
        }

        public bool GetResponse()
        {
            return response;
        }
        public void SetResponse(bool val)
        {
            this.response = val;
        }
        public string GetStreet()
        {
            return street;
        }
        public void SetStreet(string val)
        {
            this.street = val;
        }
        public string GetToken()
        {
            return token;
        }
        public void SetToken(string val)
        {
            this.token = val;
        }



    }
}
