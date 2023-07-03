using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPlataformaEntregaAppService : IAppServiceBase<PLATAFORMA_ENTREGA>
    {
        Int32 ValidateCreate(PLATAFORMA_ENTREGA item, USUARIO usuario);
        Int32 ValidateEdit(PLATAFORMA_ENTREGA item, PLATAFORMA_ENTREGA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(PLATAFORMA_ENTREGA item, PLATAFORMA_ENTREGA itemAntes);
        Int32 ValidateDelete(PLATAFORMA_ENTREGA item, USUARIO usuario);
        Int32 ValidateReativar(PLATAFORMA_ENTREGA item, USUARIO usuario);

        PLATAFORMA_ENTREGA CheckExist(PLATAFORMA_ENTREGA item, Int32 idAss);
        PLATAFORMA_ENTREGA GetItemById(Int32 id);
        List<PLATAFORMA_ENTREGA> GetAllItens(Int32 idAss);
        List<PLATAFORMA_ENTREGA> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<PLATAFORMA_ENTREGA>, Boolean> ExecuteFilterTuple(String nome, Int32 idAss);
    }
}
