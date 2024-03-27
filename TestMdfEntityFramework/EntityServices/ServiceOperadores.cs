using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceOperadores : AbstractService<ct_operadores>
    {
        public override void addEntity(ct_operadores entity)
        {
            em.ct_operadores.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_operadores it = em.ct_operadores.Where(q => (Int64)q.pkOperador == (Int64)pk).First<ct_operadores>();
            if (it == null)
            {
                throw new ArgumentException("Operador no Encontrado");
            }
            else
            {
                em.ct_operadores.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_operadores> getEntities()
        {
            return em.ct_operadores.ToList<ct_operadores>();
        }

        public override ct_operadores getEntity(object pk)
        {
            return em.ct_operadores.Where(q => (Int64)q.pkOperador == (Int64)pk).FirstOrDefault<ct_operadores>();
        }

        public override void updEntity(ct_operadores entity)
        {
            ct_operadores it = em.ct_operadores.Where(q => (Int64)q.pkOperador == (Int64)entity.pkOperador).First<ct_operadores>();
            if (it == null)
            {
                throw new ArgumentException("Operador no Encontrado");
            }
            else
            {
                it.fkEmpresa = entity.fkEmpresa;
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
