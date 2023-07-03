using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMensagemAnexoRepository : IRepositoryBase<MENSAGEM_ANEXO>
    {
        List<MENSAGEM_ANEXO> GetAllItens();
        MENSAGEM_ANEXO GetItemById(Int32 id);

    }
}
