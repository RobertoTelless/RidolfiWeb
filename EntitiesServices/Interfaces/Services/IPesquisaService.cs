using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPesquisaService : IServiceBase<PESQUISA>
    {
        Int32 Create(PESQUISA perfil, LOG log);
        Int32 Create(PESQUISA perfil);
        Int32 Edit(PESQUISA perfil, LOG log);
        Int32 Edit(PESQUISA perfil);
        Int32 Delete(PESQUISA perfil, LOG log);

        PESQUISA CheckExist(PESQUISA conta, Int32 idAss);
        PESQUISA GetItemById(Int32 id);
        List<PESQUISA> GetAllItens(Int32 idAss);
        List<PESQUISA> GetAllItensAdm(Int32 idAss);
        List<TIPO_PESQUISA> GetAllTipos(Int32 idAss);
        List<TIPO_ITEM_PESQUISA> GetAllTiposItem(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<PESQUISA_RESPOSTA> GetAllRespostas(Int32 idAss);

        PESQUISA_ANEXO GetAnexoById(Int32 id);
        PESQUISA_ITEM GetItemPesquisaById(Int32 id);
        PESQUISA_ITEM_OPCAO GetItemOpcaoPesquisaById(Int32 id);
        List<PESQUISA> ExecuteFilter(Int32? catId, String nome, String descricao, String campanha, Int32 idAss);
        PESQUISA_ANOTACAO GetComentarioById(Int32 id);
        PESQUISA_ENVIO GetPesquisaEnvioById(Int32 id);

        Int32 EditItemPesquisa(PESQUISA_ITEM item);
        Int32 CreateItemPesquisa(PESQUISA_ITEM item);
        Int32 EditAnotacao(PESQUISA_ANOTACAO item);
        Int32 EditItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item);
        Int32 CreateItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item);
        Int32 CreatePesquisaEnvio(PESQUISA_ENVIO item);

        PESQUISA_RESPOSTA GetPesquisaRespostaById(Int32 id);
        Int32 EditPesquisaResposta(PESQUISA_RESPOSTA item);
        Int32 CreatePesquisaResposta(PESQUISA_RESPOSTA item);

    }
}
