using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ConfiguracaoChavesRepository : RepositoryBase<CONFIGURACAO_CHAVES>, IConfiguracaoChavesRepository
    {
        public CONFIGURACAO_CHAVES GetItemById(Int32 id)
        {
            IQueryable<CONFIGURACAO_CHAVES> query = Db.CONFIGURACAO_CHAVES;
            query = query.Where(p => p.CFBA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONFIGURACAO_CHAVES> GetAllItems(Int32 idAss)
        {
            IQueryable<CONFIGURACAO_CHAVES> query = Db.CONFIGURACAO_CHAVES;
            query = query.Where(p => p.CFBA_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 