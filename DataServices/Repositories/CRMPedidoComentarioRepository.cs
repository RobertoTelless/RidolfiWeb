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
    public class CRMPedidoComentarioRepository : RepositoryBase<CRM_PEDIDO_VENDA_ACOMPANHAMENTO>, ICRMPedidoComentarioRepository
    {
        public List<CRM_PEDIDO_VENDA_ACOMPANHAMENTO> GetAllItens()
        {
            return Db.CRM_PEDIDO_VENDA_ACOMPANHAMENTO.ToList();
        }

        public CRM_PEDIDO_VENDA_ACOMPANHAMENTO GetItemById(Int32 id)
        {
            IQueryable<CRM_PEDIDO_VENDA_ACOMPANHAMENTO> query = Db.CRM_PEDIDO_VENDA_ACOMPANHAMENTO.Where(p => p.CRPC_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 