using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFilialAppService : IAppServiceBase<FILIAL>
    {
        Int32 ValidateCreate(FILIAL perfil, USUARIO usuario);
        Int32 ValidateEdit(FILIAL perfil, FILIAL perfilAntes, USUARIO usuario);
        Int32 ValidateDelete(FILIAL perfil, USUARIO usuario);
        Int32 ValidateReativar(FILIAL perfil, USUARIO usuario);

        List<FILIAL> GetAllItens(Int32 idAss);
        List<FILIAL> GetAllItensAdm(Int32 idAss);
        FILIAL GetItemById(Int32 id);
        FILIAL CheckExist(FILIAL filial, Int32 idAss);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        List<TIPO_PESSOA> GetAllTiposPessoa();

    }
}
