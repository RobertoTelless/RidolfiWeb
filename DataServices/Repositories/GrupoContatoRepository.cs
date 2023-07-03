using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class GrupoContatoRepository : RepositoryBase<GRUPO_CLIENTE>, IGrupoContatoRepository
    {
        public GRUPO_CLIENTE CheckExist(GRUPO_CLIENTE conta)
        {
            IQueryable<GRUPO_CLIENTE> query = Db.GRUPO_CLIENTE;
            query = query.Where(p => p.GRUP_CD_ID == conta.GRUP_CD_ID);
            query = query.Where(p => p.CLIE_CD_ID == conta.CLIE_CD_ID);
            return query.FirstOrDefault();
        }

        public List<GRUPO_CLIENTE> GetAllItens()
        {
            return Db.GRUPO_CLIENTE.ToList();
        }

        public GRUPO_CLIENTE GetItemById(Int32 id)
        {
            IQueryable<GRUPO_CLIENTE> query = Db.GRUPO_CLIENTE.Where(p => p.GRCL_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 