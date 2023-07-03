using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMaquinaAppService : IAppServiceBase<MAQUINA>
    {
        Int32 ValidateCreate(MAQUINA item, USUARIO usuario);
        Int32 ValidateEdit(MAQUINA item, MAQUINA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(MAQUINA item, MAQUINA itemAntes);
        Int32 ValidateDelete(MAQUINA item, USUARIO usuario);
        Int32 ValidateReativar(MAQUINA item, USUARIO usuario);

        MAQUINA CheckExist(MAQUINA item, Int32 idAss);
        MAQUINA GetItemById(Int32 id);
        List<MAQUINA> GetAllItens(Int32 idAss);
        List<MAQUINA> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<MAQUINA>, Boolean> ExecuteFilterTuple(String provedor, String nome, Int32? idAss);

    }
}
