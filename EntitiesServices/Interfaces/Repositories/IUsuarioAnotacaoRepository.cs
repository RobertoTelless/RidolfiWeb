using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUsuarioAnotacaoRepository : IRepositoryBase<USUARIO_ANOTACAO>
    {
        List<USUARIO_ANOTACAO> GetAllItens();
        USUARIO_ANOTACAO GetItemById(Int32 id);
    }
}
