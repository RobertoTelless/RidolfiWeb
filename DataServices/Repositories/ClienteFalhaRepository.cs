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
    public class ClienteFalhaRepository : RepositoryBase<CLIENTE_FALHA>, IClienteFalhaRepository
    {
        public List<CLIENTE_FALHA> GetAllItens()
        {
            return Db.CLIENTE_FALHA.ToList();
        }

        public CLIENTE_FALHA GetItemById(Int32 id)
        {
            IQueryable<CLIENTE_FALHA> query = Db.CLIENTE_FALHA.Where(p => p.CLFA_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 