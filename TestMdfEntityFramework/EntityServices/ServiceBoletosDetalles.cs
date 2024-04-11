using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceBoletosDetalles : AbstractService<sy_boletos_detalle>
    {
        public override void addEntity(sy_boletos_detalle entity)
        {
            em.sy_boletos_detalle.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_boletos_detalle it = em.sy_boletos_detalle.Where(q => (Int64)q.pkBoletoDetalle == (Int64)pk).First<sy_boletos_detalle>();
            if (it == null)
            {
                throw new ArgumentException("Boleto Detalle no Encontrado");
            }
            else
            {
                em.sy_boletos_detalle.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_boletos_detalle> getEntities()
        {
            return em.sy_boletos_detalle.ToList<sy_boletos_detalle>();
        }

        public override sy_boletos_detalle getEntity(object pk)
        {
            return em.sy_boletos_detalle.Where(q => (Int64)q.pkBoletoDetalle == (Int64)pk).FirstOrDefault<sy_boletos_detalle>();
        }

        public override void updEntity(sy_boletos_detalle entity)
        {
            sy_boletos_detalle it = em.sy_boletos_detalle.Where(q => (Int64)q.pkBoletoDetalle == (Int64)entity.pkBoletoDetalle).First<sy_boletos_detalle>();
            if (it == null)
            {
                throw new ArgumentException("Boleto Detalle no Encontrado");
            }
            else
            {
                it.pkBoletoDetalleTISA = entity.pkBoletoDetalleTISA;
                it.fkBoleto = entity.fkBoleto;
                it.fkPerfil = entity.fkPerfil;
                it.fkTarifa = entity.fkTarifa;
                it.fkStatus = entity.fkStatus;
                it.cantidad = entity.cantidad;
                it.subtotal = entity.subtotal;
                it.enviado = entity.enviado;
                it.confirmadoTISA = entity.confirmadoTISA;
                it.modo = entity.modo;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public List<sy_boletos_detalle> getEntitiesByEnviados()
        {
            return em.sy_boletos_detalle.Where(q => q.enviado == 0).ToList<sy_boletos_detalle>();
        }
        public List<sy_boletos_detalle> getEntitiesByConfirmadosTISA()
        {
            return em.sy_boletos_detalle.Where(q => q.confirmadoTISA == 0).ToList<sy_boletos_detalle>();
        }

        internal List<sy_boletos_detalle> getEntitiesByFkBoleto(long pk)
        {
            return em.sy_boletos_detalle.Where(q => q.fkBoleto == pk).ToList<sy_boletos_detalle>();
        }
    }
}
