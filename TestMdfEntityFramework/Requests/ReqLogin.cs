﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    class ReqLogin
    {
        public string email { get; set; }
        public string password { get; set; }

        public ReqLogin()
        {

        }
        public ReqLogin(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}
