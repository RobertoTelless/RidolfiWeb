using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPrecatorioEstadoRepository : IRepositoryBase<PRECATORIO_ESTADO>
    {
        List<PRECATORIO_ESTADO> GetAllItens();
        PRECATORIO_ESTADO GetItemById(Int32 id);
        List<PRECATORIO_ESTADO> GetAllItensAdm();
        PRECATORIO_ESTADO CheckExist(PRECATORIO_ESTADO prec);
    }
}
