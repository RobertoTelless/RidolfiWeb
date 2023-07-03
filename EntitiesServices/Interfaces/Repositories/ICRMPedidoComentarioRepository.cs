using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMPedidoComentarioRepository : IRepositoryBase<CRM_PEDIDO_VENDA_ACOMPANHAMENTO>
    {
        List<CRM_PEDIDO_VENDA_ACOMPANHAMENTO> GetAllItens();
        CRM_PEDIDO_VENDA_ACOMPANHAMENTO GetItemById(Int32 id);
    }
}
