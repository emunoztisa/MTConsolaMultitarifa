using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceBoletos : AbstractService<sy_boletos>
    {
        public override void addEntity(sy_boletos entity)
        {
            em.sy_boletos.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_boletos it = em.sy_boletos.Where(q => (Int64)q.pkBoleto == (Int64)pk).First<sy_boletos>();
            if (it == null)
            {
                throw new ArgumentException("Boleto no Encontrado");
            }
            else
            {
                em.sy_boletos.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_boletos> getEntities()
        {
            return em.sy_boletos.ToList<sy_boletos>();
        }

        public override sy_boletos getEntity(object pk)
        {
            return em.sy_boletos.Where(q => (Int64)q.pkBoleto == (Int64)pk).FirstOrDefault<sy_boletos>();
        }

        public override void updEntity(sy_boletos entity)
        {
            sy_boletos it = em.sy_boletos.Where(q => (Int64)q.pkBoleto == (Int64)entity.pkBoleto).First<sy_boletos>();
            if (it == null)
            {
                throw new ArgumentException("Boleto no Encontrado");
            }
            else
            {
                it.fkAsignacion = entity.fkAsignacion;
                it.fkLugarOrigen = entity.fkLugarOrigen;
                it.fkLugarDestino = entity.fkLugarDestino;
                it.fkStatus = entity.fkStatus;
                it.folio = entity.folio;
                it.total = entity.total;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
    }
}
