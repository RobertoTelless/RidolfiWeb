using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;


namespace DataServices.Repositories
{
    public class RecursividadeDataRepository : RepositoryBase<RECURSIVIDADE_DATA>, IRecursividadeDataRepository
    {
        public List<RECURSIVIDADE_DATA> GetAllItens()
        {
            return Db.RECURSIVIDADE_DATA.ToList();
        }

        public RECURSIVIDADE_DATA GetItemById(Int32 id)
        {
            IQueryable<RECURSIVIDADE_DATA> query = Db.RECURSIVIDADE_DATA.Where(p => p.REDA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<RECURSIVIDADE_DATA> GetItensByData(DateTime data)
        {
            return Db.RECURSIVIDADE_DATA.Where(x => x.REDA_DT_PROGRAMADA.Date == data.Date).ToList();
        }
    }
}
 