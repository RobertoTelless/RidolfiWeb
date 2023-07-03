using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPlataformaEntregaRepository : IRepositoryBase<PLATAFORMA_ENTREGA>
    {
        PLATAFORMA_ENTREGA CheckExist(PLATAFORMA_ENTREGA item, Int32 idAss);
        PLATAFORMA_ENTREGA GetItemById(Int32 id);
        List<PLATAFORMA_ENTREGA> GetAllItens(Int32 idAss);
        List<PLATAFORMA_ENTREGA> GetAllItensAdm(Int32 idAss);
        List<PLATAFORMA_ENTREGA> ExecuteFilter(String nome, Int32 idAss);
    }
}
