using antlr;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using Spire.Pdf.HtmlConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CRMPresentation.Controllers
{
    public class ControleCache : Controller
    {
        private readonly IClienteAppService _cliApp;
        private readonly IGrupoAppService _gruApp;
        private readonly IMensagemAppService _baseApp;
        private readonly ITemplateSMSAppService _temApp;
        private readonly ITemplateEMailAppService _temaApp;
        private readonly IPeriodicidadeAppService _periodicidadeApp;

        public ControleCache(IMensagemAppService baseApp, ITemplateSMSAppService temApp, ITemplateEMailAppService temaApp, IGrupoAppService gruApps, IClienteAppService cliApps, IPeriodicidadeAppService periodicidadeApp)
        {
            _baseApp = baseApp;
            _gruApp = gruApps;
            _cliApp = cliApps;
            _temApp = temApp;
            _temaApp = temaApp;
            _periodicidadeApp = periodicidadeApp;
        }


        [NonAction]
        public List<CLIENTE> CarregaCliente()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CLIENTE> conf = new List<CLIENTE>();
            if (Session["Clientes"] == null)
            {
                conf = _cliApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["ClienteAlterada"] == 1)
                {
                    conf = _cliApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<CLIENTE>)Session["Clientes"];
                }
            }
            Session["Clientes"] = conf;
            Session["ClienteAlterada"] = 0;
            return conf;
        }

        [NonAction]
        public List<GRUPO> CarregaGrupo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<GRUPO> conf = new List<GRUPO>();
            if (Session["Grupos"] == null)
            {
                conf = _gruApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["GrupoAlterada"] == 1)
                {
                    conf = _gruApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<GRUPO>)Session["Grupos"];
                }
            }
            Session["Grupos"] = conf;
            Session["GrupoAlterada"] = 0;
            return conf;
        }

        [NonAction]
        public List<SelectListItem> CarregaModelosHtml()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            String caminho = "/TemplateEMail/Modelos/" + idAss.ToString() + "/";
            String path = Path.Combine(Server.MapPath(caminho));
            String[] files = Directory.GetFiles(path, "*.html");
            List<SelectListItem> mod = new List<SelectListItem>();
            foreach (String file in files)
            {
                mod.Add(new SelectListItem() { Text = System.IO.Path.GetFileNameWithoutExtension(file), Value = file });
            }

            return mod;
        }

        [NonAction]
        public List<SelectListItem> CarregaRepeticao()
        {
            List<SelectListItem> rep = new List<SelectListItem>();
            rep.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            rep.Add(new SelectListItem() { Text = "Não", Value = "0" });

            return rep;
        }

        [NonAction]
        public List<SelectListItem> CarregaPeriodicidade(USUARIO usuario)
        {
            List<SelectListItem> peri = new List<SelectListItem>();

            foreach (var item in _periodicidadeApp.GetByAssinante(usuario))
            {
                peri.Add(new SelectListItem() { Text = item.PETA_NM_NOME, Value = item.PETA_NR_DIAS.ToString() });
            }
            /*peri.Add(new SelectListItem() { Text = "Mesma Data (Anual)", Value = "1" });
            peri.Add(new SelectListItem() { Text = "Mensal", Value = "2" });
            peri.Add(new SelectListItem() { Text = "Semanal", Value = "3" });
            peri.Add(new SelectListItem() { Text = "Nunca", Value = "0" });*/


            return peri;
        }


        [NonAction]
        public List<MENSAGENS> CarregaMensagem()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> conf = new List<MENSAGENS>();

            conf = _baseApp.GetAllItens(idAss);

            /* if (Session["Mensagens"] == null)
             {
                 conf = _baseApp.GetAllItens(idAss);
             }
             else
             {
                 if ((Int32)Session["MensagemAlterada"] == 1)
                 {
                     conf = _baseApp.GetAllItens(idAss);
                 }
                 else
                 {
                     conf = (List<MENSAGENS>)Session["Mensagens"];
                 }
             }*/
            Session["MensagemAlterada"] = 0;
            Session["Mensagens"] = conf;
            return conf;
        }


        [NonAction]
        public List<TEMPLATE_SMS> CarregaTemplateSMS()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TEMPLATE_SMS> conf = new List<TEMPLATE_SMS>();
            if (Session["TemplatesSMS"] == null)
            {
                conf = _temApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["GrupoAlterada"] == 1)
                {
                    conf = _temApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<TEMPLATE_SMS>)Session["TemplatesSMS"];
                }
            }
            Session["TemplatesSMS"] = conf;
            Session["TemplateSMSAlterada"] = 0;
            return conf;
        }

        [NonAction]
        public List<TEMPLATE_EMAIL> CarregaTemplateEMail()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TEMPLATE_EMAIL> conf = new List<TEMPLATE_EMAIL>();
            if (Session["TemplatesEMail"] == null)
            {
                conf = _temaApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["GrupoAlterada"] == 1)
                {
                    conf = _temaApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<TEMPLATE_EMAIL>)Session["TemplatesEMail"];
                }
            }
            Session["TemplatesEMail"] = conf;
            Session["TemplateEMailAlterada"] = 0;
            return conf;
        }

        [NonAction]
        public USUARIO GetUsuarioLogado()
        {
           return (USUARIO)Session["UserCredentials"];
        }
    }
}