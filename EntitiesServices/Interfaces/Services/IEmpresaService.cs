using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IEmpresaService : IServiceBase<EMPRESA>
    {
        Int32 Create(EMPRESA perfil, LOG log);
        Int32 Create(EMPRESA perfil);
        Int32 Edit(EMPRESA perfil, LOG log);
        Int32 Edit(EMPRESA perfil);
        Int32 Delete(EMPRESA perfil, LOG log);

        EMPRESA CheckExist(EMPRESA item, Int32 idAss);
        EMPRESA GetItemById(Int32 id);
        EMPRESA GetItemByAssinante(Int32 id);
        List<EMPRESA> GetAllItens(Int32 idAss);
        List<EMPRESA> GetAllItensAdm(Int32 idAss);
        List<EMPRESA> ExecuteFilter(String nome, Int32? idAss);

        List<MAQUINA> GetAllMaquinas(Int32 idAss);
        List<REGIME_TRIBUTARIO> GetAllRegimes();
        EMPRESA_ANEXO GetAnexoById(Int32 id);
        REGIME_TRIBUTARIO GetRegimeById(Int32 id);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

        EMPRESA_MAQUINA CheckExistMaquina(EMPRESA_MAQUINA item, Int32 idAss);
        EMPRESA_MAQUINA GetMaquinaById(Int32 id);
        Int32 EditMaquina(EMPRESA_MAQUINA item);
        Int32 CreateMaquina(EMPRESA_MAQUINA item);
        EMPRESA_MAQUINA GetByEmpresaMaquina(Int32 empresa, Int32 maquina);
       
        EMPRESA_PLATAFORMA CheckExistPlataforma(EMPRESA_PLATAFORMA item, Int32 idAss);
        EMPRESA_PLATAFORMA GetPlataformaById(Int32 id);
        Int32 EditPlataforma(EMPRESA_PLATAFORMA item);
        Int32 CreatePlataforma(EMPRESA_PLATAFORMA item);
        EMPRESA_PLATAFORMA GetByEmpresaPlataforma(Int32 empresa, Int32 plataforma);

        EMPRESA_CUSTO_VARIAVEL CheckExistCustoVariavel(EMPRESA_CUSTO_VARIAVEL item, Int32 idAss);
        EMPRESA_CUSTO_VARIAVEL GetCustoVariavelById(Int32 id);
        Int32 EditCustoVariavel(EMPRESA_CUSTO_VARIAVEL item);
        Int32 CreateCustoVariavel(EMPRESA_CUSTO_VARIAVEL item);
        List<EMPRESA_CUSTO_VARIAVEL> GetAllCustoVariavel(Int32 idAss);

        EMPRESA_TICKET CheckExistTicket(EMPRESA_TICKET item, Int32 idAss);
        EMPRESA_TICKET GetTicketById(Int32 id);
        Int32 EditTicket(EMPRESA_TICKET item);
        Int32 CreateTicket(EMPRESA_TICKET item);
        EMPRESA_TICKET GetByEmpresaTicket(Int32 empresa, Int32 ticket);

        CUSTO_VARIAVEL_CALCULO GetCustoVariavelCalculoById(Int32 id);
        Int32 EditCustoVariavelCalculo(CUSTO_VARIAVEL_CALCULO item);
        Int32 CreateCustoVariavelCalculo(CUSTO_VARIAVEL_CALCULO item);
        List<CUSTO_VARIAVEL_CALCULO> ExecuteFilterCalculo(DateTime? data, Int32? idAss);
        List<CUSTO_VARIAVEL_CALCULO> GetAllCalculo(Int32 idAss);

        CUSTO_VARIAVEL_HISTORICO GetCustoVariavelHistoricoById(Int32 id);
        Int32 EditCustoVariavelHistorico(CUSTO_VARIAVEL_HISTORICO item);
        Int32 CreateCustoVariavelHistorico(CUSTO_VARIAVEL_HISTORICO item);
        List<CUSTO_VARIAVEL_HISTORICO> ExecuteFilterHistorico(DateTime? data, Int32? idAss);
        List<CUSTO_VARIAVEL_HISTORICO> GetAllHistorico(Int32 idAss);

        CUSTO_HISTORICO GetCustoHistoricoById(Int32 id);
        Int32 CreateCustoHistorico(CUSTO_HISTORICO item);
        Int32 EditCustoHistorico(CUSTO_HISTORICO item);
        List<CUSTO_HISTORICO> ExecuteFilterCustoHistorico(DateTime? data, Int32? idAss);
        List<CUSTO_HISTORICO> GetAllCustoHistorico(Int32 idAss);

        COMISSAO GetComissaoById(Int32 id);
        Int32 CreateComissao(COMISSAO item);
        Int32 EditComissao(COMISSAO item);
        List<COMISSAO> ExecuteFilterComissao(DateTime? data, Int32? idAss);
        List<COMISSAO> GetAllComissao(Int32 idAss);
    }
}
