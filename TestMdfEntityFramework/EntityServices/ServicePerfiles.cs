using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServicePerfiles : AbstractService<ct_perfiles>
    {
        public override void addEntity(ct_perfiles entity)
        {
            em.ct_perfiles.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_perfiles it = em.ct_perfiles.Where(q => (Int64)q.pkPerfil == (Int64)pk).First<ct_perfiles>();
            if (it == null)
            {
                throw new ArgumentException("Perfil no Encontrado");
            }
            else
            {
                em.ct_perfiles.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_perfiles> getEntities()
        {
            return em.ct_perfiles.Where(q => q.deleted_at == null).ToList<ct_perfiles>();
        }

        public override ct_perfiles getEntity(object pk)
        {
            return em.ct_perfiles.Where(q => (Int64)q.pkPerfil == (Int64)pk).FirstOrDefault<ct_perfiles>();
        }

        public override void updEntity(ct_perfiles entity)
        {
            ct_perfiles it = em.ct_perfiles.Where(q => (Int64)q.pkPerfil == (Int64)entity.pkPerfil).First<ct_perfiles>();
            if (it == null)
            {
                throw new ArgumentException("Perfil no Encontrado");
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
        public ct_perfiles getEntityByNombre(object nombre)
        {
            return em.ct_perfiles.Where(q => (string)q.nombre == (string)nombre).FirstOrDefault<ct_perfiles>();
        }
        
    }
}
