using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    public class ReqMensajes
    {
        public long pkMensaje { get; set; }
        public Nullable<long> fkAsignacion { get; set; }
        public Nullable<long> fkStatus { get; set; }
        public string mensaje { get; set; }
        public int enviado { get; set; }
        public int confirmadoTISA { get; set; }
        public string modo { get; set; }
        public int dispositivo_origen { get; set; }
        public int dispositivo_destino { get; set; }
        public int reproducido { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        public ReqMensajes()
        {

        }
        public ReqMensajes(
            long pkMensaje, long fkAsignacion, long fkStatus, string mensaje,
            int enviado, int confirmadoTISA, string modo,
            int dispositivo_origen, int dispositivo_destino, int reproducido,
            string created_at, string updated_at, string deleted_at)
        {
            this.pkMensaje = pkMensaje;
            this.fkAsignacion = fkAsignacion;
            this.fkStatus = fkStatus;
            this.mensaje = mensaje;
            this.enviado = enviado;
            this.confirmadoTISA = confirmadoTISA;
            this.modo = modo;
            this.dispositivo_origen = dispositivo_origen;
            this.dispositivo_destino = dispositivo_destino;
            this.reproducido = reproducido;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.deleted_at = deleted_at;
        }
    }
}
