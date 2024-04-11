using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    public class ReqBoletosDetalle
    {
        public long pkBoletoDetalle { get; set; }
        public Nullable<long> pkBoletoDetalleTISA { get; set; }
        public Nullable<long> fkBoleto { get; set; }
        public Nullable<long> fkPerfil { get; set; }
        public Nullable<long> fkTarifa { get; set; }
        public Nullable<long> fkStatus { get; set; }
        public int cantidad { get; set; }
        public string subtotal { get; set; }
        public int enviado { get; set; }
        public int confirmadoTISA { get; set; }
        public string modo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        public ReqBoletosDetalle()
        {

        }
        public ReqBoletosDetalle(
            long pkBoletoDetalle, long pkBoletoDetalleTISA, long fkBoleto, long fkPerfil, long fkTarifa, long fkStatus
            , int cantidad, string subtotal, int enviado, int confirmadoTISA, string created_at, string updated_at, string deleted_at)
        {
            this.pkBoletoDetalle = pkBoletoDetalle;
            this.pkBoletoDetalleTISA = pkBoletoDetalleTISA;
            this.fkBoleto = fkBoleto;
            this.fkPerfil = fkPerfil;
            this.fkTarifa = fkTarifa;
            this.fkStatus = fkStatus;
            this.cantidad = cantidad;
            this.subtotal = subtotal;
            this.enviado = enviado;
            this.confirmadoTISA = confirmadoTISA;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.deleted_at = deleted_at;
        }
    }
}
