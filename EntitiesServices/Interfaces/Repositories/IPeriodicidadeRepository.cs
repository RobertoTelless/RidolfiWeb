using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPeriodicidadeRepository : IRepositoryBase<PERIODICIDADE_TAREFA>
    {
        PERIODICIDADE_TAREFA CheckExist(PERIODICIDADE_TAREFA item);
        List<PERIODICIDADE_TAREFA> GetAllItens();
        PERIODICIDADE_TAREFA GetItemById(Int32 id);
        List<PERIODICIDADE_TAREFA> GetAllItensAdm();
    }
}
