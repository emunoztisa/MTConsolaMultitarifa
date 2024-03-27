using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    public class ServiceTipoTarifa : AbstractService<tipo_tarifa>
    {
        public override void addEntity(tipo_tarifa entity)
        {
            em.tipo_tarifa.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            tipo_tarifa us = em.tipo_tarifa.Where(q => q.pkTipoTarifa == (int)pk).First<tipo_tarifa>();
            if (us == null)
            {
                throw new ArgumentException("Tipo Tarifa no Encontrado");
            }
            else
            {
                em.tipo_tarifa.Remove(us);
                em.SaveChanges();
            }
        }

        public override List<tipo_tarifa> getEntities()
        {
            return em.tipo_tarifa.ToList<tipo_tarifa>();
        }

        public override tipo_tarifa getEntity(object pk)
        {
            return em.tipo_tarifa.Where(q => q.pkTipoTarifa == (int)pk).First<tipo_tarifa>();
        }

        public override void updEntity(tipo_tarifa entity)
        {
            tipo_tarifa us = em.tipo_tarifa.Where(q => q.pkTipoTarifa == (int)entity.pkTipoTarifa).First<tipo_tarifa>();
            if (us == null)
            {
                throw new ArgumentException("Tipo Tarifa no Encontrado");
            }
            else
            {
                us.tipo_tarifa1 = entity.tipo_tarifa1;
                us.status = entity.status;
                us.created_at = entity.created_at;
                us.updated_at = entity.updated_at;
                us.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
        public tipo_tarifa getEntityByTipoTarifa(string tipo_tarifa)
        {
            return em.tipo_tarifa.Where(q => q.tipo_tarifa1 == (string)tipo_tarifa).First<tipo_tarifa>();
        }
    }
}
