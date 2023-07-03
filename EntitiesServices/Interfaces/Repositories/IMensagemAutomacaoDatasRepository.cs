using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMensagemAutomacaoDatasRepository : IRepositoryBase<MENSAGEM_AUTOMACAO_DATAS>
    {
        List<MENSAGEM_AUTOMACAO_DATAS> GetAllItens();
        MENSAGEM_AUTOMACAO_DATAS GetItemById(Int32 id);

    }
}
