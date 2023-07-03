using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAssinantePagamentoRepository : IRepositoryBase<ASSINANTE_PAGAMENTO>
    {
        List<ASSINANTE_PAGAMENTO> GetAllItens();
        ASSINANTE_PAGAMENTO GetItemById(Int32 id);
    }
}
