using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceTarifasMontosFijos : AbstractService<ct_tarifas_montos_fijos>
    {
        public override void addEntity(ct_tarifas_montos_fijos entity)
        {
            em.ct_tarifas_montos_fijos.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_tarifas_montos_fijos it = em.ct_tarifas_montos_fijos.Where(q => (Int64)q.pkTarifaMontoFijo == (Int64)pk).First<ct_tarifas_montos_fijos>();
            if (it == null)
            {
                throw new ArgumentException("Tarifa monto fijo no Encontrado");
            }
            else
            {
                em.ct_tarifas_montos_fijos.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<ct_tarifas_montos_fijos> getEntities()
        {
            return em.ct_tarifas_montos_fijos.Where(q => q.deleted_at == null).OrderBy(q => q.orden).ToList<ct_tarifas_montos_fijos>();
        }

        public override ct_tarifas_montos_fijos getEntity(object pk)
        {
            return em.ct_tarifas_montos_fijos.Where(q => (Int64)q.pkTarifaMontoFijo == (Int64)pk).FirstOrDefault<ct_tarifas_montos_fijos>();
        }

        public override void updEntity(ct_tarifas_montos_fijos entity)
        {
            ct_tarifas_montos_fijos it = em.ct_tarifas_montos_fijos.Where(q => (Int64)q.pkTarifaMontoFijo == (Int64)entity.pkTarifaMontoFijo).First<ct_tarifas_montos_fijos>();
            if (it == null)
            {
                throw new ArgumentException("Tarifa monto fijo no Encontrado");
            }
            else
            {
                it.valor = entity.valor;
                it.texto = entity.texto;
                it.descripcion = entity.descripcion;
                it.orden = entity.orden;
                it.status = entity.status;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }

        public ct_tarifas_montos_fijos getEntityByOrden(object orden)
        {
            return em.ct_tarifas_montos_fijos.Where(q => (int)q.orden == (int)orden).FirstOrDefault<ct_tarifas_montos_fijos>();
        }

        public void delEntityByOrden(ct_tarifas_montos_fijos entity)
        {
            string fecha_actual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ct_tarifas_montos_fijos it = em.ct_tarifas_montos_fijos.Where(q => (int)q.orden == (int)entity.orden).First<ct_tarifas_montos_fijos>();
            if (it == null)
            {
                throw new ArgumentException("Tarifa monto fijo no Encontrado");
            }
            else
            {
                //em.ct_tarifas_montos_fijos.Remove(it);
                it.deleted_at = fecha_actual;
                em.SaveChanges();
            }
        }

        public ct_tarifas_montos_fijos getEntityByValor(object valor)
        {
            return em.ct_tarifas_montos_fijos.Where(q => q.valor.ToString().Trim() == valor.ToString().Trim()).FirstOrDefault<ct_tarifas_montos_fijos>();
        }
    }
}
