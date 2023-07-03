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
    public class LogExcecaoRepository : RepositoryBase<LOG_EXCECAO_NOVO>, ILogExcecaoRepository
    {
        public LOG_EXCECAO_NOVO GetItemById(Int32 id)
        {
            IQueryable<LOG_EXCECAO_NOVO> query = Db.LOG_EXCECAO_NOVO;
            query = query.Where(p => p.LOEX_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<LOG_EXCECAO_NOVO> GetAllItens(Int32 idAss)
        {
            IQueryable<LOG_EXCECAO_NOVO> query = Db.LOG_EXCECAO_NOVO.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 