using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceDenominaciones : AbstractService<ct_denominaciones>
    {
        public override void addEntity(ct_denominaciones entity)
        {
            em.ct_denominaciones.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_denominaciones it = em.ct_denominaciones.Where(q => (Int64)q.pkDenominacion == (Int64)pk).First<ct_denominaciones>();
            if (it == null)
            {
                throw new ArgumentException("Denominacion no Encontrada");
            }
            else
            {
                em.ct_denominaciones.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_denominaciones> getEntities()
        {
            return em.ct_denominaciones.OrderBy(c => c.posicion).ToList<ct_denominaciones>();
        }

        public override ct_denominaciones getEntity(object pk)
        {
            return em.ct_denominaciones.Where(q => (Int64)q.pkDenominacion == (Int64)pk).FirstOrDefault<ct_denominaciones>();
        }

        public override void updEntity(ct_denominaciones entity)
        {
            ct_denominaciones it = em.ct_denominaciones.Where(q => (Int64)q.pkDenominacion == (Int64)entity.pkDenominacion).First<ct_denominaciones>();
            if (it == null)
            {
                throw new ArgumentException("Denominacion no Encontrada");
            }
            else
            {
                it.nombre = entity.nombre;
                it.valor = entity.valor;
                it.path_imagen = entity.path_imagen;
                it.bin_imagen = entity.bin_imagen;
                it.posicion = entity.posicion;
                it.status = entity.status;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public ct_denominaciones getEntityByOrden(object posicion)
        {
            return em.ct_denominaciones.Where(q => (int)q.posicion == (int)posicion).FirstOrDefault<ct_denominaciones>();
        }

        public void delEntityByOrden(ct_denominaciones entity)
        {
            string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ct_denominaciones it = em.ct_denominaciones.Where(q => (int)q.posicion == (int)entity.posicion).First<ct_denominaciones>();
            if (it == null)
            {
                throw new ArgumentException("Denominacion no Encontrada");
            }
            else
            {
                //em.ct_tarifas_montos_fijos.Remove(it);
                it.deleted_at = fecha_actual;
                em.SaveChanges();
            }
        }
    }
}
