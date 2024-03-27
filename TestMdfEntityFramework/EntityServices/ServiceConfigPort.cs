using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    public class ServiceConfigPort : AbstractService<config_port>
    {
        public override void addEntity(config_port entity)
        {
            em.config_port.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            config_port cp = em.config_port.Where(q => q.pkConfiPort == (int)pk).First<config_port>();
            if (cp == null)
            {
                throw new ArgumentException("Puerto no Encontrado");
            }
            else
            {
                em.config_port.Remove(cp);
                em.SaveChanges();
            }
        }

        public override List<config_port> getEntities()
        {
            return em.config_port.ToList<config_port>();
        }

        public override config_port getEntity(object pk)
        {
            return em.config_port.Where(q => q.pkConfiPort == (int)pk).First<config_port>();
        }

        public override void updEntity(config_port entity)
        {
            config_port cp = em.config_port.Where(q => q.port_name == (string)entity.port_name).First<config_port>();
            if (cp == null)
            {
                throw new ArgumentException("Puerto no Encontrado");
            }
            else
            {
                cp.port_name = entity.port_name;
                cp.baud_rate = entity.baud_rate;
                cp.data_bits = entity.data_bits;
                cp.stop_bits = entity.stop_bits;
                cp.parity = entity.parity;
                cp.created_at = entity.created_at;
                cp.updated_at = entity.updated_at;
                cp.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public config_port getEntityByStatus(int status)
        {
            return em.config_port.Where(q => q.status == (int)status).First<config_port>();
        }
    }
}
