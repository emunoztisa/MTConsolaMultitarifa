using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceUnidades : AbstractService<ct_unidades>
    {
        public override void addEntity(ct_unidades entity)
        {
            em.ct_unidades.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_unidades it = em.ct_unidades.Where(q => (Int64)q.pkUnidad == (Int64)pk).First<ct_unidades>();
            if (it == null)
            {
                throw new ArgumentException("Unidad no Encontrado");
            }
            else
            {
                em.ct_unidades.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_unidades> getEntities()
        {
            return em.ct_unidades.ToList<ct_unidades>();
        }

        public override ct_unidades getEntity(object pk)
        {
            return em.ct_unidades.Where(q => (Int64)q.pkUnidad == (Int64)pk).FirstOrDefault<ct_unidades>();
        }

        public override void updEntity(ct_unidades entity)
        {
            ct_unidades it = em.ct_unidades.Where(q => (Int64)q.pkUnidad == (Int64)entity.pkUnidad).First<ct_unidades>();
            if (it == null)
            {
                throw new ArgumentException("Unidad no Encontrado");
            }
            else
            {
                it.fkEmpresa = entity.fkEmpresa;
                it.fkCorredor = entity.fkCorredor;
                it.numeracion = entity.numeracion;
                it.nombre = entity.nombre;
                it.noSerieAVL = entity.noSerieAVL;
                it.capacidad = entity.capacidad;
                it.validador = entity.validador;
                it.status = entity.status;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public ct_unidades getEntityByName(object nombre_unidad)
        {
            return em.ct_unidades.Where(q => q.nombre == (string)nombre_unidad).FirstOrDefault<ct_unidades>();
        }

        public List<ct_unidades> getEntityPorFkCorredor(object fkCorredor)
        {
            return em.ct_unidades.Where(q => q.fkCorredor == (Int64)fkCorredor).ToList<ct_unidades>();
        }
    }
}
