using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMunicipioRepository : IRepositoryBase<MUNICIPIO>
    {
        MUNICIPIO CheckExist(MUNICIPIO item);
        List<MUNICIPIO> GetAllItens();
        MUNICIPIO GetItemById(Int32 id);
        List<MUNICIPIO> GetMunicipioByUF(Int32 uf);
    }
}
