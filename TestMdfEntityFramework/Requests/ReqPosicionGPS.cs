using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    public class ReqPosicionGPS
    {
        public long pkPosicionGPS { get; set; }
        public Nullable<long> pkPosicionGPSTISA { get; set; }
        public Nullable<long> fkAsignacion { get; set; }
        public Nullable<long> fkStatus { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string fecha_hora { get; set; }
        public Nullable<int> enviado { get; set; }
        public Nullable<int> confirmado { get; set; }
        public Nullable<int> modo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        public ReqPosicionGPS()
        {

        }
        public ReqPosicionGPS(
            long pkPosicionGPS, long pkPosicionGPSTISA, long fkAsignacion, long fkStatus
            , string lat, string lng
            , string fecha_hora
            , int enviado, int confirmado, int modo
            , string created_at, string updated_at, string deleted_at)
        {
            this.pkPosicionGPS = pkPosicionGPS;
            this.pkPosicionGPSTISA = pkPosicionGPSTISA;
            this.fkAsignacion = fkAsignacion;
            this.fkStatus = fkStatus;
            this.lat = lat;
            this.lng = lng;
            this.fecha_hora = fecha_hora;
            this.enviado = enviado;
            this.confirmado = confirmado;
            this.modo = modo;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.deleted_at = deleted_at;
        }
    }
}
