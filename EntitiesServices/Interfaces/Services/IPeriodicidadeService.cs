using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPeriodicidadeService : IServiceBase<PERIODICIDADE_TAREFA>
    {
        Int32 Create(PERIODICIDADE_TAREFA perfil, LOG log);
        Int32 Create(PERIODICIDADE_TAREFA perfil);
        Int32 Edit(PERIODICIDADE_TAREFA perfil, LOG log);
        Int32 Edit(PERIODICIDADE_TAREFA perfil);
        Int32 Delete(PERIODICIDADE_TAREFA perfil, LOG log);

        PERIODICIDADE_TAREFA CheckExist(PERIODICIDADE_TAREFA item);
        List<PERIODICIDADE_TAREFA> GetAllItens();
        PERIODICIDADE_TAREFA GetItemById(Int32 id);
        List<PERIODICIDADE_TAREFA> GetAllItensAdm();
    }
}
