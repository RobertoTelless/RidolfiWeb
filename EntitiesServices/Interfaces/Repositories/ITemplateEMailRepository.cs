using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITemplateEMailRepository : IRepositoryBase<TEMPLATE_EMAIL>
    {
        TEMPLATE_EMAIL GetByCode(String code, Int32 idAss);
        List<TEMPLATE_EMAIL> GetAllItens(Int32 idAss);
        TEMPLATE_EMAIL GetItemById(Int32 id);
        List<TEMPLATE_EMAIL> GetAllItensAdm(Int32 idAss);
        List<TEMPLATE_EMAIL> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss);
        TEMPLATE_EMAIL CheckExist(TEMPLATE_EMAIL item, Int32 idAss);
    }
}
