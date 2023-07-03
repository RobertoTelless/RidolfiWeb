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
    public class VendaMensalComissaoRepository : RepositoryBase<VENDA_MENSAL_COMISSAO>, IVendaMensalComissaoRepository
    {
        public List<VENDA_MENSAL_COMISSAO> GetAllItens()
        {
            return Db.VENDA_MENSAL_COMISSAO.ToList();
        }

        public VENDA_MENSAL_COMISSAO GetItemById(Int32 id)
        {
            IQueryable<VENDA_MENSAL_COMISSAO> query = Db.VENDA_MENSAL_COMISSAO.Where(p => p.VECO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 