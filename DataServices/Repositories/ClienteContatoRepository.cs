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
    public class ClienteContatoRepository : RepositoryBase<CLIENTE_CONTATO>, IClienteContatoRepository
    {
        public List<CLIENTE_CONTATO> GetAllItens()
        {
            return Db.CLIENTE_CONTATO.ToList();
        }

        public CLIENTE_CONTATO GetItemById(Int32 id)
        {
            IQueryable<CLIENTE_CONTATO> query = Db.CLIENTE_CONTATO.Where(p => p.CLCO_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 