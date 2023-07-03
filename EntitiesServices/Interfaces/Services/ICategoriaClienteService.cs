using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaClienteService : IServiceBase<CATEGORIA_CLIENTE>
    {
        Int32 Create(CATEGORIA_CLIENTE perfil, LOG log);
        Int32 Create(CATEGORIA_CLIENTE perfil);
        Int32 Edit(CATEGORIA_CLIENTE perfil, LOG log);
        Int32 Edit(CATEGORIA_CLIENTE perfil);
        Int32 Delete(CATEGORIA_CLIENTE perfil, LOG log);

        CATEGORIA_CLIENTE CheckExist(CATEGORIA_CLIENTE item, Int32 idAss);
        CATEGORIA_CLIENTE GetItemById(Int32 id);
        List<CATEGORIA_CLIENTE> GetAllItens(Int32 idAss);
        List<CATEGORIA_CLIENTE> GetAllItensAdm(Int32 idAss);

    }
}
