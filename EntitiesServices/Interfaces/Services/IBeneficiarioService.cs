using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IBeneficiarioService : IServiceBase<BENEFICIARIO>
    {
        Int32 Create(BENEFICIARIO perfil, LOG log);
        Int32 Create(BENEFICIARIO perfil);
        Int32 Edit(BENEFICIARIO perfil, LOG log);
        Int32 Edit(BENEFICIARIO perfil);
        Int32 Delete(BENEFICIARIO perfil, LOG log);

        BENEFICIARIO CheckExist(BENEFICIARIO conta);
        BENEFICIARIO CheckExist(String nome, String cpf);
        BENEFICIARIO GetItemById(Int32 id);
        List<BENEFICIARIO> GetAllItens();
        List<BENEFICIARIO> GetAllItensAdm();
        List<BENEFICIARIO> ExecuteFilter(Int32? tipo, Int32? sexo, Int32? estado, Int32? escolaridade, Int32? parentesco, String razao, String nome, DateTime? dataNasc, String cpf, String cnpj, String parente);

        List<ESCOLARIDADE> GetAllEscolaridade();
        List<PARENTESCO> GetAllParentesco();
        List<TIPO_PESSOA> GetAllTiposPessoa();
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        List<SEXO> GetAllSexo();
        List<ESTADO_CIVIL> GetAllEstadoCivil();
        List<TIPO_TELEFONE_BASE> GetAllTipoTelefone();

        BENEFICIARIO_ANEXO GetAnexoById(Int32 id);
        BENEFICIARIO_ANOTACOES GetComentarioById(Int32 id);

        CONTATO GetContatoById(Int32 id);
        Int32 EditContato(CONTATO item);
        Int32 CreateContato(CONTATO item);
        Int32 EditAnotacao(BENEFICIARIO_ANOTACOES item);

        ENDERECO GetEnderecoById(Int32 id);
        Int32 EditEndereco(ENDERECO item);
        Int32 CreateEndereco(ENDERECO item);

    }
}
