using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IRecursividadeService : IServiceBase<RECURSIVIDADE>
    {
        Int32 Create(RECURSIVIDADE perfil, LOG log);
        Int32 Create(RECURSIVIDADE perfil);
        Int32 Edit(RECURSIVIDADE perfil, LOG log);
        Int32 Edit(RECURSIVIDADE perfil);
        Int32 Delete(RECURSIVIDADE perfil, LOG log);

        RECURSIVIDADE CheckExist(RECURSIVIDADE conta, Int32 idAss);
        RECURSIVIDADE GetItemById(Int32 id);
        List<RECURSIVIDADE> GetAllItens(Int32 idAss);
        List<RECURSIVIDADE> GetAllItensAdm(Int32 idAss);
        List<RECURSIVIDADE> ExecuteFilter(Int32? tipoMensagem, String nome, DateTime? dataInico, DateTime? dataFim, String texto, Int32 idAss);

        RECURSIVIDADE_DESTINO GetDestinoById(Int32 id);
        RECURSIVIDADE_DATA GetDataById(Int32 id);

        Int32 CreateDestino(RECURSIVIDADE_DESTINO item);
        Int32 CreateData(RECURSIVIDADE_DATA item);
    }
}
