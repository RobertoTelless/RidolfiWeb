using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPedidoVendaRepository : IRepositoryBase<PEDIDO_VENDA>
    {
        PEDIDO_VENDA GetItemById(Int32 id);
        List<PEDIDO_VENDA> GetAllItens(Int32 idAss);
    }
}
