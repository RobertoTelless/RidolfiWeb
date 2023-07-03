using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoTelefoneBaseRepository : IRepositoryBase<TIPO_TELEFONE_BASE>
    {
        List<TIPO_TELEFONE_BASE> GetAllItens();
        TIPO_TELEFONE_BASE GetItemById(Int32 id);
    }
}
