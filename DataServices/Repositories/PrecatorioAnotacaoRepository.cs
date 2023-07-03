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
    public class PrecatorioAnotacaoRepository : RepositoryBase<PRECATORIO_ANOTACAO>, IPrecatorioAnotacaoRepository
    {
        public List<PRECATORIO_ANOTACAO> GetAllItens()
        {
            return Db.PRECATORIO_ANOTACAO.ToList();
        }

        public PRECATORIO_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<PRECATORIO_ANOTACAO> query = Db.PRECATORIO_ANOTACAO.Where(p => p.PRAT_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 