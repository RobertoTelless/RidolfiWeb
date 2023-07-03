using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPrecatorioAnotacaoRepository : IRepositoryBase<PRECATORIO_ANOTACAO>
    {
        List<PRECATORIO_ANOTACAO> GetAllItens();
        PRECATORIO_ANOTACAO GetItemById(Int32 id);
    }
}
