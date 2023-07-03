using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaClienteRepository : IRepositoryBase<CATEGORIA_CLIENTE>
    {
        CATEGORIA_CLIENTE CheckExist(CATEGORIA_CLIENTE item, Int32 idAss);
        List<CATEGORIA_CLIENTE> GetAllItens(Int32 idAss);
        CATEGORIA_CLIENTE GetItemById(Int32 id);
        List<CATEGORIA_CLIENTE> GetAllItensAdm(Int32 idAss);

    }
}
