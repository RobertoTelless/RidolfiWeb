using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMensagemAutomacaoService : IServiceBase<MENSAGEM_AUTOMACAO>
    {
        Int32 Create(MENSAGEM_AUTOMACAO item, LOG log);
        Int32 Create(MENSAGEM_AUTOMACAO item);
        Int32 Edit(MENSAGEM_AUTOMACAO item, LOG log);
        Int32 Edit(MENSAGEM_AUTOMACAO item);
        Int32 Delete(MENSAGEM_AUTOMACAO item, LOG log);

        MENSAGEM_AUTOMACAO CheckExist(MENSAGEM_AUTOMACAO item, Int32 idAss);
        MENSAGEM_AUTOMACAO GetItemById(Int32 id);
        List<MENSAGEM_AUTOMACAO> GetAllItens(Int32 idAss);
        List<MENSAGEM_AUTOMACAO> GetAllItensAdm(Int32 idAss);
        MENSAGEM_AUTOMACAO_DATAS GetDatasById(Int32 id);
        List<MENSAGEM_AUTOMACAO> ExecuteFilter(Int32? tipo, Int32? grupo, String nome, Int32 idAss);

        List<TEMPLATE_SMS> GetAllTemplatesSMS(Int32 idAss);
        List<TEMPLATE_EMAIL> GetAllTemplatesEMail(Int32 idAss);
        List<GRUPO> GetAllGrupos(Int32 idAss);

        Int32 EditDatas(MENSAGEM_AUTOMACAO_DATAS item);
        Int32 CreateDatas(MENSAGEM_AUTOMACAO_DATAS item);

    }
}
