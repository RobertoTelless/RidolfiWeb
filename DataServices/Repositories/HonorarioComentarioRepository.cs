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
    public class HonorarioComentarioRepository : RepositoryBase<HONORARIO_ANOTACOES>, IHonorarioComentarioRepository
    {
        public List<HONORARIO_ANOTACOES> GetAllItens()
        {
            return Db.HONORARIO_ANOTACOES.ToList();
        }

        public HONORARIO_ANOTACOES GetItemById(Int32 id)
        {
            IQueryable<HONORARIO_ANOTACOES> query = Db.HONORARIO_ANOTACOES.Where(p => p.HOAT_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 