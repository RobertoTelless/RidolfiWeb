using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILogExcecaoRepository : IRepositoryBase<LOG_EXCECAO_NOVO>
    {
        List<LOG_EXCECAO_NOVO> GetAllItens(Int32 idAss);
        LOG_EXCECAO_NOVO GetItemById(Int32 id);
    }
}
