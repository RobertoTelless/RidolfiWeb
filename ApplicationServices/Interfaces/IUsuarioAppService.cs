using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IUsuarioAppService : IAppServiceBase<USUARIO>
    {
        USUARIO GetByEmail(String email, Int32 idAss);
        USUARIO GetByLogin(String login, Int32 idAss);
        List<USUARIO> GetAllUsuariosAdm(Int32 idAss);
        USUARIO GetItemById(Int32 id);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<USUARIO> GetAllItens(Int32 idAss);
        List<USUARIO> GetAllItensBloqueados(Int32 idAss);
        List<USUARIO> GetAllItensAcessoHoje(Int32 idAss);
        USUARIO_ANEXO GetAnexoById(Int32 id);
        List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss);
        List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss);
        USUARIO CheckExist(USUARIO item, Int32 idAss);

        Int32 ValidateCreate(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateCreateAssinante(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioAntes, USUARIO usuarioLogado);
        Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateLogin(String email, String senha, out USUARIO usuario);
        Int32 ValidateDelete(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateBloqueio(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateDesbloqueio(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateChangePassword(USUARIO usuario);
        Int32 ValidateChangePasswordFinal(USUARIO usuario);
        Int32 ValidateReativar(USUARIO usuario, USUARIO usuarioLogado);
        List<VOLTA_PESQUISA> PesquisarTudo(String parm, Int32 idUsu, Int32 idAss);
        Int32 ValidateEditAnexo(USUARIO_ANEXO item);

        Task<Int32> GenerateNewPassword(String email);
        List<PERFIL> GetAllPerfis();
        Tuple<Int32, List<USUARIO>, Boolean> ExecuteFilter(Int32? perfilId, Int32? cargoId, String nome, String login, String email, Int32 idAss);
        List<NOTICIA> GetAllNoticias(Int32 idAss);
        USUARIO GetAdministrador(Int32 idAss);
        List<CARGO> GetAllCargos(Int32 idAss);
        USUARIO_ANOTACAO GetAnotacaoById(Int32 id);
        Tuple<Int32, List<LOG_EXCECAO_NOVO>, Boolean> ExecuteFilterExcecao(Int32? usuaId, DateTime? data, String gerador, Int32 idAss);

        LOG_EXCECAO_NOVO GetLogExcecaoById(Int32 id);
        List<LOG_EXCECAO_NOVO> GetAllLogExcecao(Int32 idAss);
        Int32 ValidateCreateLogExcecao(LOG_EXCECAO_NOVO log);
    }
}
