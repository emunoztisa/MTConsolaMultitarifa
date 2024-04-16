using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMdfEntityFramework.Utils;

namespace TestMdfEntityFramework.EntityServices
{
    public class ServiceUsers : AbstractService<users>
    {
        public override void addEntity(users entity)
        {
            em.users.Add(entity);
            em.SaveChanges();
        }

        public override void delEntity(object pk)
        {
            users us = em.users.Where(q => q.pkUser == (int)pk).First<users>();
            if (us == null)
            {
                throw new ArgumentException("User no Encontrado");
            }
            else
            {
                em.users.Remove(us);
                em.SaveChanges();
            }
        }

        public override List<users> getEntities()
        {
            return em.users.ToList<users>();
        }

        public override users getEntity(object pk)
        {
            return em.users.Where(q => q.pkUser == (int)pk).First<users>();
        }

        public users getEntityByUser(string user)
        {
            return em.users.Where(q => q.user == user).FirstOrDefault<users>();
        }

        public override void updEntity(users entity)
        {
            users us = em.users.Where(q => q.pkUser == (int)entity.pkUser).First<users>();
            if (us == null)
            {
                throw new ArgumentException("User no Encontrado");
            }
            else
            {
                us.user = entity.user;
                us.contrasena = entity.contrasena;
                us.token = entity.token;
                us.created_at = entity.created_at;
                us.updated_at = entity.updated_at;
                us.deleted_at = entity.deleted_at;

                em.SaveChanges();
            }
        }
    }
}
