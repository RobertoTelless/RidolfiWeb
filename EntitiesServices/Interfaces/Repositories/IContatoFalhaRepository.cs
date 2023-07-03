using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContatoFalhaRepository : IRepositoryBase<CONTATO_FALHA>
    {
        List<CONTATO_FALHA> GetAllItens(Int32 idAss);
        CONTATO_FALHA GetItemById(Int32 id);
    }
}
