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
    public class AssinanteAnotacaoRepository : RepositoryBase<ASSINANTE_ANOTACAO>, IAssinanteAnotacaoRepository
    {
        public List<ASSINANTE_ANOTACAO> GetAllItens()
        {
            return Db.ASSINANTE_ANOTACAO.ToList();
        }

        public ASSINANTE_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE_ANOTACAO> query = Db.ASSINANTE_ANOTACAO.Where(p => p.ASAT_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 