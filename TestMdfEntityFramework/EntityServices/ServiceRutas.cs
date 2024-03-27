using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceRutas : AbstractService<ct_rutas>
    {
        public override void addEntity(ct_rutas entity)
        {
            em.ct_rutas.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_rutas it = em.ct_rutas.Where(q => (Int64)q.pkRuta == (Int64)pk).First<ct_rutas>();
            if (it == null)
            {
                throw new ArgumentException("Ruta no Encontrada");
            }
            else
            {
                em.ct_rutas.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_rutas> getEntities()
        {
            return em.ct_rutas.ToList<ct_rutas>();
        }

        public override ct_rutas getEntity(object pk)
        {
            return em.ct_rutas.Where(q => (Int64)q.pkRuta == (Int64)pk).FirstOrDefault<ct_rutas>();
        }

        public override void updEntity(ct_rutas entity)
        {
            ct_rutas it = em.ct_rutas.Where(q => (Int64)q.pkRuta == (Int64)entity.pkRuta).First<ct_rutas>();
            if (it == null)
            {
                throw new ArgumentException("Ruta no Encontrada");
            }
            else
            {
                it.fkCorredor = entity.fkCorredor;
                it.nombre = entity.nombre;
                it.status = entity.status;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
    }
}
