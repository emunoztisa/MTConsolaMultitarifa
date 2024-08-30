
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    public class ServiceComun_Usuarios : AbstractService<ct_usuarios>
    {
        public override void addEntity(ct_usuarios entity)
        {
            em.ct_usuarios.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            ct_usuarios obj = em.ct_usuarios.Where(q => q.pkUsuario == (long)pk).First<ct_usuarios>();
            if (obj == null)
            {
                throw new ArgumentException("No Encontrado");
            }
            else
            {
                em.ct_usuarios.Remove(obj);
                em.SaveChanges();
            }
        }

        public override List<ct_usuarios> getEntities()
        {
            return em.ct_usuarios.ToList<ct_usuarios>();
        }

        public override ct_usuarios getEntity(object pk)
        {
            return em.ct_usuarios.Where(q => q.pkUsuario == (long)pk).First<ct_usuarios>();
        }

        public ct_usuarios getEntityByUser(string usuario)
        {
            return em.ct_usuarios.Where(q => q.usuario == usuario).FirstOrDefault<ct_usuarios>();
        }

        public override void updEntity(ct_usuarios entity)
        {
            ct_usuarios obj = em.ct_usuarios.Where(q => q.pkUsuario == (long)entity.pkUsuario).First<ct_usuarios>();
            if (obj == null)
            {
                throw new ArgumentException("No Encontrado");
            }
            else
            {
                obj.fkPuesto = entity.fkPuesto;
                obj.fkStatus = entity.fkStatus;
                obj.nombre = entity.nombre;
                obj.usuario = entity.usuario;
                obj.contrasena = entity.contrasena;
                obj.token = entity.token;
                obj.tipo_usuario = entity.tipo_usuario;
                obj.enviado = entity.enviado;
                obj.confirmado = entity.confirmado;
                obj.modo = entity.modo;
                obj.created_at = entity.created_at;
                obj.updated_at = entity.updated_at;
                obj.deleted_at = entity.deleted_at;
                obj.created_id = entity.created_id;
                obj.updated_id = entity.updated_id;
                obj.deleted_id = entity.deleted_id;

                em.SaveChanges();
            }
        }

        
    }
}
