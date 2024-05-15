using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceOpcionesGenerales : AbstractService<opciones_generales>
    {
        public override void addEntity(opciones_generales entity)
        {
            em.opciones_generales.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            opciones_generales us = em.opciones_generales.Where(q => q.pkOpcionGeneral == (int)pk).First<opciones_generales>();
            if (us == null)
            {
                throw new ArgumentException("Opcion General no Encontrado");
            }
            else
            {
                em.opciones_generales.Remove(us);
                em.SaveChanges();
            }
        }

        public override List<opciones_generales> getEntities()
        {
            return em.opciones_generales.ToList<opciones_generales>();
        }

        public override opciones_generales getEntity(object pk)
        {
            return em.opciones_generales.Where(q => q.pkOpcionGeneral == (int)pk).First<opciones_generales>();
        }

        public override void updEntity(opciones_generales entity)
        {
            opciones_generales us = em.opciones_generales.Where(q => q.pkOpcionGeneral == (int)entity.pkOpcionGeneral).First<opciones_generales>();
            if (us == null)
            {
                throw new ArgumentException("Opcion General no Encontrado");
            }
            else
            {
                us.opcion_general = entity.opcion_general;
                us.valor = entity.valor;
                us.orden = entity.orden;
                us.agrupador = entity.agrupador;
                us.created_at = entity.created_at;
                us.updated_at = entity.updated_at;
                us.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public List<opciones_generales> getEntitiesByAgrupador(string agrupador)
        {
            return em.opciones_generales.Where(q => q.agrupador == (string)agrupador).ToList<opciones_generales>();
        }

        public opciones_generales getEntityByOpcionGeneral(string opcion_general)
        {
            return em.opciones_generales.Where(q => q.opcion_general == opcion_general).First<opciones_generales>();
        }

        public void updEntityByOpcionGeneral(opciones_generales entity)
        {
            opciones_generales us = em.opciones_generales.Where(q => q.opcion_general == (string)entity.opcion_general).First<opciones_generales>();
            if (us == null)
            {
                throw new ArgumentException("Opcion General no Encontrado");
            }
            else
            {
                us.valor = entity.valor;
                us.orden = entity.orden;
                us.agrupador = entity.agrupador;
                us.created_at = entity.created_at;
                us.updated_at = entity.updated_at;
                us.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
    }
}
