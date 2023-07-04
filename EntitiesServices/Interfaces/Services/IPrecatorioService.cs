using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPrecatorioService : IServiceBase<PRECATORIO>
    {
        Int32 Create(PRECATORIO perfil, LOG log);
        Int32 Create(PRECATORIO perfil);
        Int32 Edit(PRECATORIO perfil, LOG log);
        Int32 Edit(PRECATORIO perfil);
        Int32 Delete(PRECATORIO perfil, LOG log);

        PRECATORIO CheckExist(PRECATORIO conta);
        PRECATORIO CheckExist(String item);
        PRECATORIO GetItemById(Int32 id);
        List<PRECATORIO> GetAllItens();
        List<PRECATORIO> GetAllItensAdm();
        List<PRECATORIO> ExecuteFilter(Int32? trf, Int32? beneficiario, Int32? advogado, Int32? natureza, Int32? estado, String nome, String ano, Int32? crm, Int32? pesquisa, Decimal? valor1, Decimal? valor2, Int32? situacao);

        List<TRF> GetAllTRF();
        List<BENEFICIARIO> GetAllBeneficiarios();
        List<HONORARIO> GetAllAdvogados();
        List<BANCO> GetAllBancos();
        List<NATUREZA> GetAllNaturezas();
        List<PRECATORIO_ESTADO> GetAllEstados();
        NATUREZA CheckExistNatureza(String natureza);
        CONTATO CheckExistContato(String prot);
        Int32 EditAnotacao(PRECATORIO_ANOTACAO item);

        PRECATORIO_ANEXO GetAnexoById(Int32 id);
        PRECATORIO_ANOTACAO GetComentarioById(Int32 id);
        Int32 CreateFalha(PRECATORIO_FALHA item);
        Int32 CreateFalhaContato(CONTATO_FALHA item);
    }
}
