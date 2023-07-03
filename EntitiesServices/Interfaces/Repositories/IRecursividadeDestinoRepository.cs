using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRecursividadeDestinoRepository : IRepositoryBase<RECURSIVIDADE_DESTINO>
    {
        List<RECURSIVIDADE_DESTINO> GetAllItens();
        RECURSIVIDADE_DESTINO GetItemById(Int32 id);
    }
}
