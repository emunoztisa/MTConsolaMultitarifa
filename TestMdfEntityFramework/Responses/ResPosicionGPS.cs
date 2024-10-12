using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Responses
{
    public class ResPosicionGPS
    {
        public bool response { get; set; }
        public sy_posicion_gps[] data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResPosicionGPS()
        {

        }

        public ResPosicionGPS(bool response, sy_posicion_gps[] data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }

    public class ResPosicionGPS_Insert
    {
        public bool response { get; set; }
        public sy_posicion_gps data { get; set; }
        public string message { get; set; }
        public int status { get; set; }

        public ResPosicionGPS_Insert()
        {

        }

        public ResPosicionGPS_Insert(bool response, sy_posicion_gps data, string message, int status)
        {
            this.response = response;
            this.data = data;
            this.message = message;
            this.status = status;
        }
    }
}
