using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class CRMItemPedidoRepository : RepositoryBase<CRM_PEDIDO_VENDA_ITEM>, ICRMItemPedidoRepository
    {
        public List<CRM_PEDIDO_VENDA_ITEM> GetAllItens()
        {
            return Db.CRM_PEDIDO_VENDA_ITEM.ToList();
        }

        public CRM_PEDIDO_VENDA_ITEM GetItemById(Int32 id)
        {
            IQueryable<CRM_PEDIDO_VENDA_ITEM> query = Db.CRM_PEDIDO_VENDA_ITEM.Where(p => p.CRPI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public CRM_PEDIDO_VENDA_ITEM GetItemByProduto(Int32 id)
        {
            IQueryable<CRM_PEDIDO_VENDA_ITEM> query = Db.CRM_PEDIDO_VENDA_ITEM.Where(p => p.PROD_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
