using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IDepartamentoRepository : IRepositoryBase<DEPARTAMENTO>
    {
        DEPARTAMENTO CheckExist(DEPARTAMENTO item, Int32 idAss);
        List<DEPARTAMENTO> GetAllItens(Int32 idAss);
        DEPARTAMENTO GetItemById(Int32 id);
        List<DEPARTAMENTO> GetAllItensAdm(Int32 idAss);
    }
}
