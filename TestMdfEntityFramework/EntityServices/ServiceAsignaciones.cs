using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceAsignaciones : AbstractService<sy_asignaciones>
    {
        public override void addEntity(sy_asignaciones entity)
        {
            em.sy_asignaciones.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_asignaciones it = em.sy_asignaciones.Where(q => (Int64)q.pkAsignacion == (Int64)pk).First<sy_asignaciones>();
            if (it == null)
            {
                throw new ArgumentException("Asignacion no Encontrada");
            }
            else
            {
                em.sy_asignaciones.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_asignaciones> getEntities()
        {
            return em.sy_asignaciones.ToList<sy_asignaciones>();
        }

        public override sy_asignaciones getEntity(object pk)
        {
            return em.sy_asignaciones.Where(q => (Int64)q.pkAsignacion == (Int64)pk).FirstOrDefault<sy_asignaciones>();
        }

        public override void updEntity(sy_asignaciones entity)
        {
            sy_asignaciones it = em.sy_asignaciones.Where(q => (Int64)q.pkAsignacion == (Int64)entity.pkAsignacion).First<sy_asignaciones>();
            if (it == null)
            {
                throw new ArgumentException("Asignacion no Encontrada");
            }
            else
            {
                it.fkRuta = entity.fkRuta;
                it.fkUnidad = entity.fkUnidad;
                it.fkOperador = entity.fkOperador;
                it.fkAndador = entity.fkAndador;
                it.fkLiquidacion = entity.fkLiquidacion;
                it.fkStatus = entity.fkStatus;
                it.folio = entity.folio;
                it.fecha = entity.fecha;
                it.hora = entity.hora;
                it.ihora = entity.ihora;
                it.cantidadAsientosDisp = entity.cantidadAsientosDisp;
                it.recurrente = entity.recurrente;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public sy_asignaciones getEntityByDayAndHour(string day, int hour)
        {
            List<sy_asignaciones> list_day = em.sy_asignaciones.Where(q => q.fecha == day).ToList<sy_asignaciones>();
            sy_asignaciones asignacion_activa = list_day.Where(q => q.ihora <= hour).FirstOrDefault<sy_asignaciones>();
            return asignacion_activa;
        }
        public List<sy_asignaciones> getEntityByDay(string day)
        {
            List<sy_asignaciones> list_asign_day = em.sy_asignaciones.Where(q => q.fecha == day).ToList<sy_asignaciones>();
            return list_asign_day;
        }

        public sy_asignaciones getEntityByFolio(object folio)
        {
            return em.sy_asignaciones.Where(q => (string)q.folio == (string)folio).FirstOrDefault<sy_asignaciones>();
        }
    }
}
