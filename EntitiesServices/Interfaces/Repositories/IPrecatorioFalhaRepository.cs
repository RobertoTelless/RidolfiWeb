using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPrecatorioFalhaRepository : IRepositoryBase<PRECATORIO_FALHA>
    {
        List<PRECATORIO_FALHA> GetAllItens(Int32 idAss);
        PRECATORIO_FALHA GetItemById(Int32 id);
    }
}
