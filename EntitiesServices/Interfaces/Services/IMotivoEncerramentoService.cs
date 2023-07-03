using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMotivoEncerramentoService : IServiceBase<MOTIVO_ENCERRAMENTO>
    {
        Int32 Create(MOTIVO_ENCERRAMENTO perfil, LOG log);
        Int32 Create(MOTIVO_ENCERRAMENTO perfil);
        Int32 Edit(MOTIVO_ENCERRAMENTO perfil, LOG log);
        Int32 Edit(MOTIVO_ENCERRAMENTO perfil);
        Int32 Delete(MOTIVO_ENCERRAMENTO perfil, LOG log);

        MOTIVO_ENCERRAMENTO CheckExist(MOTIVO_ENCERRAMENTO item, Int32 idAss);
        MOTIVO_ENCERRAMENTO GetItemById(Int32 id);
        List<MOTIVO_ENCERRAMENTO> GetAllItens(Int32 idAss);
        List<MOTIVO_ENCERRAMENTO> GetAllItensAdm(Int32 idAss);
    }
}
