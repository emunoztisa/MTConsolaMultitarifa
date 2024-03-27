using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceLugarRuta : AbstractService<sy_lugar_ruta>
    {
        public override void addEntity(sy_lugar_ruta entity)
        {
            em.sy_lugar_ruta.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_lugar_ruta it = em.sy_lugar_ruta.Where(q => (Int64)q.pkLugarRuta == (Int64)pk).First<sy_lugar_ruta>();
            if (it == null)
            {
                throw new ArgumentException("Lugar-Ruta no Encontrada");
            }
            else
            {
                em.sy_lugar_ruta.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_lugar_ruta> getEntities()
        {
            return em.sy_lugar_ruta.ToList<sy_lugar_ruta>();
        }

        public override sy_lugar_ruta getEntity(object pk)
        {
            return em.sy_lugar_ruta.Where(q => (Int64)q.pkLugarRuta == (Int64)pk).FirstOrDefault<sy_lugar_ruta>();
        }

        public override void updEntity(sy_lugar_ruta entity)
        {
            sy_lugar_ruta it = em.sy_lugar_ruta.Where(q => (Int64)q.pkLugarRuta == (Int64)entity.pkLugarRuta).First<sy_lugar_ruta>();
            if (it == null)
            {
                throw new ArgumentException("Lugar-Ruta no Encontrada");
            }
            else
            {
                it.fkLugar = entity.fkLugar;
                it.fkRuta = entity.fkRuta;
                it.orden = entity.orden;
                it.status = entity.status;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public List<sy_lugar_ruta> getEntityByFkRuta(object pk)
        {
            return em.sy_lugar_ruta.Where(q => (Int64)q.fkRuta == (Int64)pk).ToList<sy_lugar_ruta>();
        }
    }
}
