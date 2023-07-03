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
    public class PesquisaItemOpcaoRepository : RepositoryBase<PESQUISA_ITEM_OPCAO>, IPesquisaItemOpcaoRepository
    {
        public List<PESQUISA_ITEM_OPCAO> GetAllItens()
        {
            return Db.PESQUISA_ITEM_OPCAO.ToList();
        }

        public PESQUISA_ITEM_OPCAO GetItemById(Int32 id)
        {
            IQueryable<PESQUISA_ITEM_OPCAO> query = Db.PESQUISA_ITEM_OPCAO.Where(p => p.PEIO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 