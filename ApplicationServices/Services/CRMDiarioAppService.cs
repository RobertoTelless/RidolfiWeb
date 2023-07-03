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
    public class CRMDiarioAppService : AppServiceBase<DIARIO_PROCESSO>, ICRMDiarioAppService
    {
        private readonly ICRMDiarioService _baseService;
        private readonly IConfiguracaoService _confService;

        public CRMDiarioAppService(ICRMDiarioService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<DIARIO_PROCESSO> GetAllItens(Int32 idAss)
        {
            List<DIARIO_PROCESSO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            List<USUARIO> lista = _baseService.GetAllUsuarios(idAss);
            return lista;
        }

        public DIARIO_PROCESSO GetItemById(Int32 id)
        {
            DIARIO_PROCESSO item = _baseService.GetItemById(id);
            return item;
        }

        public DIARIO_PROCESSO GetByDate(DateTime data)
        {
            DIARIO_PROCESSO item = _baseService.GetByDate(data);
            return item;
        }

        public Int32 ExecuteFilter(Int32? processo, DateTime? inicio, DateTime? final, Int32? usuario, String operacao, String descricao, Int32 idAss, out List<DIARIO_PROCESSO> objeto)
        {
            try
            {
                objeto = new List<DIARIO_PROCESSO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(processo, inicio, final, usuario, operacao, descricao, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(DIARIO_PROCESSO item)
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
