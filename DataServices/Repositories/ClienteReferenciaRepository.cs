using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ClienteReferenciaRepository : RepositoryBase<CLIENTE_REFERENCIA>, IClienteReferenciaRepository
    {
        public List<CLIENTE_REFERENCIA> GetAllItens()
        {
            return Db.CLIENTE_REFERENCIA.ToList();
        }

        public CLIENTE_REFERENCIA GetItemById(Int32 id)
        {
            IQueryable<CLIENTE_REFERENCIA> query = Db.CLIENTE_REFERENCIA.Where(p => p.CLRE_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
