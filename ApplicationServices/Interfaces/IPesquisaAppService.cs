using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPesquisaAppService : IAppServiceBase<PESQUISA>
    {
        Int32 ValidateCreate(PESQUISA perfil, USUARIO usuario);
        Int32 ValidateEdit(PESQUISA perfil, PESQUISA perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(PESQUISA item, PESQUISA itemAntes);
        Int32 ValidateDelete(PESQUISA perfil, USUARIO usuario);
        Int32 ValidateReativar(PESQUISA perfil, USUARIO usuario);
        Tuple<Int32, String, Boolean> ValidateEnviarPesquisa(PESQUISA perfil, USUARIO usuario);

        List<PESQUISA> GetAllItens(Int32 idAss);
        List<PESQUISA> GetAllItensAdm(Int32 idAss);
        PESQUISA GetItemById(Int32 id);
        PESQUISA CheckExist(PESQUISA conta, Int32 idAss);
        List<PESQUISA_RESPOSTA> GetAllRespostas(Int32 idAss);

        List<TIPO_PESQUISA> GetAllTipos(Int32 idAss);
        List<TIPO_ITEM_PESQUISA> GetAllTiposItem(Int32 idAss);
        PESQUISA_ANEXO GetAnexoById(Int32 id);
        PESQUISA_ITEM GetItemPesquisaById(Int32 id);
        PESQUISA_ANOTACAO GetComentarioById(Int32 id);
        PESQUISA_ITEM_OPCAO GetItemOpcaoPesquisaById(Int32 id);
        PESQUISA_ENVIO GetPesquisaEnvioById(Int32 id);

        Int32 ExecuteFilter(Int32? catId, String nome, String descricao, String campanha, Int32 idAss, out List<PESQUISA> objeto);
        Tuple<Int32, List<PESQUISA>, Boolean> ExecuteFilterTuple(Int32? catId, String nome, String descricao, String campanha, Int32 idAss);

        Int32 ValidateEditAnotacao(PESQUISA_ANOTACAO item);
        Int32 ValidateEditItemPesquisa(PESQUISA_ITEM item, PESQUISA pesq);
        Int32 ValidateCreateItemPesquisa(PESQUISA_ITEM item, PESQUISA pesq);
        Int32 ValidateDeleteItemPesquisa(PESQUISA_ITEM item);
        Int32 ValidateEditItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item, PESQUISA_ITEM pesq);
        Int32 ValidateCreateItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item, PESQUISA_ITEM pesq);
        Int32 ValidateDeleteItemOpcaoPesquisa(PESQUISA_ITEM_OPCAO item);
        Int32 ValidateCreatePesquisaEnvio(PESQUISA_ENVIO item);
        PESQUISA_RESPOSTA GetPesquisaRespostaById(Int32 id);
        Int32 ValidateEditPesquisaResposta(PESQUISA_RESPOSTA item);
        Int32 ValidateCreatePesquisaResposta(PESQUISA_RESPOSTA item);

    }
}
