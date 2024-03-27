using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceTarifas : AbstractService<sy_tarifas>
    {
        public override void addEntity(sy_tarifas entity)
        {
            em.sy_tarifas.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_tarifas it = em.sy_tarifas.Where(q => (Int64)q.pkTarifa == (Int64)pk).First<sy_tarifas>();
            if (it == null)
            {
                throw new ArgumentException("Tarifa no Encontrada");
            }
            else
            {
                em.sy_tarifas.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_tarifas> getEntities()
        {
            return em.sy_tarifas.ToList<sy_tarifas>();
        }

        public override sy_tarifas getEntity(object pk)
        {
            return em.sy_tarifas.Where(q => (Int64)q.pkTarifa == (Int64)pk).FirstOrDefault<sy_tarifas>();
        }

        public override void updEntity(sy_tarifas entity)
        {
            sy_tarifas it = em.sy_tarifas.Where(q => (Int64)q.pkTarifa == (Int64)entity.pkTarifa).First<sy_tarifas>();
            if (it == null)
            {
                throw new ArgumentException("Tarifa no Encontrada");
            }
            else
            {
                it.fkLugarOrigen = entity.fkLugarOrigen;
                it.fkLugarDestino = entity.fkLugarDestino;
                it.fkPerfil = entity.fkPerfil;
                it.monto = entity.monto;
                it.status = entity.status;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public sy_tarifas getEntityByLugarOriLugarDesAndPerfil(Int64 fkLugarOri, Int64 fkLugarDes, Int64 fkPerfil)
        {
            return em.sy_tarifas.Where(q => (Int64)q.fkLugarOrigen == (Int64)fkLugarOri)
                                .Where(q => (Int64)q.fkLugarDestino == (Int64)fkLugarDes)
                                .Where(q => (Int64)q.fkPerfil == (Int64)fkPerfil)
                                .FirstOrDefault<sy_tarifas>();
        }

        
    }
}
