using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoPesquisaRepository : IRepositoryBase<TIPO_PESQUISA>
    {
        TIPO_PESQUISA CheckExist(TIPO_PESQUISA item, Int32 idAss);
        List<TIPO_PESQUISA> GetAllItens(Int32 idAss);
        TIPO_PESQUISA GetItemById(Int32 id);
        List<TIPO_PESQUISA> GetAllItensAdm(Int32 idAss);
    }
}
