using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Work_Classes;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IGrupoAppService : IAppServiceBase<GRUPO>
    {
        Int32 ValidateCreate(GRUPO item, MontagemGrupo grupo, USUARIO usuario);
        Int32 ValidateEdit(GRUPO item, GRUPO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(GRUPO item, GRUPO itemAntes);
        Int32 ValidateDelete(GRUPO item, USUARIO usuario);
        Int32 ValidateReativar(GRUPO item, USUARIO usuario);
        Int32 ValidateRemontar(GRUPO item, MontagemGrupo grupo, USUARIO usuario);

        List<GRUPO> GetAllItens(Int32 idAss);
        List<GRUPO> GetAllItensAdm(Int32 idAss);
        GRUPO GetItemById(Int32 id);
        GRUPO CheckExist(GRUPO conta, Int32 idAss);

        GRUPO_CLIENTE GetContatoById(Int32 id);
        Int32 ValidateCreateContato(GRUPO_CLIENTE item);
        Int32 ValidateEditContato(GRUPO_CLIENTE item);
        GRUPO_CLIENTE CheckExistContato(GRUPO_CLIENTE conta);
    }
}
