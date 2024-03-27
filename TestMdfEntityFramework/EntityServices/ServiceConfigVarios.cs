using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceConfigVarios : AbstractService<config_varios>
    {
        public override void addEntity(config_varios entity)
        {
            em.config_varios.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            config_varios us = em.config_varios.Where(q => q.pkConfigVarios == (int)pk).First<config_varios>();
            if (us == null)
            {
                throw new ArgumentException("Config Varios no Encontrado");
            }
            else
            {
                em.config_varios.Remove(us);
                em.SaveChanges();
            }
        }

        public override List<config_varios> getEntities()
        {
            return em.config_varios.ToList<config_varios>();
        }

        public override config_varios getEntity(object pk)
        {
            return em.config_varios.Where(q => q.pkConfigVarios == (int)pk).First<config_varios>();
        }

        public override void updEntity(config_varios entity)
        {
            config_varios us = em.config_varios.Where(q => q.pkConfigVarios == (int)entity.pkConfigVarios).First<config_varios>();
            if (us == null)
            {
                throw new ArgumentException("Config Varios no Encontrado");
            }
            else
            {
                us.clave = entity.clave;
                us.valor = entity.valor;
                us.descripcion = entity.descripcion;
                us.created_at = entity.created_at;
                us.updated_at = entity.updated_at;
                us.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public config_varios getEntityByClave(string clave)
        {
            return em.config_varios.Where(q => q.clave == (string)clave).First<config_varios>();
        }

        public void updEntityByClave(config_varios entity)
        {
            config_varios us = em.config_varios.Where(q => q.clave == (string)entity.clave).First<config_varios>();
            if (us == null)
            {
                throw new ArgumentException("Config Varios no Encontrado");
            }
            else
            {
                us.valor = entity.valor;
                us.descripcion = entity.descripcion;
                us.created_at = entity.created_at;
                us.updated_at = entity.updated_at;
                us.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
    }
}
