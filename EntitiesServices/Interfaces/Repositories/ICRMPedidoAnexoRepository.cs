using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMPedidoAnexoRepository : IRepositoryBase<CRM_PEDIDO_VENDA_ANEXO>
    {
        List<CRM_PEDIDO_VENDA_ANEXO> GetAllItens();
        CRM_PEDIDO_VENDA_ANEXO GetItemById(Int32 id);
    }
}
