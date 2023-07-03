using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoTarefaRepository : IRepositoryBase<TIPO_TAREFA>
    {
        TIPO_TAREFA CheckExist(TIPO_TAREFA item, Int32 idAss);
        List<TIPO_TAREFA> GetAllItens(Int32 idAss);
        TIPO_TAREFA GetItemById(Int32 id);
        List<TIPO_TAREFA> GetAllItensAdm(Int32 idAss);
    }
}
