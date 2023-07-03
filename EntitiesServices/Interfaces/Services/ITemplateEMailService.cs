using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITemplateEMailService : IServiceBase<TEMPLATE_EMAIL>
    {
        Int32 Create(TEMPLATE_EMAIL perfil, LOG log);
        Int32 Create(TEMPLATE_EMAIL perfil);
        Int32 Edit(TEMPLATE_EMAIL perfil, LOG log);
        Int32 Edit(TEMPLATE_EMAIL perfil);
        Int32 Delete(TEMPLATE_EMAIL perfil, LOG log);

        TEMPLATE_EMAIL GetByCode(String code, Int32 idAss);
        List<TEMPLATE_EMAIL> GetAllItens(Int32 idAss);
        TEMPLATE_EMAIL GetItemById(Int32 id);
        List<TEMPLATE_EMAIL> GetAllItensAdm(Int32 idAss);
        List<TEMPLATE_EMAIL> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss);
        TEMPLATE_EMAIL CheckExist(TEMPLATE_EMAIL item, Int32 idAss);
    }
}
