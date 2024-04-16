using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceMensajes : AbstractService<sy_mensajes>
    {
        public override void addEntity(sy_mensajes entity)
        {
            em.sy_mensajes.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_mensajes it = em.sy_mensajes.Where(q => (Int64)q.pkMensaje == (Int64)pk).First<sy_mensajes>();
            if (it == null)
            {
                throw new ArgumentException("Mensaje no Encontrado");
            }
            else
            {
                em.sy_mensajes.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_mensajes> getEntities()
        {
            return em.sy_mensajes.ToList<sy_mensajes>();
        }

        public override sy_mensajes getEntity(object pk)
        {
            return em.sy_mensajes.Where(q => (Int64)q.pkMensaje == (Int64)pk).FirstOrDefault<sy_mensajes>();
        }

        public override void updEntity(sy_mensajes entity)
        {
            sy_mensajes it = em.sy_mensajes.Where(q => (Int64)q.pkMensaje == (Int64)entity.pkMensaje).First<sy_mensajes>();
            if (it == null)
            {
                throw new ArgumentException("Mensaje no Encontrado");
            }
            else
            {
                it.pkMensaje = entity.pkMensaje;
                it.fkAsignacion = entity.fkAsignacion;
                it.fkStatus = entity.fkStatus;
                it.mensaje = entity.mensaje;
                it.enviado = entity.enviado;
                it.confirmadoTISA = entity.confirmadoTISA;
                it.modo = entity.modo;
                it.dispositivo_origen = entity.dispositivo_origen;
                it.dispositivo_destino = entity.dispositivo_destino;
                it.reproducido = entity.reproducido;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public List<sy_mensajes> getEntityNoReproducidos()
        {
            return em.sy_mensajes.Where(q => (q.reproducido == null || q.reproducido == 0) && q.dispositivo_destino == 1).ToList<sy_mensajes>();
        }
    }
}
