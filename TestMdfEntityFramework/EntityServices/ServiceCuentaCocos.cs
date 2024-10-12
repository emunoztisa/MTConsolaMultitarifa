using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceCuentaCocos : AbstractService<sy_conteo_cuenta_cocos>
    {
        public override void addEntity(sy_conteo_cuenta_cocos entity)
        {
            em.sy_conteo_cuenta_cocos.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_conteo_cuenta_cocos it = em.sy_conteo_cuenta_cocos.Where(q => (Int64)q.pkConteoCuentaCocos == (Int64)pk).First<sy_conteo_cuenta_cocos>();
            if (it == null)
            {
                throw new ArgumentException("Registro no Encontrado");
            }
            else
            {
                em.sy_conteo_cuenta_cocos.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_conteo_cuenta_cocos> getEntities()
        {
            return em.sy_conteo_cuenta_cocos.ToList<sy_conteo_cuenta_cocos>();
        }

        public override sy_conteo_cuenta_cocos getEntity(object pk)
        {
            return em.sy_conteo_cuenta_cocos.Where(q => (Int64)q.pkConteoCuentaCocos == (Int64)pk).FirstOrDefault<sy_conteo_cuenta_cocos>();
        }

        public override void updEntity(sy_conteo_cuenta_cocos entity)
        {
            sy_conteo_cuenta_cocos it = em.sy_conteo_cuenta_cocos.Where(q => (Int64)q.pkConteoCuentaCocos == (Int64)entity.pkConteoCuentaCocos).First<sy_conteo_cuenta_cocos>();
            if (it == null)
            {
                throw new ArgumentException("Registro no Encontrado");
            }
            else
            {
                it.pkConteoCuentaCocosTISA = entity.pkConteoCuentaCocosTISA;
                it.fkAsignacion = entity.fkAsignacion;
                it.fkStatus = entity.fkStatus;
                it.cc1_subidas = entity.cc1_subidas;
                it.cc1_bajadas = entity.cc1_bajadas;
                it.cc2_subidas = entity.cc2_subidas;
                it.cc2_bajadas = entity.cc2_bajadas;
                it.cc3_subidas = entity.cc3_subidas;
                it.cc3_bajadas = entity.cc3_bajadas;
                it.fecha_hora = entity.fecha_hora;
                it.enviado = entity.enviado;
                it.confirmado = entity.confirmado;
                it.modo = entity.modo;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
        public List<sy_conteo_cuenta_cocos> getEntitiesByEnviados()
        {
            return em.sy_conteo_cuenta_cocos.Where(q => q.enviado == 0 || q.enviado == null).ToList<sy_conteo_cuenta_cocos>();
        }
    }
}
