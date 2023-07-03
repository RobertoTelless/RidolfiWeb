using System;
using System.Web;
using System.IO;
using System.Web.SessionState;
using System.Drawing;
using System.Drawing.Imaging;

namespace Presentation.Handlers
{
    /// <summary>
    /// Descrição resumida de Captcha
    /// </summary>
    public class Captcha : IHttpHandler, IReadOnlySessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            MemoryStream memStream = new MemoryStream();
            string phrase = Convert.ToString(context.Session["Captcha"]);

            // Gera imagem a partir do texto
            Bitmap CaptchaImg = new Bitmap(180, 60);
            Graphics Graphic = Graphics.FromImage(CaptchaImg);
            Graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Ajusta altura e largura
            Graphic.FillRectangle(new SolidBrush(Color.Blue), 0, 0, 180, 60);
            Graphic.DrawString(phrase, new Font("Calibri", 30), new SolidBrush(Color.White), 15, 15);
            CaptchaImg.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imgBytes = memStream.GetBuffer();

            Graphic.Dispose();
            CaptchaImg.Dispose();
            memStream.Close();

            // Exibe imagem
            context.Response.ContentType = "image/jpeg";
            context.Response.BinaryWrite(imgBytes);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}