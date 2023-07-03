using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IQuemDesligouRepository : IRepositoryBase<QUEM_DESLIGOU>
    {
        QUEM_DESLIGOU CheckExist(String quem);
        List<QUEM_DESLIGOU> GetAllItens();
        QUEM_DESLIGOU GetItemById(Int32 id);
        List<QUEM_DESLIGOU> GetAllItensAdm();
    }
}
