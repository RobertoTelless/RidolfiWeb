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
    public class BeneficiarioComentarioRepository : RepositoryBase<BENEFICIARIO_ANOTACOES>, IBeneficiarioComentarioRepository
    {
        public List<BENEFICIARIO_ANOTACOES> GetAllItens()
        {
            return Db.BENEFICIARIO_ANOTACOES.ToList();
        }

        public BENEFICIARIO_ANOTACOES GetItemById(Int32 id)
        {
            IQueryable<BENEFICIARIO_ANOTACOES> query = Db.BENEFICIARIO_ANOTACOES.Where(p => p.BECO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 