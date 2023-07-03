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
    public class PesquisaItemRepository : RepositoryBase<PESQUISA_ITEM>, IPesquisaItemRepository
    {
        public List<PESQUISA_ITEM> GetAllItens()
        {
            return Db.PESQUISA_ITEM.ToList();
        }

        public PESQUISA_ITEM GetItemById(Int32 id)
        {
            IQueryable<PESQUISA_ITEM> query = Db.PESQUISA_ITEM.Where(p => p.PEIT_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 