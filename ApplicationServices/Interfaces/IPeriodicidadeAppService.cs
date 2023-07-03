using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPeriodicidadeAppService : IAppServiceBase<PERIODICIDADE_TAREFA>
    {
        Int32 ValidateCreate(PERIODICIDADE_TAREFA item, USUARIO usuario);
        Int32 ValidateEdit(PERIODICIDADE_TAREFA item, PERIODICIDADE_TAREFA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(PERIODICIDADE_TAREFA item, USUARIO usuario);
        Int32 ValidateReativar(PERIODICIDADE_TAREFA item, USUARIO usuario);

        PERIODICIDADE_TAREFA CheckExist(PERIODICIDADE_TAREFA item);
        List<PERIODICIDADE_TAREFA> GetAllItens();
        PERIODICIDADE_TAREFA GetItemById(Int32 id);
        List<PERIODICIDADE_TAREFA> GetAllItensAdm();
        List<PERIODICIDADE_TAREFA> GetByAssinante(USUARIO usuario);

    }
}
