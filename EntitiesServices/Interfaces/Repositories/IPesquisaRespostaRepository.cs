using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPesquisaRespostaRepository : IRepositoryBase<PESQUISA_RESPOSTA>
    {
        List<PESQUISA_RESPOSTA> GetAllItens(Int32 idAss);
        PESQUISA_RESPOSTA GetItemById(Int32 id);
    }
}
