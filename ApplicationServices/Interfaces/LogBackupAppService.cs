using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class LogBackupAppService : AppServiceBase<LOG_BACKUP>, ILogBackupAppService
    {
        private readonly ILogBackupService _baseService;

        public LogBackupAppService(ILogBackupService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public LOG_BACKUP GetById(Int32 id)
        {
            return _baseService.GetById(id);
        }

        public List<LOG_BACKUP> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public List<LOG_BACKUP> GetAllItensUsuario(Int32 id, Int32 idAss)
        {
            return _baseService.GetAllItensUsuario(id, idAss);
        }

        public Tuple<Int32, List<LOG_BACKUP>, Boolean> ExecuteFilterTuple(Int32? usuId, DateTime? data, String operacao, Int32 idAss)
        {
            try
            {
                List<LOG_BACKUP> objeto = new List<LOG_BACKUP>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(usuId, data, operacao, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(LOG_BACKUP item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
