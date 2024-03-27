using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    class ServiceEmpresaCorredorOperador : AbstractService<sy_empresa_corredor_operador>
    {
        public override void addEntity(sy_empresa_corredor_operador entity)
        {
            em.sy_empresa_corredor_operador.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            sy_empresa_corredor_operador it = em.sy_empresa_corredor_operador.Where(q => (Int64)q.pkEmpresaCorredorOperador == (Int64)pk).First<sy_empresa_corredor_operador>();
            if (it == null)
            {
                throw new ArgumentException("EmpresaCorredorOperador no Encontrado");
            }
            else
            {
                em.sy_empresa_corredor_operador.Remove(it);
                em.SaveChanges();
            }
        }

        public override List<sy_empresa_corredor_operador> getEntities()
        {
            return em.sy_empresa_corredor_operador.ToList<sy_empresa_corredor_operador>();
        }

        public override sy_empresa_corredor_operador getEntity(object pk)
        {
            return em.sy_empresa_corredor_operador.Where(q => (Int64)q.pkEmpresaCorredorOperador == (Int64)pk).FirstOrDefault<sy_empresa_corredor_operador>();
        }

        public override void updEntity(sy_empresa_corredor_operador entity)
        {
            sy_empresa_corredor_operador it = em.sy_empresa_corredor_operador.Where(q => (Int64)q.pkEmpresaCorredorOperador == (Int64)entity.pkEmpresaCorredorOperador).First<sy_empresa_corredor_operador>();
            if (it == null)
            {
                throw new ArgumentException("EmpresaCorredorOperador no Encontrado");
            }
            else
            {
                it.fkEmpresa = entity.fkEmpresa;
                it.fkCorredor = entity.fkCorredor;
                it.fkOperador = entity.fkOperador;
                it.status = entity.status;
                it.created_at = entity.created_at;
                it.updated_at = entity.updated_at;
                it.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
    }
}
