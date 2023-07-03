using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaAgendaRepository : IRepositoryBase<CATEGORIA_AGENDA>
    {
        CATEGORIA_AGENDA CheckExist(CATEGORIA_AGENDA item, Int32 idAss);
        List<CATEGORIA_AGENDA> GetAllItens(Int32 idAss);
        CATEGORIA_AGENDA GetItemById(Int32 id);
        List<CATEGORIA_AGENDA> GetAllItensAdm(Int32 idAss);
        Task < IEnumerable < CATEGORIA_AGENDA >> GetAllItensAsync(Int32 idAss);
    }
}
