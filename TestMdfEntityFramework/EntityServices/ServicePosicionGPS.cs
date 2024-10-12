using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServicePosicionGPS : AbstractService<sy_posicion_gps>
    {
        public override void addEntity(sy_posicion_gps entity)
        {
            em.sy_posicion_gps.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_posicion_gps it = em.sy_posicion_gps.Where(q => (long)q.pkPosicionGPS == (long)pk).First<sy_posicion_gps>();
            if (it == null)
            {
                throw new ArgumentException("Registro no Encontrado");
            }
            else
            {
                em.sy_posicion_gps.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_posicion_gps> getEntities()
        {
            return em.sy_posicion_gps.ToList<sy_posicion_gps>();
        }

        public override sy_posicion_gps getEntity(object pk)
        {
            return em.sy_posicion_gps.Where(q => (long)q.pkPosicionGPS == (long)pk).FirstOrDefault<sy_posicion_gps>();
        }

        public override void updEntity(sy_posicion_gps entity)
        {
            sy_posicion_gps it = em.sy_posicion_gps.Where(q => (long)q.pkPosicionGPS == (long)entity.pkPosicionGPS).First<sy_posicion_gps>();
            if (it == null)
            {
                throw new ArgumentException("Registro no Encontrado");
            }
            else
            {
                it.fkAsignacion = entity.fkAsignacion;
                it.fkStatus = entity.fkStatus;
                it.lat = entity.lat;
                it.lng = entity.lng;
                it.fecha_hora = entity.fecha_hora;
                it.enviado = entity.enviado;
                it.confirmado = entity.confirmado;
                it.modo = entity.modo;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public List<sy_posicion_gps> getEntitiesByEnviados()
        {
            return em.sy_posicion_gps.Where(q => q.enviado == 0 || q.enviado == null).ToList<sy_posicion_gps>();
        }
    }
}
