using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoTelefoneRepository : IRepositoryBase<TIPO_TELEFONE>
    {
        List<TIPO_TELEFONE> GetAllItens();
        TIPO_TELEFONE GetItemById(Int32 id);
    }
}
