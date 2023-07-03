using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IHonorarioService : IServiceBase<HONORARIO>
    {
        Int32 Create(HONORARIO perfil, LOG log);
        Int32 Create(HONORARIO perfil);
        Int32 Edit(HONORARIO perfil, LOG log);
        Int32 Edit(HONORARIO perfil);
        Int32 Delete(HONORARIO perfil, LOG log);

        HONORARIO CheckExist(HONORARIO conta);
        HONORARIO CheckExist(String nome, String cpf);
        HONORARIO GetItemById(Int32 id);
        List<HONORARIO> GetAllItens();
        List<HONORARIO> GetAllItensAdm();
        List<HONORARIO> ExecuteFilter(Int32? tipo, String cpf, String cnpj, String razao, String nome);

        List<TIPO_PESSOA> GetAllTiposPessoa();

        HONORARIO_ANEXO GetAnexoById(Int32 id);
        HONORARIO_ANOTACOES GetComentarioById(Int32 id);
    }
}
