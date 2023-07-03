using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAssinanteService : IServiceBase<ASSINANTE>
    {
        Int32 Create(ASSINANTE perfil, LOG log);
        Int32 Create(ASSINANTE perfil);
        Int32 Edit(ASSINANTE perfil, LOG log);
        Int32 Edit(ASSINANTE perfil);
        Int32 Delete(ASSINANTE perfil, LOG log);

        ASSINANTE CheckExist(ASSINANTE conta);
        ASSINANTE GetItemById(Int32 id);
        List<ASSINANTE> GetAllItens();
        List<ASSINANTE> GetAllItensAdm();
        List<ASSINANTE> ExecuteFilter(Int32? tipo, String nome, String cpf, String cnpj, String cidade, Int32? uf, Int32? status);

        List<ASSINANTE_PAGAMENTO> ExecuteFilterAtraso(String nome, String cpf, String cnpj, String cidade, Int32? uf);
        List<ASSINANTE_PLANO> ExecuteFilterVencidos(String nome, String cpf, String cnpj, String cidade, Int32? uf);
        List<ASSINANTE_PLANO> ExecuteFilterVencer30(String nome, String cpf, String cnpj, String cidade, Int32? uf);

        List<TIPO_PESSOA> GetAllTiposPessoa();
        List<PLANO> GetAllPlanos();
        List<UF> GetAllUF();
        ASSINANTE_ANEXO GetAnexoById(Int32 id);
        UF GetUFBySigla(String sigla);
        ASSINANTE_ANOTACAO GetAnotacaoById(Int32 id);
        CONFIGURACAO_CHAVES GetChaves(Int32 id);

        List<ASSINANTE_PAGAMENTO> GetAllPagamentos();

        ASSINANTE_PAGAMENTO GetPagtoById(Int32 id);
        Int32 EditPagto(ASSINANTE_PAGAMENTO item);
        Int32 CreatePagto(ASSINANTE_PAGAMENTO item);

        ASSINANTE_PLANO GetPlanoById(Int32 id);
        Int32 EditPlano(ASSINANTE_PLANO item);
        Int32 CreatePlano(ASSINANTE_PLANO item);
        ASSINANTE_PLANO GetByAssPlan(Int32 plan, Int32 assi);
        List<ASSINANTE_PLANO> GetAllAssPlanos();
        PLANO GetPlanoBaseById(Int32 id);
    }
}
