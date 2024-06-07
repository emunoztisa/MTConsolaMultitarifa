using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Requests
{
    public class ReqCortes
    {
        public long pkCorte { get; set; }
        public Nullable<long> fkAsignacion { get; set; }
        public Nullable<long> fkLugarOrigen { get; set; }
        public Nullable<long> fkLugarDestino { get; set; }
        public Nullable<long> fkStatus { get; set; }
        public string folio { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public decimal total_efectivo_acumulado { get; set; }
        public decimal total_tarifas { get; set; }
        public decimal total_efectivo_rst { get; set; }

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



        public int enviado { get; set; }
        public int confirmadoTISA { get; set; }
        public string modo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        public ReqCortes()
        {

        }
        public ReqCortes(
            long pkCorte, long fkAsignacion, long fkLugarOrigen, long fkLugarDestino, long fkStatus
            , string folio, string fecha, string hora
            , decimal total_efectivo_acumulado, decimal total_tarifas, decimal total_efectivo_rst
            , string efectivo_moneda, string efectivo_billete
            , string cant_mon_tipo_0, string cant_mon_tipo_1, string cant_mon_tipo_2, string cant_mon_tipo_3, string cant_mon_tipo_4, string cant_mon_tipo_5, string cant_mon_tipo_6
            , string cant_bill_tipo_0, string cant_bill_tipo_1, string cant_bill_tipo_2, string cant_bill_tipo_3, string cant_bill_tipo_4, string cant_bill_tipo_5, string cant_bill_tipo_6
            , int enviado, int confirmadoTISA, string modo
            , string created_at, string updated_at, string deleted_at)
        {
            this.pkCorte = pkCorte;
            this.fkAsignacion = fkAsignacion;
            this.fkLugarOrigen = fkLugarOrigen;
            this.fkLugarDestino = fkLugarDestino;
            this.fkStatus = fkStatus;
            this.folio = folio;
            this.fecha = fecha;
            this.hora = hora;
            this.total_efectivo_acumulado = total_efectivo_acumulado;
            this.total_tarifas = total_tarifas;
            this.total_efectivo_rst = total_efectivo_rst;

            this.efectivo_moneda = efectivo_moneda;
            this.efectivo_billete = efectivo_billete;

            this.cant_mon_tipo_0 = cant_mon_tipo_0;
            this.cant_mon_tipo_1 = cant_mon_tipo_1;
            this.cant_mon_tipo_2 = cant_mon_tipo_2;
            this.cant_mon_tipo_3 = cant_mon_tipo_3;
            this.cant_mon_tipo_4 = cant_mon_tipo_4;
            this.cant_mon_tipo_5 = cant_mon_tipo_5;
            this.cant_mon_tipo_6 = cant_mon_tipo_6;

            this.cant_bill_tipo_0 = cant_bill_tipo_0;
            this.cant_bill_tipo_1 = cant_bill_tipo_1;
            this.cant_bill_tipo_2 = cant_bill_tipo_2;
            this.cant_bill_tipo_3 = cant_bill_tipo_3;
            this.cant_bill_tipo_4 = cant_bill_tipo_4;
            this.cant_bill_tipo_5 = cant_bill_tipo_5;
            this.cant_bill_tipo_6 = cant_bill_tipo_6;

            this.enviado = enviado;
            this.confirmadoTISA = confirmadoTISA;
            this.modo = modo;

            this.created_at = created_at;
            this.updated_at = updated_at;
            this.deleted_at = deleted_at;
        }
    }
}
