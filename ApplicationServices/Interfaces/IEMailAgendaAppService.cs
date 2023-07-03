using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IEMailAgendaAppService : IAppServiceBase<EMAIL_AGENDAMENTO>
    {
        Int32 ValidateCreate(EMAIL_AGENDAMENTO item);
        Int32 ValidateEdit(EMAIL_AGENDAMENTO item);
        List<EMAIL_AGENDAMENTO> GetAllItens(Int32 idAss);
    }
}
