using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Clases
{
    public class Perfiles
    {
        public int pkUserPerfil { get; set; }
        public int fkUser { get; set; }
        public int fkRole { get; set; }


        public Perfiles()
        {

        }

        public Perfiles(int pkUserPerfil, int fkUser, int fkRole)
        {
            this.pkUserPerfil = pkUserPerfil;
            this.fkUser = fkUser;
            this.fkRole = fkRole;
        }

        public int GetPkUserPerfil()
        {
            return pkUserPerfil;
        }
        public void SetPkUserPerfil(int val)
        {
            this.pkUserPerfil = val;
        }
        public int GetFkUser()
        {
            return fkUser;
        }
        public void SetFkUser(int val)
        {
            this.fkUser = val;
        }
        public int GetFkRole()
        {
            return fkRole;
        }
        public void SetFkRole(int val)
        {
            this.fkRole = val;
        }

    }
}
