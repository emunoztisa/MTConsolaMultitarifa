//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestMdfEntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class sy_cortes
    {
        public long pkCorte { get; set; }
        public Nullable<long> pkCorteTISA { get; set; }
        public Nullable<long> fkAsignacion { get; set; }
        public Nullable<long> fkLugarOrigen { get; set; }
        public Nullable<long> fkLugarDestino { get; set; }
        public Nullable<long> fkStatus { get; set; }
        public string folio { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public Nullable<decimal> total_efectivo_acumulado { get; set; }
        public Nullable<decimal> total_tarifas { get; set; }
        public Nullable<decimal> total_efectivo_rst { get; set; }
        public string efectivo_moneda { get; set; }
        public string efectivo_billete { get; set; }
        public string cant_mon_tipo_0 { get; set; }
        public string cant_mon_tipo_1 { get; set; }
        public string cant_mon_tipo_2 { get; set; }
        public string cant_mon_tipo_3 { get; set; }
        public string cant_mon_tipo_4 { get; set; }
        public string cant_mon_tipo_5 { get; set; }
        public string cant_mon_tipo_6 { get; set; }
        public string cant_bill_tipo_0 { get; set; }
        public string cant_bill_tipo_1 { get; set; }
        public string cant_bill_tipo_2 { get; set; }
        public string cant_bill_tipo_3 { get; set; }
        public string cant_bill_tipo_4 { get; set; }
        public string cant_bill_tipo_5 { get; set; }
        public string cant_bill_tipo_6 { get; set; }
        public Nullable<int> enviado { get; set; }
        public Nullable<int> confirmadoTISA { get; set; }
        public string modo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
