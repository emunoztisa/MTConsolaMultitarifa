using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceAndadores : AbstractService<ct_andadores>
    {
        public override void addEntity(ct_andadores entity)
        {
            em.ct_andadores.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_andadores it = em.ct_andadores.Where(q => (Int64)q.pkAndador == (Int64)pk).First<ct_andadores>();
            if (it == null)
            {
                throw new ArgumentException("Andador no Encontrado");
            }
            else
            {
                em.ct_andadores.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_andadores> getEntities()
        {
            return em.ct_andadores.ToList<ct_andadores>();
        }

        public override ct_andadores getEntity(object pk)
        {
            return em.ct_andadores.Where(q => (Int64)q.pkAndador == (Int64)pk).FirstOrDefault<ct_andadores>();
        }

        public override void updEntity(ct_andadores entity)
        {
            ct_andadores it = em.ct_andadores.Where(q => (Int64)q.pkAndador == (Int64)entity.pkAndador).First<ct_andadores>();
            if (it == null)
            {
                throw new ArgumentException("Andador no Encontrado");
            }
            else
            {
                it.fkLugar = entity.fkLugar;
                it.fkStatus = entity.fkStatus;
                it.nombre = entity.nombre;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
    }
}
