using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVaraRepository : IRepositoryBase<VARA>
    {
        VARA CheckExist(VARA conta);
        VARA GetItemById(Int32 id);
        List<VARA> GetAllItens();
        List<VARA> GetAllItensAdm();
        List<VARA> ExecuteFilter(String nome, Int32? trf);
    }
}

