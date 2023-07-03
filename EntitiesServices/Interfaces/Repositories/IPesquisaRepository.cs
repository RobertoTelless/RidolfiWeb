using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPesquisaRepository : IRepositoryBase<PESQUISA>
    {
        PESQUISA CheckExist(PESQUISA item, Int32 idAss);
        PESQUISA GetItemById(Int32 id);
        List<PESQUISA> GetAllItens(Int32 idAss);
        List<PESQUISA> GetAllItensAdm(Int32 idAss);
        List<PESQUISA> ExecuteFilter(Int32? catId, String nome, String descricao, String campanha, Int32 idAss);
    }
}
