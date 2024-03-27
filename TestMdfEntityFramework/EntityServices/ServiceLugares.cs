using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceLugares : AbstractService<ct_lugares>
    {
        public override void addEntity(ct_lugares entity)
        {
            em.ct_lugares.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_lugares it = em.ct_lugares.Where(q => (Int64)q.pkLugar == (Int64)pk).First<ct_lugares>();
            if (it == null)
            {
                throw new ArgumentException("Lugar no Encontrado");
            }
            else
            {
                em.ct_lugares.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_lugares> getEntities()
        {
            return em.ct_lugares.ToList<ct_lugares>();
        }

        public override ct_lugares getEntity(object pk)
        {
            return em.ct_lugares.Where(q => (Int64)q.pkLugar == (Int64)pk).FirstOrDefault<ct_lugares>();
        }

        public override void updEntity(ct_lugares entity)
        {
            ct_lugares it = em.ct_lugares.Where(q => (Int64)q.pkLugar == (Int64)entity.pkLugar).First<ct_lugares>();
            if (it == null)
            {
                throw new ArgumentException("Lugar no Encontrado");
            }
            else
            {
                it.nombre = entity.nombre;
                it.status = entity.status;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public ct_lugares getEntityByNombre(object nombre)
        {
            return em.ct_lugares.Where(q => (string)q.nombre == (string)nombre).FirstOrDefault<ct_lugares>();
        }

        
    }
}
