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
    public class VendaMensalAnotacaoRepository : RepositoryBase<VENDA_MENSAL_ANOTACAO>, IVendaMensalAnotacaoRepository
    {
        public List<VENDA_MENSAL_ANOTACAO> GetAllItens()
        {
            return Db.VENDA_MENSAL_ANOTACAO.ToList();
        }

        public VENDA_MENSAL_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<VENDA_MENSAL_ANOTACAO> query = Db.VENDA_MENSAL_ANOTACAO.Where(p => p.VMAT_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 