using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICRMAppService : IAppServiceBase<CRM>
    {
        Int32 ValidateCreate(CRM item, USUARIO usuario);
        Int32 ValidateEdit(CRM item, CRM itemAntes, USUARIO usuario);
        Int32 ValidateEditSimples(CRM item, CRM itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CRM item, CRM itemAntes);
        Int32 ValidateDelete(CRM item, USUARIO usuario);
        Int32 ValidateReativar(CRM item, USUARIO usuario);

        CRM CheckExist(CRM item, Int32 idUsu, Int32 idAss);
        List<CRM> GetByDate(DateTime data, Int32 idAss);
        List<CRM> GetByUser(Int32 user);
        List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss);
        CRM GetItemById(Int32 id);
        List<CRM> GetAllItens(Int32 idAss);
        List<CRM> GetAllItensAdm(Int32 idAss);
        List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss);
        List<CRM_ORIGEM> GetAllOrigens(Int32 idAss);
        List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss);
        List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss);
        CRM_COMENTARIO GetComentarioById(Int32 id);
        CRM_FOLLOW GetFollowById(Int32 id);
        Int32 ValidateEditFollow(CRM_FOLLOW item);
        List<TIPO_FOLLOW> GetAllTipoFollow(Int32 idAss);
        List<CRM_FOLLOW> GetAllFollow(Int32 idAss);
        List<CRM_COMENTARIO> GetAllAnotacao(Int32 idAss);
        Int32 ValidateEditAnexo(CRM_ANEXO item);

        List<CRM_ACAO> GetAllAcoes(Int32 idAss);
        List<TIPO_CRM> GetAllTipos();
        USUARIO GetUserById(Int32 id);
        CRM_ANEXO GetAnexoById(Int32 id);
        Tuple<Int32, List<CRM>, Boolean> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32? temperatura, Int32? funil, String campanha, Int32 idAss);
        Int32 ExecuteFilterVenda(String busca, Int32? status, DateTime? inicio, DateTime? final, Int32? filial, Int32? usuario, Int32 idAss, out List<CRM_PEDIDO_VENDA> objeto);

        CRM_CONTATO GetContatoById(Int32 id);
        CRM_ACAO GetAcaoById(Int32 id);
        Int32 ValidateEditContato(CRM_CONTATO item);
        Int32 ValidateCreateContato(CRM_CONTATO item);
        Int32 ValidateEditAcao(CRM_ACAO item);
        Int32 ValidateCreateAcao(CRM_ACAO item, USUARIO usuario);

        List<TEMPLATE_PROPOSTA> GetAllTemplateProposta(Int32 idAss);
        TEMPLATE_PROPOSTA GetTemplateById(Int32 id);

        List<CRM_PEDIDO_VENDA> GetAllPedidosGeral(Int32 idAss);
        List<CRM_PEDIDO_VENDA> GetAllPedidosVenda(Int32 idAss);
        CRM_PEDIDO_VENDA_ACOMPANHAMENTO GetPedidoComentarioById(Int32 id);
        List<CRM_PEDIDO_VENDA> GetAllPedidos(Int32 idAss);
        CRM_PEDIDO_VENDA GetPedidoById(Int32 id);
        Int32 ValidateEditPedido(CRM_PEDIDO_VENDA item);
        Int32 ValidateCreatePedido(CRM_PEDIDO_VENDA item);

        CRM_PEDIDO_VENDA_ANEXO GetAnexoPedidoById(Int32 id);
        CRM_PEDIDO_VENDA GetPedidoByNumero(String num, Int32 idAss);
        Int32 ValidateCancelarPedido(CRM_PEDIDO_VENDA item);
        Int32 ValidateReprovarPedido(CRM_PEDIDO_VENDA item);
        Int32 ValidateAprovarPedido(CRM_PEDIDO_VENDA item);
        Int32 ValidateEnviarPedido(CRM_PEDIDO_VENDA item);
        Int32 ValidateAprovarPedidoDireto(CRM_PEDIDO_VENDA item);

        Int32 ValidateEditAnotacao(CRM_COMENTARIO item);
        Int32 ValidateCreatePedidoVenda(CRM_PEDIDO_VENDA item);
        Int32 ValidateEditPedidoVenda(CRM_PEDIDO_VENDA item);
        Int32 ValidateCancelarPedidoVenda(CRM_PEDIDO_VENDA item);
        Int32 ValidateReprovarPedidoVenda(CRM_PEDIDO_VENDA item);
        Int32 ValidateAprovarPedidoVenda(CRM_PEDIDO_VENDA item);
        Int32 ValidateEnviarPedidoVenda(CRM_PEDIDO_VENDA item);
        Int32 ValidateExpedicaoPedidoVenda(CRM_PEDIDO_VENDA item);
        Int32 ValidateEntregarPedidoVenda(CRM_PEDIDO_VENDA item);


        //CRM_PEDIDO_VENDA_ITEM GetItemPedidoById(Int32 id);
        //Int32 ValidateEditItemPedido(CRM_PEDIDO_VENDA_ITEM item);
        //Int32 ValidateCreateItemPedido(CRM_PEDIDO_VENDA_ITEM item);

    }
}
