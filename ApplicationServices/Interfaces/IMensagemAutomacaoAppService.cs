using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMensagemAutomacaoAppService : IAppServiceBase<MENSAGEM_AUTOMACAO>
    {
        Int32 ValidateCreate(MENSAGEM_AUTOMACAO item, USUARIO usuario);
        Int32 ValidateEdit(MENSAGEM_AUTOMACAO item, MENSAGEM_AUTOMACAO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(MENSAGEM_AUTOMACAO item, MENSAGEM_AUTOMACAO itemAntes);
        Int32 ValidateDelete(MENSAGEM_AUTOMACAO item, USUARIO usuario);
        Int32 ValidateReativar(MENSAGEM_AUTOMACAO item, USUARIO usuario);

        List<MENSAGEM_AUTOMACAO> GetAllItens(Int32 idAss);
        List<MENSAGEM_AUTOMACAO> GetAllItensAdm(Int32 idAss);
        MENSAGEM_AUTOMACAO GetItemById(Int32 id);
        MENSAGEM_AUTOMACAO CheckExist(MENSAGEM_AUTOMACAO conta, Int32 idAss);
        MENSAGEM_AUTOMACAO_DATAS GetDatasById(Int32 id);
        Int32 ExecuteFilter(Int32? tipo, Int32? grupo, String nome, Int32 idAss, out List<MENSAGEM_AUTOMACAO> objeto);

        List<GRUPO> GetAllGrupos(Int32 idAss);
        List<TEMPLATE_SMS> GetAllTemplatesSMS(Int32 idAss);
        List<TEMPLATE_EMAIL> GetAllTemplatesEMail(Int32 idAss);

        Int32 ValidateEditDatas(MENSAGEM_AUTOMACAO_DATAS item);
        Int32 ValidateCreateDatas(MENSAGEM_AUTOMACAO_DATAS item);
    }
}
