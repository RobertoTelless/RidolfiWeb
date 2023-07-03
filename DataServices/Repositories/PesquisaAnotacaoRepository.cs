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
    public class PesquisaAnotacaoRepository : RepositoryBase<PESQUISA_ANOTACAO>, IPesquisaAnotacaoRepository
    {
        public List<PESQUISA_ANOTACAO> GetAllItens()
        {
            return Db.PESQUISA_ANOTACAO.ToList();
        }

        public PESQUISA_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<PESQUISA_ANOTACAO> query = Db.PESQUISA_ANOTACAO.Where(p => p.PECO_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 