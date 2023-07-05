using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using CrossCutting;

public class CaptchaUtility
{

    public byte[] GetCaptchaImage(String checkCode)
    {
        if (checkCode == null || checkCode == String.Empty )
        {
            checkCode = CrossCutting.Cryptography.GenerateRandomPasswordNumero(6);
        }
        Bitmap image = new Bitmap(Convert.ToInt32(Math.Ceiling((decimal)(checkCode.Length * 20))), 45);
        Graphics g = Graphics.FromImage(image);
        try
        {
            Random random = new Random();
            g.Clear(Color.Beige);
            Font font = new Font("Comic Sans MS", 20, FontStyle.Bold);
            string str = "";
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Green, Color.DarkRed, 1.2f, true);
            for (int i = 0; i < checkCode.Length; i++)
            {
                str = str + checkCode.Substring(i, 1);
            }
            g.DrawString(str, font, new SolidBrush(Color.Green), 0, 0);
            g.Flush();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        finally
        {
            g.Dispose();
            image.Dispose();
        }
    }

    public byte[] VerificationTextGenerator()
    {
        String randomCode = CrossCutting.Cryptography.GenerateRandomPasswordNumero(4);
        HttpContext.Current.Session["Captcha"] = randomCode;
        return GetCaptchaImage(randomCode);
    }
}
