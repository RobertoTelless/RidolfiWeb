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
    public class AssinantePagamentoRepository : RepositoryBase<ASSINANTE_PAGAMENTO>, IAssinantePagamentoRepository
    {
        public List<ASSINANTE_PAGAMENTO> GetAllItens()
        {
            IQueryable<ASSINANTE_PAGAMENTO> query = Db.ASSINANTE_PAGAMENTO.Where(p => p.ASPA_IN_ATIVO == 1);
            return query.ToList();
        }

        public ASSINANTE_PAGAMENTO GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE_PAGAMENTO> query = Db.ASSINANTE_PAGAMENTO.Where(p => p.ASPA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 