using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IClienteAppService : IAppServiceBase<CLIENTE>
    {
        Int32 ValidateCreate(CLIENTE perfil, USUARIO usuario);
        Int32 ValidateEdit(CLIENTE perfil, CLIENTE perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(CLIENTE item, CLIENTE itemAntes);
        Int32 ValidateDelete(CLIENTE perfil, USUARIO usuario);
        Int32 ValidateReativar(CLIENTE perfil, USUARIO usuario);

        List<CLIENTE> GetAllItens(Int32 idAss);
        List<CLIENTE> GetAllItensAdm(Int32 idAss);
        List<CLIENTE> GetAllItensUltimos(Int32 num, Int32 idAss);
        CLIENTE GetItemById(Int32 id);
        CLIENTE GetByEmail(String email);
        CLIENTE CheckExist(CLIENTE conta, Int32 idAss);
        CLIENTE CheckExistDoctos(String cpf, String cnpj, Int32 idAss);

        List<CATEGORIA_CLIENTE> GetAllTipos(Int32 idAss);
        List<TIPO_PESSOA> GetAllTiposPessoa();
        CLIENTE_ANEXO GetAnexoById(Int32 id);
        CLIENTE_CONTATO GetContatoById(Int32 id);
        CLIENTE_ANOTACAO GetAnotacaoById(Int32 id);
        CLIENTE_REFERENCIA GetReferenciaById(Int32 id);
        List<TIPO_CONTRIBUINTE> GetAllContribuinte(Int32 idAss);
        List<REGIME_TRIBUTARIO> GetAllRegimes(Int32 idAss);
        List<SEXO> GetAllSexo();
        CLIENTE_ANOTACAO GetComentarioById(Int32 id);
        List<CLIENTE_FALHA> GetAllFalhas(Int32 idAss);
        GRUPO_CLIENTE GetGrupoById(Int32 id);

        //Int32 ExecuteFilter(Int32? id,  Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32 idAss, out List<CLIENTE> objeto);
        Tuple < Int32, List<CLIENTE>, Boolean > ExecuteFilterTuple(Int32? id, Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32? filial, Int32 idAss);

        Int32 ValidateEditAnotacao(CLIENTE_ANOTACAO item);
        Int32 ValidateEditContato(CLIENTE_CONTATO item);
        Int32 ValidateCreateContato(CLIENTE_CONTATO item);
        Int32 ValidateEditReferencia(CLIENTE_REFERENCIA item);
        Int32 ValidateCreateReferencia(CLIENTE_REFERENCIA item);
        Int32 ValidateEditFalha(CLIENTE_FALHA item);
        Int32 ValidateCreateFalha(CLIENTE_FALHA item);
        Int32 ValidateCreateGrupo(GRUPO_CLIENTE item);
        Int32 ValidateEditGrupo(GRUPO_CLIENTE item);

        List<VOLTA_PESQUISA> PesquisarTudo(String parm, Int32[] permissoes, String perfil, Int32? empresa, Int32 idAss);
        Int32 AtualizarCategoriaClienteCalculo(Int32 idAss, CLIENTE cli);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

        List<LINGUA> GetAllLinguas();
        List<NACIONALIDADE> GetAllNacionalidades();
        List<MUNICIPIO> GetAllMunicipios();
        List<MUNICIPIO> GetMunicipioByUF(Int32 uf);
        Int32 ValidateCreateMunicipio(MUNICIPIO item);
        MUNICIPIO CheckExistMunicipio(MUNICIPIO conta);

    }
}
