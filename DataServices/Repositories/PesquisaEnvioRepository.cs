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
    public class PesquisaEnvioRepository : RepositoryBase<PESQUISA_ENVIO>, IPesquisaEnvioRepository
    {
        public List<PESQUISA_ENVIO> GetAllItens()
        {
            return Db.PESQUISA_ENVIO.ToList();
        }

        public PESQUISA_ENVIO GetItemById(Int32 id)
        {
            IQueryable<PESQUISA_ENVIO> query = Db.PESQUISA_ENVIO.Where(p => p.PEEN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 