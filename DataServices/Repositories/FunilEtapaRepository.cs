using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;


namespace DataServices.Repositories
{
    public class FunilEtapaRepository : RepositoryBase<FUNIL_ETAPA>, IFunilEtapaRepository
    {
        public List<FUNIL_ETAPA> GetAllItens()
        {
            return Db.FUNIL_ETAPA.ToList();
        }

        public FUNIL_ETAPA GetItemById(Int32 id)
        {
            IQueryable<FUNIL_ETAPA> query = Db.FUNIL_ETAPA.Where(p => p.FUET_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<FUNIL_ETAPA> GetItensByFunil(int funilId)
        {
            return Db.FUNIL_ETAPA.Where(x => x.FUNI_CD_ID == funilId).ToList();            
        }
    }
}
 