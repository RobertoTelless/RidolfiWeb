using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution;
using CRMPresentation.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text;
using System.Net;
using CrossCutting;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using log4net.Config;
using log4net.Core;

namespace ERP_Condominios_Solution.Classes
{
    public class GravaLogExcecao
    {
        private readonly IUsuarioAppService usuApp;

        public GravaLogExcecao(IUsuarioAppService usuApps)
        {
            usuApp = usuApps;
        }

        public Int32 GravarLogExcecao(Exception ex, String origem, String aplicacao, Int32 tipo, USUARIO usuario)
        {
            // Monta log
            LOG_EXCECAO_NOVO log = new LOG_EXCECAO_NOVO();
            log.ASSI_CD_ID = 1;
            log.USUA_CD_ID = usuario.USUA_CD_ID;
            log.LOEX_NM_APLICACAO = aplicacao;
            log.LOEX_DT_DATA = DateTime.Now;
            log.LOEX_NM_GERADOR = origem;
            log.LOEX_NM_TIPO_EXCECAO = ex.GetType().ToString();
            log.LOEX_DS_MENSAGEM = ex.Message;
            log.LOEX_DS_STACK_TRACE = ex.StackTrace;
            log.LOEX_DS_SOURCE = ex.Source;
            if (ex.InnerException != null)
            {
                log.LOEX_DS_INNER = ex.InnerException.Message;
                log.LOEX_NM_INNER_TIPO_EXCECAO = ex.InnerException.GetType().ToString();
            }
            log.LOEX_IN_TIPO_REGISTRO = tipo;

            Int32 volta = usuApp.ValidateCreateLogExcecao(log);
            return 0;
        }

    }
}