using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceCorredores : AbstractService<ct_corredores>
    {
        public override void addEntity(ct_corredores entity)
        {
            em.ct_corredores.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_corredores it = em.ct_corredores.Where(q => (Int64)q.pkCorredor == (Int64)pk).First<ct_corredores>();
            if (it == null)
            {
                throw new ArgumentException("Corredor no Encontrado");
            }
            else
            {
                em.ct_corredores.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_corredores> getEntities()
        {
            return em.ct_corredores.ToList<ct_corredores>();
        }

        public override ct_corredores getEntity(object pk)
        {
            return em.ct_corredores.Where(q => (Int64)q.pkCorredor == (Int64)pk).FirstOrDefault<ct_corredores>();
        }

        public override void updEntity(ct_corredores entity)
        {
            ct_corredores it = em.ct_corredores.Where(q => (Int64)q.pkCorredor == (Int64)entity.pkCorredor).First<ct_corredores>();
            if (it == null)
            {
                throw new ArgumentException("Corredor no Encontrado");
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

        public List<ct_corredores> getEntityPorFkEmpresa(object fkEmpresa)
        {
            return em.ct_corredores.Where(q => q.fkEmpresa == (Int64)fkEmpresa).ToList<ct_corredores>();
        }

        public ct_corredores getEntityPorNombreCorredor(string nombre_corredor)
        {
            return em.ct_corredores.Where(q => q.nombre == nombre_corredor).FirstOrDefault<ct_corredores>();
        }
    }
}
