using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPesquisaAnexoRepository : IRepositoryBase<PESQUISA_ANEXO>
    {
        List<PESQUISA_ANEXO> GetAllItens();
        PESQUISA_ANEXO GetItemById(Int32 id);
    }
}
