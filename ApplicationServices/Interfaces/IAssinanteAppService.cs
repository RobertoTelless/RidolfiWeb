using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAssinanteAppService : IAppServiceBase<ASSINANTE>
    {
        Int32 ValidateCreate(ASSINANTE perfil, USUARIO usuario);
        Int32 ValidateEdit(ASSINANTE perfil, ASSINANTE perfilAntes, USUARIO usuario);
        Int32 ValidateDelete(ASSINANTE perfil, USUARIO usuario);
        Int32 ValidateReativar(ASSINANTE perfil, USUARIO usuario);

        ASSINANTE CheckExist(ASSINANTE conta);
        List<ASSINANTE> GetAllItens();
        List<ASSINANTE> GetAllItensAdm();
        ASSINANTE GetItemById(Int32 id);
        Tuple<Int32, List<ASSINANTE>, Boolean> ExecuteFilter(Int32? tipo, String nome, String cpf, String cnpj, String cidade, Int32? uf, Int32? status);

        Int32 ExecuteFilterAtraso(String nome, String cpf, String cnpj, String cidade, Int32? uf, out List<ASSINANTE_PAGAMENTO> objeto);
        Int32 ExecuteFilterVencidos(String nome, String cpf, String cnpj, String cidade, Int32? uf, out List<ASSINANTE_PLANO> objeto);
        Int32 ExecuteFilterVencer30(String nome, String cpf, String cnpj, String cidade, Int32? uf, out List<ASSINANTE_PLANO> objeto);

        List<TIPO_PESSOA> GetAllTiposPessoa();
        List<PLANO> GetAllPlanos();
        List<UF> GetAllUF();
        ASSINANTE_ANEXO GetAnexoById(Int32 id);
        UF GetUFBySigla(String sigla);
        ASSINANTE_ANOTACAO GetAnotacaoById(Int32 id);
        List<ASSINANTE_PAGAMENTO> GetAllPagamentos();
        CONFIGURACAO_CHAVES GetChaves(Int32 id);

        ASSINANTE_PAGAMENTO GetPagtoById(Int32 id);
        Int32 ValidateEditPagto(ASSINANTE_PAGAMENTO item);
        Int32 ValidateCreatePagto(ASSINANTE_PAGAMENTO item);
        ASSINANTE_PLANO GetPlanoById(Int32 id);
        Int32 ValidateEditPlano(ASSINANTE_PLANO item);
        Int32 ValidateCreatePlano(ASSINANTE_PLANO item);
        ASSINANTE_PLANO GetByAssPlan(Int32 plan, Int32 assi);
        List<ASSINANTE_PLANO> GetAllAssPlanos();
        PLANO GetPlanoBaseById(Int32 id);

    }
}
