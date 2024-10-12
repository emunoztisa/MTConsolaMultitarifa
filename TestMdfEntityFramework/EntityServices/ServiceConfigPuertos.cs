using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceConfigPuertos : AbstractService<ct_config_puertos>
    {
        public override void addEntity(ct_config_puertos entity)
        {
            em.ct_config_puertos.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_config_puertos it = em.ct_config_puertos.Where(q => (Int64)q.pkConfigPuerto == (Int64)pk).First<ct_config_puertos>();
            if (it == null)
            {
                throw new ArgumentException("Denominacion no Encontrada");
            }
            else
            {
                em.ct_config_puertos.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_config_puertos> getEntities()
        {
            return em.ct_config_puertos.OrderBy(c => c.port_name).ToList<ct_config_puertos>();
        }

        public List<ct_config_puertos> getEntitiesNotDeleted()
        {
            return em.ct_config_puertos.Where(q => q.deleted_at == null).OrderBy(c => c.port_name).ToList<ct_config_puertos>();
        }

        public override ct_config_puertos getEntity(object pk)
        {
            return em.ct_config_puertos.Where(q => q.pkConfigPuerto.ToString().Trim() == pk.ToString().Trim()).FirstOrDefault<ct_config_puertos>();
        }

        public override void updEntity(ct_config_puertos entity)
        {
            ct_config_puertos it = em.ct_config_puertos.Where(q => (Int64)q.pkConfigPuerto == (Int64)entity.pkConfigPuerto).First<ct_config_puertos>();
            if (it == null)
            {
                throw new ArgumentException("Denominacion no Encontrada");
            }
            else
            {
                it.nombre_dispositivo = entity.nombre_dispositivo;
                it.status = entity.status;

                it.port_name = entity.port_name;
                it.baud_rate = entity.baud_rate;
                it.data_bits = entity.data_bits;
                it.stop_bits = entity.stop_bits;
                it.paridad = entity.paridad;
                it.handshake = entity.handshake;
                
                it.enviado = entity.enviado;
                it.confirmado = entity.confirmado;
                it.modo = entity.modo;


                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
        public void delEntityByNombreDispositivo(ct_config_puertos entity)
        {
            string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ct_config_puertos it = em.ct_config_puertos.Where(q => q.deleted_at == null).Where(q => (string)q.nombre_dispositivo == (string)entity.nombre_dispositivo).First<ct_config_puertos>();
            if (it == null)
            {
                throw new ArgumentException("Config Puerto no Encontrada");
            }
            else
            {
                //em.ct_tarifas_montos_fijos.Remove(it);
                it.deleted_at = fecha_actual;
                it.status = 0;
                em.SaveChanges();
            }
        }

        public ct_config_puertos getEntityByNombreDispositivo(object nombre_dispositivo)
        {
            return em.ct_config_puertos.Where(q => q.nombre_dispositivo.ToString().Trim() == nombre_dispositivo.ToString().Trim()).Where(q => q.deleted_at == null).FirstOrDefault<ct_config_puertos>();
        }


    }
}
