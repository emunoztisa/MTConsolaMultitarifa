using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Clases;

namespace TestMdfEntityFramework.Responses
{
    public class ResLogin
    {
        public bool response { get; set; }
        public string street { get; set; }
        public string token { get; set; }
        public Perfiles[] perfiles { get; set; }


        public ResLogin()
        {

        }

        public ResLogin(bool response, string street, string token, Perfiles[] perfiles)
        {
            this.response = response;
            this.street = street;
            this.token = token;
            this.perfiles = perfiles;
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

        public Perfiles[] GetPerfiles()
        {
            return perfiles;
        }
        public void SetPerfiles(Perfiles[] val)
        {
            this.perfiles = val;
        }



    }
}
