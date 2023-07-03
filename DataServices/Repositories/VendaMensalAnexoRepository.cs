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
    public class VendaMensalAnexoRepository : RepositoryBase<VENDA_MENSAL_ANEXO>, IVendaMensalAnexoRepository
    {
        public List<VENDA_MENSAL_ANEXO> GetAllItens()
        {
            return Db.VENDA_MENSAL_ANEXO.ToList();
        }

        public VENDA_MENSAL_ANEXO GetItemById(Int32 id)
        {
            IQueryable<VENDA_MENSAL_ANEXO> query = Db.VENDA_MENSAL_ANEXO.Where(p => p.VMAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 