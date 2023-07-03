using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMItemPedidoRepository : IRepositoryBase<CRM_PEDIDO_VENDA_ITEM>
    {
        List<CRM_PEDIDO_VENDA_ITEM> GetAllItens();
        CRM_PEDIDO_VENDA_ITEM GetItemById(Int32 id);
        CRM_PEDIDO_VENDA_ITEM GetItemByProduto(Int32 id);
    
    }
}
