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
using System.Web.Configuration;

namespace DataServices.Repositories
{
    public class NacionalidadeRepository : RepositoryBase<NACIONALIDADE>, INacionalidadeRepository
    {
        public NACIONALIDADE GetItemById(Int32 id)
        {
            IQueryable<NACIONALIDADE> query = Db.NACIONALIDADE;
            query = query.Where(p => p.NACI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<NACIONALIDADE> GetAllItens()
        {
            IQueryable<NACIONALIDADE> query = Db.NACIONALIDADE.Where(p => p.NACI_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 