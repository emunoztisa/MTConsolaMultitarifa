using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceCortes : AbstractService<sy_cortes>
    {
        public override void addEntity(sy_cortes entity)
        {
            em.sy_cortes.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_cortes it = em.sy_cortes.Where(q => (Int64)q.pkCorte == (Int64)pk).First<sy_cortes>();
            if (it == null)
            {
                throw new ArgumentException("Corte no Encontrado");
            }
            else
            {
                em.sy_cortes.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_cortes> getEntities()
        {
            return em.sy_cortes.ToList<sy_cortes>();
        }

        public override sy_cortes getEntity(object pk)
        {
            return em.sy_cortes.Where(q => (Int64)q.pkCorte == (Int64)pk).FirstOrDefault<sy_cortes>();
        }

        public override void updEntity(sy_cortes entity)
        {
            sy_cortes it = em.sy_cortes.Where(q => (Int64)q.pkCorte == (Int64)entity.pkCorte).First<sy_cortes>();
            if (it == null)
            {
                throw new ArgumentException("Corte no Encontrado");
            }
            else
            {
                it.fkAsignacion = entity.fkAsignacion;
                it.fkLugarOrigen = entity.fkLugarOrigen;
                it.fkLugarDestino = entity.fkLugarDestino;
                it.fkStatus = entity.fkStatus;
                it.folio = entity.folio;
                it.fecha = entity.fecha;
                it.hora = entity.hora;
                it.total_efectivo_acumulado = entity.total_efectivo_acumulado;
                it.total_tarifas = entity.total_tarifas;
                it.total_efectivo_rst = entity.total_efectivo_rst;
                it.enviado = entity.enviado;
                it.confirmadoTISA = entity.confirmadoTISA;
                it.modo = entity.modo;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public sy_cortes getEntityLast()
        {
            return em.sy_cortes.OrderByDescending(q => q.pkCorte).FirstOrDefault();
        }
    }
}
