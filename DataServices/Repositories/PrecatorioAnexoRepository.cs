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
    public class PrecatorioAnexoRepository : RepositoryBase<PRECATORIO_ANEXO>, IPrecatorioAnexoRepository
    {
        public List<PRECATORIO_ANEXO> GetAllItens()
        {
            return Db.PRECATORIO_ANEXO.ToList();
        }

        public PRECATORIO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<PRECATORIO_ANEXO> query = Db.PRECATORIO_ANEXO.Where(p => p.PRAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 