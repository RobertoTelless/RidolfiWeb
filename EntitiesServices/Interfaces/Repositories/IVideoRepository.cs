using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVideoRepository : IRepositoryBase<VIDEO>
    {
        VIDEO GetItemById(Int32 id);
        List<VIDEO> GetAllItens(Int32 idAss);
        List<VIDEO> GetAllItensAdm(Int32 idAss);
        List<VIDEO> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link, Int32 idAss);
        List<VIDEO> GetAllItensValidos(Int32 idAss);
    }
}
