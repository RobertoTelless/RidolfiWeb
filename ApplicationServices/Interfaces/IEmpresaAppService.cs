using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IEmpresaAppService : IAppServiceBase<EMPRESA>
    {
        Int32 ValidateCreate(EMPRESA item, USUARIO usuario);
        Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes);
        Int32 ValidateDelete(EMPRESA item, USUARIO usuario);
        Int32 ValidateReativar(EMPRESA item, USUARIO usuario);

        EMPRESA CheckExist(EMPRESA item, Int32 idAss);
        EMPRESA GetItemById(Int32 id);
        EMPRESA GetItemByAssinante(Int32 id);
        List<EMPRESA> GetAllItens(Int32 idAss);
        List<EMPRESA> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<EMPRESA>, Boolean> ExecuteFilterTuple(String nome, Int32 idAss);

        List<MAQUINA> GetAllMaquinas(Int32 idAss);
        List<REGIME_TRIBUTARIO> GetAllRegimes();
        EMPRESA_ANEXO GetAnexoById(Int32 id);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

        EMPRESA_MAQUINA GetMaquinaById(Int32 id);
        EMPRESA_PLATAFORMA GetPlataformaById(Int32 id);
        REGIME_TRIBUTARIO GetRegimeById(Int32 id);
        Int32 ValidateEditMaquina(EMPRESA_MAQUINA item);
        Int32 ValidateCreateMaquina(EMPRESA_MAQUINA item, Int32 idAss);
        Int32 ValidateEditPlataforma(EMPRESA_PLATAFORMA item);
        Int32 ValidateCreatePlataforma(EMPRESA_PLATAFORMA item, Int32 idAss);
        EMPRESA_MAQUINA GetByEmpresaMaquina(Int32 empresa, Int32 maquina);
        EMPRESA_PLATAFORMA GetByEmpresaPlataforma(Int32 empresa, Int32 plataforma);

        EMPRESA_CUSTO_VARIAVEL CheckExistCustoVariavel(EMPRESA_CUSTO_VARIAVEL item, Int32 idAss);
        EMPRESA_CUSTO_VARIAVEL GetCustoVariavelById(Int32 id);
        Int32 ValidateEditCustoVariavel(EMPRESA_CUSTO_VARIAVEL item);
        Int32 ValidateCreateCustoVariavel(EMPRESA_CUSTO_VARIAVEL item, Int32 idAss);
        List<EMPRESA_CUSTO_VARIAVEL> GetAllCustoVariavel(Int32 idAss);

        EMPRESA_TICKET CheckExistTicket(EMPRESA_TICKET item, Int32 idAss);
        EMPRESA_TICKET GetTicketById(Int32 id);
        Int32 ValidateEditTicket(EMPRESA_TICKET item);
        Int32 ValidateCreateTicket(EMPRESA_TICKET item, Int32 idAss);
        EMPRESA_TICKET GetByEmpresaTicket(Int32 empresa, Int32 ticket);

        CUSTO_VARIAVEL_CALCULO GetCustoVariavelCalculoById(Int32 id);
        Int32 ValidateEditCustoVariavelCalculo(CUSTO_VARIAVEL_CALCULO item);
        Int32 ValidateCreateCustoVariavelCalculo(CUSTO_VARIAVEL_CALCULO item);
        Tuple<Int32, List<CUSTO_VARIAVEL_CALCULO>, Boolean> ExecuteFilterCalculo(DateTime? data, Int32? idAss);
        List<CUSTO_VARIAVEL_CALCULO> GetAllCalculo(Int32 idAss);

        CUSTO_VARIAVEL_HISTORICO GetCustoVariavelHistoricoById(Int32 id);
        Int32 ValidateEditCustoVariavelHistorico(CUSTO_VARIAVEL_HISTORICO item);
        Int32 ValidateCreateCustoVariavelHistorico(CUSTO_VARIAVEL_HISTORICO item);
        Tuple<Int32, List<CUSTO_VARIAVEL_HISTORICO>, Boolean> ExecuteFilterHistorico(DateTime? data, Int32? idAss);
        List<CUSTO_VARIAVEL_HISTORICO> GetAllHistorico(Int32 idAss);

        CUSTO_HISTORICO GetCustoHistoricoById(Int32 id);
        Int32 ValidateCreateCustoHistorico(CUSTO_HISTORICO item);
        Int32 ValidateEditCustoHistorico(CUSTO_HISTORICO item);
        Tuple<Int32, List<CUSTO_HISTORICO>, Boolean> ExecuteFilterCustoHistorico(DateTime? data, Int32? idAss);
        List<CUSTO_HISTORICO> GetAllCustoHistorico(Int32 idAss);

    }
}
