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
    public class PesquisaRespostaRepository : RepositoryBase<PESQUISA_RESPOSTA>, IPesquisaRespostaRepository
    {
        public List<PESQUISA_RESPOSTA> GetAllItens(Int32 idAss)
        {
            IQueryable<PESQUISA_RESPOSTA> query = Db.PESQUISA_RESPOSTA.Where(p => p.PERE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public PESQUISA_RESPOSTA GetItemById(Int32 id)
        {
            IQueryable<PESQUISA_RESPOSTA> query = Db.PESQUISA_RESPOSTA.Where(p => p.PERE_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 