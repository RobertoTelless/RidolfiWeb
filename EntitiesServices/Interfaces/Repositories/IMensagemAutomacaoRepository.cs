using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMensagemAutomacaoRepository : IRepositoryBase<MENSAGEM_AUTOMACAO>
    {
        MENSAGEM_AUTOMACAO CheckExist(MENSAGEM_AUTOMACAO item, Int32 idAss);
        MENSAGEM_AUTOMACAO GetItemById(Int32 id);
        List<MENSAGEM_AUTOMACAO> GetAllItens(Int32 idAss);
        List<MENSAGEM_AUTOMACAO> GetAllItensAdm(Int32 idAss);
        List<MENSAGEM_AUTOMACAO> ExecuteFilter(Int32? tipo, Int32? grupo, String nome, Int32 idAss);

    }
}
