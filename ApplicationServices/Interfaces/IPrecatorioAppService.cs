using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPrecatorioAppService : IAppServiceBase<PRECATORIO>
    {
        Int32 ValidateCreate(PRECATORIO perfil, USUARIO usuario);
        Int32 ValidateEdit(PRECATORIO perfil, PRECATORIO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(PRECATORIO item, PRECATORIO itemAntes);
        Int32 ValidateDelete(PRECATORIO perfil, USUARIO usuario);
        Int32 ValidateReativar(PRECATORIO perfil, USUARIO usuario);

        PRECATORIO CheckExist(PRECATORIO conta);
        PRECATORIO CheckExist(String item);
        PRECATORIO GetItemById(Int32 id);
        List<PRECATORIO> GetAllItens();
        List<PRECATORIO> GetAllItensAdm();
        Int32 ExecuteFilter(Int32? trf, Int32? beneficiario, Int32? advogado, Int32? natureza, Int32? estado, String nome, String ano, Int32? crm, Int32? pesquisa, Decimal? valor1, Decimal? valor2, Int32? situacao, out List<PRECATORIO> objeto);

        List<TRF> GetAllTRF();
        List<BENEFICIARIO> GetAllBeneficiarios();
        List<HONORARIO> GetAllAdvogados();
        List<BANCO> GetAllBancos();
        List<NATUREZA> GetAllNaturezas();
        List<PRECATORIO_ESTADO> GetAllEstados();
        NATUREZA CheckExistNatureza(String natureza);
        CONTATO CheckExistContato(String prot);

        PRECATORIO_ANEXO GetAnexoById(Int32 id);
        PRECATORIO_ANOTACAO GetComentarioById(Int32 id);
        Int32 ValidateCreateFalha(PRECATORIO_FALHA item);
        Int32 ValidateCreateFalhaContato(CONTATO_FALHA item);

        List<VOLTA_PESQUISA> PesquisarTudo(String parm, Int32 idAss);

    }
}
