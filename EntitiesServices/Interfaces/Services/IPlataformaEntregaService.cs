using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPlataformaEntregaService : IServiceBase<PLATAFORMA_ENTREGA>
    {
        Int32 Create(PLATAFORMA_ENTREGA perfil, LOG log);
        Int32 Create(PLATAFORMA_ENTREGA perfil);
        Int32 Edit(PLATAFORMA_ENTREGA perfil, LOG log);
        Int32 Edit(PLATAFORMA_ENTREGA perfil);
        Int32 Delete(PLATAFORMA_ENTREGA perfil, LOG log);

        PLATAFORMA_ENTREGA CheckExist(PLATAFORMA_ENTREGA item, Int32 idAss);
        PLATAFORMA_ENTREGA GetItemById(Int32 id);
        List<PLATAFORMA_ENTREGA> GetAllItens(Int32 idAss);
        List<PLATAFORMA_ENTREGA> GetAllItensAdm(Int32 idAss);
        List<PLATAFORMA_ENTREGA> ExecuteFilter(String nome, Int32 idAss);

    }
}
