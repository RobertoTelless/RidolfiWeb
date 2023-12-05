using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEstadoRepository : IRepositoryBase<ESTADO>
    {
        ESTADO GetItemById(Int32 id);
        List<ESTADO> GetAllItens(Int32 idAss);
    }
}
