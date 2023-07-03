using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMotivoEncerramentoAppService : IAppServiceBase<MOTIVO_ENCERRAMENTO>
    {
        Int32 ValidateCreate(MOTIVO_ENCERRAMENTO item, USUARIO usuario);
        Int32 ValidateEdit(MOTIVO_ENCERRAMENTO item, MOTIVO_ENCERRAMENTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(MOTIVO_ENCERRAMENTO item, USUARIO usuario);
        Int32 ValidateReativar(MOTIVO_ENCERRAMENTO item, USUARIO usuario);

        MOTIVO_ENCERRAMENTO CheckExist(MOTIVO_ENCERRAMENTO item, Int32 idAss);
        List<MOTIVO_ENCERRAMENTO> GetAllItens(Int32 idAss);
        MOTIVO_ENCERRAMENTO GetItemById(Int32 id);
        List<MOTIVO_ENCERRAMENTO> GetAllItensAdm(Int32 idAss);

    }
}
