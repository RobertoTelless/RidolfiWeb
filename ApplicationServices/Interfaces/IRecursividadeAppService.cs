using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IRecursividadeAppService : IAppServiceBase<RECURSIVIDADE>
    {
        Int32 ValidateCreate(RECURSIVIDADE perfil, USUARIO usuario);
        Int32 ValidateEdit(RECURSIVIDADE perfil, RECURSIVIDADE perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(RECURSIVIDADE item, RECURSIVIDADE itemAntes);
        Int32 ValidateDelete(RECURSIVIDADE perfil, USUARIO usuario);
        Int32 ValidateReativar(RECURSIVIDADE perfil, USUARIO usuario);

        List<RECURSIVIDADE> GetAllItens(Int32 idAss);
        List<RECURSIVIDADE> GetAllItensAdm(Int32 idAss);
        RECURSIVIDADE GetItemById(Int32 id);
        RECURSIVIDADE CheckExist(RECURSIVIDADE conta, Int32 idAss);
        Tuple<Int32, List<RECURSIVIDADE>, Boolean> ExecuteFilter(Int32? tipoMensagem, String nome, DateTime? dataInico, DateTime? dataFim, String texto, Int32 idAss);

        RECURSIVIDADE_DESTINO GetDestinoById(Int32 id);
        RECURSIVIDADE_DATA GetDataById(Int32 id);

        Int32 ValidateCreateDestino(RECURSIVIDADE_DESTINO item);
        Int32 ValidateCreateData(RECURSIVIDADE_DATA item);
    }
}
