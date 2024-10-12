using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    public class ReqCuentaCocos
    {
        public long pkConteoCuentaCocos { get; set; }
        public Nullable<long> pkConteoCuentaCocosTISA { get; set; }
        public Nullable<long> fkAsignacion { get; set; }
        public Nullable<long> fkStatus { get; set; }
        public Nullable<long> cc1_subidas { get; set; }
        public Nullable<long> cc1_bajadas { get; set; }
        public Nullable<long> cc2_subidas { get; set; }
        public Nullable<long> cc2_bajadas { get; set; }
        public Nullable<long> cc3_subidas { get; set; }
        public Nullable<long> cc3_bajadas { get; set; }
        public string fecha_hora { get; set; }
        public Nullable<int> enviado { get; set; }
        public Nullable<int> confirmado { get; set; }
        public Nullable<int> modo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        public ReqCuentaCocos()
        {

        }
        public ReqCuentaCocos(
            long pkConteoCuentaCocos, long pkConteoCuentaCocosTISA, long fkAsignacion, long fkStatus
            , long cc1_subidas, long cc1_bajadas
            , long cc2_subidas, long cc2_bajadas
            , long cc3_subidas, long cc3_bajadas
            , string fecha_hora
            , int enviado, int confirmado, int modo
            , string created_at, string updated_at, string deleted_at)
        {
            this.pkConteoCuentaCocos = pkConteoCuentaCocos;
            this.pkConteoCuentaCocosTISA = pkConteoCuentaCocosTISA;
            this.fkAsignacion = fkAsignacion;
            this.fkStatus = fkStatus;
            this.cc1_subidas = cc1_subidas;
            this.cc1_bajadas = cc1_bajadas;
            this.cc2_subidas = cc2_subidas;
            this.cc2_bajadas = cc2_bajadas;
            this.cc3_subidas = cc3_subidas;
            this.cc3_bajadas = cc3_bajadas;
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
