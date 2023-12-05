using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IClienteService : IServiceBase<CLIENTE>
    {
        Int32 Create(CLIENTE perfil, LOG log);
        Int32 Create(CLIENTE perfil);
        Int32 Edit(CLIENTE perfil, LOG log);
        Int32 Edit(CLIENTE perfil);
        Int32 Delete(CLIENTE perfil, LOG log);

        CLIENTE CheckExist(CLIENTE conta, Int32 idAss);
        CLIENTE CheckExistDoctos(String cpf, String cnpj, Int32 idAss);
        CLIENTE GetItemById(Int32 id);
        CLIENTE GetByEmail(String email);
        List<CLIENTE> GetAllItens(Int32 idAss);
        List<CLIENTE> GetAllItensAdm(Int32 idAss);
        List<CLIENTE> GetAllItensUltimos(Int32 num, Int32 idAss);
        List<CATEGORIA_CLIENTE> GetAllTipos(Int32 idAss);
        List<TIPO_PESSOA> GetAllTiposPessoa();
        List<TIPO_CONTRIBUINTE> GetAllContribuinte(Int32 idAss);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        List<REGIME_TRIBUTARIO> GetAllRegimes(Int32 idAss);
        List<SEXO> GetAllSexo();
        List<CLIENTE_FALHA> GetAllFalhas(Int32 idAss);
        List<LINGUA> GetAllLinguas();
        List<NACIONALIDADE> GetAllNacionalidades();
        List<MUNICIPIO> GetAllMunicipios();
        List<MUNICIPIO> GetMunicipioByUF(Int32 uf);
        MUNICIPIO CheckExistMunicipio(MUNICIPIO conta);

        CLIENTE_ANEXO GetAnexoById(Int32 id);
        CLIENTE_CONTATO GetContatoById(Int32 id);
        CLIENTE_ANOTACAO GetAnotacaoById(Int32 id);
        CLIENTE_REFERENCIA GetReferenciaById(Int32 id);
        List<CLIENTE> ExecuteFilter(Int32? id, Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32? filial, Int32? usu, Int32 idAss);
        CLIENTE_ANOTACAO GetComentarioById(Int32 id);
        GRUPO_CLIENTE GetGrupoById(Int32 id);

        Int32 CreateMunicipio(MUNICIPIO item);

        Int32 EditAnotacao(CLIENTE_ANOTACAO item);
        Int32 EditContato(CLIENTE_CONTATO item);
        Int32 CreateContato(CLIENTE_CONTATO item);
        Int32 EditReferencia(CLIENTE_REFERENCIA item);
        Int32 CreateReferencia(CLIENTE_REFERENCIA item);
        Int32 EditFalha(CLIENTE_FALHA item);
        Int32 CreateFalha(CLIENTE_FALHA item);
        Int32 CreateGrupo(GRUPO_CLIENTE item);
        Int32 EditGrupo(GRUPO_CLIENTE item);
    }
}
