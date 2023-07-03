using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoTarefaAppService : IAppServiceBase<TIPO_TAREFA>
    {
        Int32 ValidateCreate(TIPO_TAREFA item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_TAREFA item, TIPO_TAREFA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_TAREFA item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_TAREFA item, USUARIO usuario);

        TIPO_TAREFA CheckExist(TIPO_TAREFA item, Int32 idAss);
        List<TIPO_TAREFA> GetAllItens(Int32 idAss);
        TIPO_TAREFA GetItemById(Int32 id);
        List<TIPO_TAREFA> GetAllItensAdm(Int32 idAss);

    }
}
