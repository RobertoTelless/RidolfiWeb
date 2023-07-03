using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMotivoCancelamentoService : IServiceBase<MOTIVO_CANCELAMENTO>
    {
        Int32 Create(MOTIVO_CANCELAMENTO perfil, LOG log);
        Int32 Create(MOTIVO_CANCELAMENTO perfil);
        Int32 Edit(MOTIVO_CANCELAMENTO perfil, LOG log);
        Int32 Edit(MOTIVO_CANCELAMENTO perfil);
        Int32 Delete(MOTIVO_CANCELAMENTO perfil, LOG log);

        MOTIVO_CANCELAMENTO CheckExist(MOTIVO_CANCELAMENTO item, Int32 idAss);
        MOTIVO_CANCELAMENTO GetItemById(Int32 id);
        List<MOTIVO_CANCELAMENTO> GetAllItens(Int32 idAss);
        List<MOTIVO_CANCELAMENTO> GetAllItensAdm(Int32 idAss);
    }
}
