using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    public class ReqUnidades
    {
        public long pkUnidad { get; set; }
        public Nullable<long> fkEmpresa { get; set; }
        public Nullable<long> fkCorredor { get; set; }
        public string nombre { get; set; }
        public string noSerieAVL { get; set; }
        public string economico { get; set; }
        public int capacidad { get; set; }
        public string validador { get; set; }
        public int status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        public ReqUnidades()
        {

        }
        public ReqUnidades(
            long pkUnidad, long fkEmpresa, long fkCorredor
            , string nombre, string noSerieAVL, string economico, int capacidad, string validador, int status
            , string created_at, string updated_at, string deleted_at)
        {
            this.pkUnidad = pkUnidad;
            this.fkEmpresa = fkEmpresa;
            this.fkCorredor = fkCorredor;
            this.nombre = nombre;
            this.noSerieAVL = noSerieAVL;
            this.economico = economico;
            this.capacidad = capacidad;
            this.validador = validador;
            this.status = status;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.deleted_at = deleted_at;
        }
    }
}
