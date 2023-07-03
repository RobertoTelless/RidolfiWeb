using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITemplateSMSService : IServiceBase<TEMPLATE_SMS>
    {
        Int32 Create(TEMPLATE_SMS perfil, LOG log);
        Int32 Create(TEMPLATE_SMS perfil);
        Int32 Edit(TEMPLATE_SMS perfil, LOG log);
        Int32 Edit(TEMPLATE_SMS perfil);
        Int32 Delete(TEMPLATE_SMS perfil, LOG log);

        TEMPLATE_SMS GetByCode(String code, Int32 idAss);
        List<TEMPLATE_SMS> GetAllItens(Int32 idAss);
        TEMPLATE_SMS GetItemById(Int32 id);
        List<TEMPLATE_SMS> GetAllItensAdm(Int32 idAss);
        List<TEMPLATE_SMS> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss);
        TEMPLATE_SMS CheckExist(TEMPLATE_SMS item, Int32 idAss);
    }
}
