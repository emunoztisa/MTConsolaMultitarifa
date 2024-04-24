using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceImagenesSubidas : AbstractService<ct_imagenes_subidas>
    {
        public override void addEntity(ct_imagenes_subidas entity)
        {
            em.ct_imagenes_subidas.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_imagenes_subidas it = em.ct_imagenes_subidas.Where(q => (Int64)q.pkImagenSubida == (Int64)pk).First<ct_imagenes_subidas>();
            if (it == null)
            {
                throw new ArgumentException("Imagen no Encontrada");
            }
            else
            {
                em.ct_imagenes_subidas.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_imagenes_subidas> getEntities()
        {
            return em.ct_imagenes_subidas.OrderBy(c => c.pkImagenSubida).ToList<ct_imagenes_subidas>();
        }

        public override ct_imagenes_subidas getEntity(object pk)
        {
            return em.ct_imagenes_subidas.Where(q => (Int64)q.pkImagenSubida == (Int64)pk).FirstOrDefault<ct_imagenes_subidas>();
        }

        public override void updEntity(ct_imagenes_subidas entity)
        {
            ct_imagenes_subidas it = em.ct_imagenes_subidas.Where(q => (Int64)q.pkImagenSubida == (Int64)entity.pkImagenSubida).First<ct_imagenes_subidas>();
            if (it == null)
            {
                throw new ArgumentException("Imagen no Encontrada");
            }
            else
            {
                it.nombre = entity.nombre;
                it.imagen = entity.imagen;
                it.path_imagen = entity.path_imagen;
                it.status = entity.status;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public ct_imagenes_subidas getEntityByName(object name)
        {
            return em.ct_imagenes_subidas.Where(q => (string)q.nombre == (string)name).FirstOrDefault<ct_imagenes_subidas>();
        }
    }
}
