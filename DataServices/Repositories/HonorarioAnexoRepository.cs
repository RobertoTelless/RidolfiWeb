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
    public class HonorarioAnexoRepository : RepositoryBase<HONORARIO_ANEXO>, IHonorarioAnexoRepository
    {
        public List<HONORARIO_ANEXO> GetAllItens()
        {
            return Db.HONORARIO_ANEXO.ToList();
        }

        public HONORARIO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<HONORARIO_ANEXO> query = Db.HONORARIO_ANEXO.Where(p => p.HOAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 