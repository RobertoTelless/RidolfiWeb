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
    public class ClienteAnotacaoRepository : RepositoryBase<CLIENTE_ANOTACAO>, IClienteAnotacaoRepository
    {
        public List<CLIENTE_ANOTACAO> GetAllItens()
        {
            return Db.CLIENTE_ANOTACAO.ToList();
        }

        public CLIENTE_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<CLIENTE_ANOTACAO> query = Db.CLIENTE_ANOTACAO.Where(p => p.CLAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 