using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceBoletosTarifaFija : AbstractService<boletos_tarifa_fija>
    {
        public override void addEntity(boletos_tarifa_fija entity)
        {
            em.boletos_tarifa_fija.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            boletos_tarifa_fija us = em.boletos_tarifa_fija.Where(q => q.pkBoleto == (int)pk).First<boletos_tarifa_fija>();
            if (us == null)
            {
                throw new ArgumentException("Boleto no Encontrado");
            }
            else
            {
                em.boletos_tarifa_fija.Remove(us);
                em.SaveChanges();
            }
        }

        public override List<boletos_tarifa_fija> getEntities()
        {
            return em.boletos_tarifa_fija.ToList<boletos_tarifa_fija>();
        }

        public override boletos_tarifa_fija getEntity(object pk)
        {
            return em.boletos_tarifa_fija.Where(q => q.pkBoleto == (int)pk).First<boletos_tarifa_fija>();
        }

        public override void updEntity(boletos_tarifa_fija entity)
        {
            boletos_tarifa_fija us = em.boletos_tarifa_fija.Where(q => q.pkBoleto == (int)entity.pkBoleto).First<boletos_tarifa_fija>();
            if (us == null)
            {
                throw new ArgumentException("Boleto no Encontrado");
            }
            else
            {
                us.folio = entity.folio;
                us.tarifa = entity.tarifa;
                us.cant_pasajeros = entity.cant_pasajeros;
                us.total = entity.total;
                us.created_at = entity.created_at;
                us.updated_at = entity.updated_at;
                us.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public UInt32 getLastEntity()
        {
            return em.boletos_tarifa_fija.Last<boletos_tarifa_fija>().pkBoleto;
        }
    }
}
