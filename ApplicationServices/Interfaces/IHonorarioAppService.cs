using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IHonorarioAppService : IAppServiceBase<HONORARIO>
    {
        Int32 ValidateCreate(HONORARIO perfil, USUARIO usuario);
        Int32 ValidateEdit(HONORARIO perfil, HONORARIO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(HONORARIO item, HONORARIO itemAntes);
        Int32 ValidateDelete(HONORARIO perfil, USUARIO usuario);
        Int32 ValidateReativar(HONORARIO perfil, USUARIO usuario);

        HONORARIO CheckExist(HONORARIO conta);
        HONORARIO CheckExist(String nome, String cpf);
        HONORARIO GetItemById(Int32 id);
        List<HONORARIO> GetAllItens();
        List<HONORARIO> GetAllItensAdm();
        Int32 ExecuteFilter(Int32? tipo, String cpf, String cnpj, String razao, String nome, out List<HONORARIO> objeto);
        Tuple<Int32, List<HONORARIO>, Boolean> ExecuteFilterTuple(Int32? tipo, String cpf, String cnpj, String razao, String nome, Int32 idAss);

        List<TIPO_PESSOA> GetAllTiposPessoa();

        HONORARIO_ANEXO GetAnexoById(Int32 id);
        HONORARIO_ANOTACOES GetComentarioById(Int32 id);
    }
}
