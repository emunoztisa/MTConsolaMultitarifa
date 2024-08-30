using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    public class ReqUsuarios
    {
        public long pkUsuario { get; set; }
        public long fkPuesto { get; set; }
        public long fkStatus { get; set; }
        public string nombre { get; set; }
        public string usuario { get; set; }
        public string contrasena { get; set; }
        public string token { get; set; }
        public string tipo_usuario { get; set; }
        public Nullable<int> enviado { get; set; }
        public Nullable<int> confirmado { get; set; }
        public Nullable<int> modo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
        public Nullable<long> created_id { get; set; }
        public Nullable<long> updated_id { get; set; }
        public Nullable<long> deleted_id { get; set; }

        public ReqUsuarios()
        {

        }

        public ReqUsuarios(long pkUsuario, long fkPuesto, long fkStatus, string nombre, string usuario, string contrasena, string token, string tipo_usuario, int? enviado, int? confirmado, int? modo, string created_at, string updated_at, string deleted_at, long? created_id, long? updated_id, long? deleted_id)
        {
            this.pkUsuario = pkUsuario;
            this.fkPuesto = fkPuesto;
            this.fkStatus = fkStatus;
            this.nombre = nombre;
            this.usuario = usuario;
            this.contrasena = contrasena;
            this.token = token;
            this.tipo_usuario = tipo_usuario;
            this.enviado = enviado;
            this.confirmado = confirmado;
            this.modo = modo;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.deleted_at = deleted_at;
            this.created_id = created_id;
            this.updated_id = updated_id;
            this.deleted_id = deleted_id;
        }
    }
}
