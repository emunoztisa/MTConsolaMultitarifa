using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    public class ReqUbicacion
    {
        public long pkUbicacion { get; set; }
        public Nullable<long> fkAsignacion { get; set; }
        public decimal latitud { get; set; }
        public decimal longitud { get; set; }
        public int enviado { get; set; }
        public int confirmadoTISA { get; set; }
        public string modo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        public ReqUbicacion()
        {

        }
        public ReqUbicacion(
              long pkUbicacion
            , long fkAsignacion
            , decimal latitud
            , decimal longitud
            , int enviado
            , int confirmadoTISA
            , string modo
            , string created_at
            , string updated_at
            , string deleted_at
            )
        {
            this.pkUbicacion = pkUbicacion;
            this.fkAsignacion = fkAsignacion;
            this.latitud = latitud;
            this.longitud = longitud;
            this.enviado = enviado;
            this.confirmadoTISA = confirmadoTISA;
            this.modo = modo;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.deleted_at = deleted_at;
        }
    }
}
