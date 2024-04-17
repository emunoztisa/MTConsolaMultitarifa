using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceUbicacion : AbstractService<sy_ubicacion>
    {
        public override void addEntity(sy_ubicacion entity)
        {
            em.sy_ubicacion.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_ubicacion it = em.sy_ubicacion.Where(q => (Int64)q.pkUbicacion == (Int64)pk).First<sy_ubicacion>();
            if (it == null)
            {
                throw new ArgumentException("Ubicacion no Encontrada");
            }
            else
            {
                em.sy_ubicacion.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_ubicacion> getEntities()
        {
            return em.sy_ubicacion.ToList<sy_ubicacion>();
        }

        public override sy_ubicacion getEntity(object pk)
        {
            return em.sy_ubicacion.Where(q => (Int64)q.pkUbicacion == (Int64)pk).FirstOrDefault<sy_ubicacion>();
        }

        public override void updEntity(sy_ubicacion entity)
        {
            sy_ubicacion it = em.sy_ubicacion.Where(q => (Int64)q.pkUbicacion == (Int64)entity.pkUbicacion).First<sy_ubicacion>();
            if (it == null)
            {
                throw new ArgumentException("Ubicacion no Encontrada");
            }
            else
            {
                it.fkAsignacion = entity.fkAsignacion;
                it.latitud = entity.latitud;
                it.longitud = entity.longitud;
                it.enviado = entity.enviado;
                it.confirmadoTISA = entity.confirmadoTISA;
                it.modo = entity.modo;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
    }
}
