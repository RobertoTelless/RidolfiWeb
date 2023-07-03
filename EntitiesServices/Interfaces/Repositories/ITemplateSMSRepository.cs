using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITemplateSMSRepository : IRepositoryBase<TEMPLATE_SMS>
    {
        TEMPLATE_SMS GetByCode(String code, Int32 idAss);
        List<TEMPLATE_SMS> GetAllItens(Int32 idAss);
        TEMPLATE_SMS GetItemById(Int32 id);
        List<TEMPLATE_SMS> GetAllItensAdm(Int32 idAss);
        List<TEMPLATE_SMS> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss);
        TEMPLATE_SMS CheckExist(TEMPLATE_SMS item, Int32 idAss);
    }
}
