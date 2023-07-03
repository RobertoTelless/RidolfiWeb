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
    public class PesquisaRespostaItemRepository : RepositoryBase<PESQUISA_RESPOSTA_ITEM>, IPesquisaRespostaItemRepository
    {
        public List<PESQUISA_RESPOSTA_ITEM> GetAllItens()
        {
            return Db.PESQUISA_RESPOSTA_ITEM.ToList();
        }

        public PESQUISA_RESPOSTA_ITEM GetItemById(Int32 id)
        {
            IQueryable<PESQUISA_RESPOSTA_ITEM> query = Db.PESQUISA_RESPOSTA_ITEM.Where(p => p.PERT_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 