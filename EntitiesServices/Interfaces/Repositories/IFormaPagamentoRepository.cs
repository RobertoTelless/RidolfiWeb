using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormaPagamentoRepository : IRepositoryBase<FORMA_PAGAMENTO>
    {
        FORMA_PAGAMENTO CheckExist(FORMA_PAGAMENTO conta, Int32 idAss);
        List<FORMA_PAGAMENTO> GetAllItensTipo(Int32 tipo, Int32 idAss);
        List<FORMA_PAGAMENTO> GetAllItensAdm(Int32 idAss);
        FORMA_PAGAMENTO GetItemById(Int32 id);
        List<FORMA_PAGAMENTO> GetAllItens(Int32 idAss);
    }
}
