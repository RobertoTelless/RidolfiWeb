using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;


namespace DataServices.Repositories
{
    public class RecursividadeDestinoRepository : RepositoryBase<RECURSIVIDADE_DESTINO>, IRecursividadeDestinoRepository
    {
        public List<RECURSIVIDADE_DESTINO> GetAllItens()
        {
            return Db.RECURSIVIDADE_DESTINO.ToList();
        }

        public RECURSIVIDADE_DESTINO GetItemById(Int32 id)
        {
            IQueryable<RECURSIVIDADE_DESTINO> query = Db.RECURSIVIDADE_DESTINO.Where(p => p.REDE_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 