using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFunilService : IServiceBase<FUNIL>
    {
        Int32 Create(FUNIL perfil, LOG log);
        Int32 Create(FUNIL perfil);
        Int32 Edit(FUNIL perfil, LOG log);
        Int32 Edit(FUNIL perfil);
        Int32 Delete(FUNIL perfil, LOG log);

        FUNIL CheckExist(FUNIL conta, Int32 idAss);
        FUNIL GetItemById(Int32 id);
        FUNIL GetItemBySigla(String sigla, Int32 idAss);
        List<FUNIL> GetAllItens(Int32 idAss);
        List<FUNIL> GetAllItensAdm(Int32 idAss);

        FUNIL_ETAPA GetEtapaById(Int32 id);

        Int32 EditEtapa(FUNIL_ETAPA item);
        Int32 CreateEtapa(FUNIL_ETAPA item);
    }
}
