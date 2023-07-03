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
    public class CRMPedidoAnexoRepository : RepositoryBase<CRM_PEDIDO_VENDA_ANEXO>, ICRMPedidoAnexoRepository
    {
        public List<CRM_PEDIDO_VENDA_ANEXO> GetAllItens()
        {
            return Db.CRM_PEDIDO_VENDA_ANEXO.ToList();
        }

        public CRM_PEDIDO_VENDA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<CRM_PEDIDO_VENDA_ANEXO> query = Db.CRM_PEDIDO_VENDA_ANEXO.Where(p => p.CRPA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 