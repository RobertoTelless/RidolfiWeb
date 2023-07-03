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
    public class PesquisaAnexoRepository : RepositoryBase<PESQUISA_ANEXO>, IPesquisaAnexoRepository
    {
        public List<PESQUISA_ANEXO> GetAllItens()
        {
            return Db.PESQUISA_ANEXO.ToList();
        }

        public PESQUISA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<PESQUISA_ANEXO> query = Db.PESQUISA_ANEXO.Where(p => p.PEAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 