using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceBoletosTarifaFija : AbstractService<sy_boletos_tarifa_fija>
    {
        public override void addEntity(sy_boletos_tarifa_fija entity)
        {
            em.sy_boletos_tarifa_fija.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_boletos_tarifa_fija us = em.sy_boletos_tarifa_fija.Where(q => q.pkBoleto == (int)pk).First<sy_boletos_tarifa_fija>();
            if (us == null)
            {
                throw new ArgumentException("Boleto no Encontrado");
            }
            else
            {
                em.sy_boletos_tarifa_fija.Remove(us);
                em.SaveChanges();
            }
        }

        public override List<sy_boletos_tarifa_fija> getEntities()
        {
            return em.sy_boletos_tarifa_fija.ToList<sy_boletos_tarifa_fija>();
        }

        public override sy_boletos_tarifa_fija getEntity(object pk)
        {
            return em.sy_boletos_tarifa_fija.Where(q => (Int64)q.pkBoleto == (Int64)pk).First<sy_boletos_tarifa_fija>();
        }

        public override void updEntity(sy_boletos_tarifa_fija entity)
        {
            sy_boletos_tarifa_fija us = em.sy_boletos_tarifa_fija.Where(q => q.pkBoleto == (int)entity.pkBoleto).First<sy_boletos_tarifa_fija>();
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

        public Int64 getLastEntity()
        {
            return em.sy_boletos_tarifa_fija.Last().pkBoleto;
        }

        public Int64 addEntityReturnPkInserted(sy_boletos_tarifa_fija entity)
        {
            em.sy_boletos_tarifa_fija.Add(entity);
            em.SaveChanges();

            return entity.pkBoleto;
        }

        public sy_boletos_tarifa_fija getEntityLast()
        {
            return em.sy_boletos_tarifa_fija.OrderByDescending(q => q.pkBoleto).FirstOrDefault<sy_boletos_tarifa_fija>();
        }

        public List<sy_boletos_tarifa_fija> getEntitiesByEnviados()
        {
            return em.sy_boletos_tarifa_fija.Where(q => q.enviado == 0).ToList<sy_boletos_tarifa_fija>();
        }
        public List<sy_boletos_tarifa_fija> getEntitiesByConfirmadosTISA()
        {
            return em.sy_boletos_tarifa_fija.Where(q => q.confirmadoTISA == 0).ToList<sy_boletos_tarifa_fija>();
        }
    }
}
