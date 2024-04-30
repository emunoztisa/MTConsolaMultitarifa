using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    public class ReqBoletosTarifaFija
    {
        public long pkBoleto { get; set; }
        public Nullable<long> pkBoletoTISA { get; set; }
        public Nullable<long> fkAsignacion { get; set; }
        public Nullable<long> fkStatus { get; set; }
        public string folio { get; set; }
        public string tarifa { get; set; }
        public int cant_pasajeros { get; set; }
        public string total { get; set; }
        public string totalCobrado { get; set; }
        public string totalPagado { get; set; }
        public string fechaHoraCancelacion { get; set; }
        public int enviado { get; set; }
        public int confirmadoTISA { get; set; }
        public string modo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        public ReqBoletosTarifaFija()
        {

        }
        public ReqBoletosTarifaFija(
            long pkBoleto, long pkBoletoTISA, long fkAsignacion, long fkLugarOrigen, long fkLugarDestino, long fkStatus
            , string folio, string tarifa, int cant_pasajeros, string total
            , string totalCobrado, string totalPagado, string fechaHoraCancelacion
            , int enviado, int confirmadoTISA, string modo, string created_at, string updated_at, string deleted_at)
        {
            this.pkBoleto = pkBoleto;
            this.pkBoletoTISA = pkBoletoTISA;
            this.fkAsignacion = fkAsignacion;
            this.fkStatus = fkStatus;
            this.folio = folio;
            this.tarifa = tarifa;
            this.cant_pasajeros = cant_pasajeros;
            this.total = total;
            this.totalCobrado = totalCobrado;
            this.totalPagado = totalPagado;
            this.fechaHoraCancelacion = fechaHoraCancelacion;
            this.enviado = enviado;
            this.confirmadoTISA = confirmadoTISA;
            this.modo = modo;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.deleted_at = deleted_at;
        }
    }
}
