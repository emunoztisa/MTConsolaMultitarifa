using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceEmpresas : AbstractService<ct_empresas>
    {
        public override void addEntity(ct_empresas entity)
        {
            em.ct_empresas.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_empresas it = em.ct_empresas.Where(q => (Int64)q.pkEmpresa == (Int64)pk).First<ct_empresas>();
            if (it == null)
            {
                throw new ArgumentException("Empresa no Encontrado");
            }
            else
            {
                em.ct_empresas.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_empresas> getEntities()
        {
            return em.ct_empresas.ToList<ct_empresas>();
        }

        public override ct_empresas getEntity(object pk)
        {
            return em.ct_empresas.Where(q => (Int64)q.pkEmpresa == (Int64)pk).FirstOrDefault<ct_empresas>();
        }

        public override void updEntity(ct_empresas entity)
        {
            ct_empresas it = em.ct_empresas.Where(q => (Int64)q.pkEmpresa == (Int64)entity.pkEmpresa).First<ct_empresas>();
            if (it == null)
            {
                throw new ArgumentException("Empresa no Encontrada");
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
    }
}
