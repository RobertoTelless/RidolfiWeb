using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFunilAppService : IAppServiceBase<FUNIL>
    {
        Int32 ValidateCreate(FUNIL perfil, USUARIO usuario);
        Int32 ValidateEdit(FUNIL perfil, FUNIL perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(FUNIL item, FUNIL itemAntes);
        Int32 ValidateDelete(FUNIL perfil, USUARIO usuario);
        Int32 ValidateReativar(FUNIL perfil, USUARIO usuario);

        List<FUNIL> GetAllItens(Int32 idAss);
        List<FUNIL> GetAllItensAdm(Int32 idAss);
        FUNIL GetItemById(Int32 id);
        FUNIL GetItemBySigla(String sigla, Int32 idAss);
        FUNIL CheckExist(FUNIL conta, Int32 idAss);

        FUNIL_ETAPA GetEtapaById(Int32 id);
        

        Int32 ValidateEditEtapa(FUNIL_ETAPA item);
        Int32 ValidateCreateEtapa(FUNIL_ETAPA item);
    }
}
