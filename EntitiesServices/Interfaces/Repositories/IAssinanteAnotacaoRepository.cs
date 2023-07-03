using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAssinanteAnotacaoRepository : IRepositoryBase<ASSINANTE_ANOTACAO>
    {
        List<ASSINANTE_ANOTACAO> GetAllItens();
        ASSINANTE_ANOTACAO GetItemById(Int32 id);
    }
}
