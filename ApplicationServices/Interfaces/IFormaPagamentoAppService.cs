using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFormaPagamentoAppService : IAppServiceBase<FORMA_PAGAMENTO>
    {
        Int32 ValidateCreate(FORMA_PAGAMENTO item, USUARIO usuario);
        Int32 ValidateEdit(FORMA_PAGAMENTO item, FORMA_PAGAMENTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(FORMA_PAGAMENTO item, USUARIO usuario);
        Int32 ValidateReativar(FORMA_PAGAMENTO item, USUARIO usuario);

        FORMA_PAGAMENTO CheckExist(FORMA_PAGAMENTO conta, Int32 idAss);
        List<FORMA_PAGAMENTO> GetAllItensTipo(Int32 tipo, Int32 idAss);
        FORMA_PAGAMENTO GetItemById(Int32 id);
        List<FORMA_PAGAMENTO> GetAllItensAdm(Int32 idAss);
        List<FORMA_PAGAMENTO> GetAllItens(Int32 idAss);
    }
}
