using System;
using System.IO;
using System.Net;
using System.Windows.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AdRotatorXNA.Helpers
{
    class ImageDownload
    {

        internal static void GetImageFromURL(GraphicsDevice graphics, string imageUrl, Action<Texture2D, Exception> callback)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                callback(null, new Exception());
                return;
            }
            Uri ImageURI = new Uri(imageUrl);
            WebClient client = new WebClient();
            try
            {
                client.AllowReadStreamBuffering = true;
                //Stream stream = 
                client.OpenReadCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        var errorimage = new Texture2D(graphics, 1, 1);
                        errorimage.SetData<Color>(new Color[1] { Color.Gray });
                        callback(errorimage, e.Error);
                    }
                    else
                    {
                        StreamResourceInfo sri = new StreamResourceInfo(e.Result, null);

                        var image = Texture2D.FromStream(graphics, sri.Stream);

                        callback(image, null);
                    }
                };
                client.OpenReadAsync(ImageURI);
            }
            catch (Exception ex)
            {
                callback(null, ex);
            }
            finally
            {
                client = null;
            }
        }
    }
}
