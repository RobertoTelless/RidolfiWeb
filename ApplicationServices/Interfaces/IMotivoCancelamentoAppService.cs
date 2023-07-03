using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMotivoCancelamentoAppService : IAppServiceBase<MOTIVO_CANCELAMENTO>
    {
        Int32 ValidateCreate(MOTIVO_CANCELAMENTO item, USUARIO usuario);
        Int32 ValidateEdit(MOTIVO_CANCELAMENTO item, MOTIVO_CANCELAMENTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(MOTIVO_CANCELAMENTO item, USUARIO usuario);
        Int32 ValidateReativar(MOTIVO_CANCELAMENTO item, USUARIO usuario);

        MOTIVO_CANCELAMENTO CheckExist(MOTIVO_CANCELAMENTO item, Int32 idAss);
        List<MOTIVO_CANCELAMENTO> GetAllItens(Int32 idAss);
        MOTIVO_CANCELAMENTO GetItemById(Int32 id);
        List<MOTIVO_CANCELAMENTO> GetAllItensAdm(Int32 idAss);

    }
}
