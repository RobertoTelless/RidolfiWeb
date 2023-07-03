using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMensagemDestinoRepository : IRepositoryBase<MENSAGENS_DESTINOS>
    {
        List<MENSAGENS_DESTINOS> GetAllItens();
        MENSAGENS_DESTINOS GetItemById(Int32 id);

    }
}
