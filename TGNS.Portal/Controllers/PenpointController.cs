//penpoint to jpg

//http://stackoverflow.com/a/4093951/116895
//https://stackoverflow.com/questions/6765370/merge-image-using-javascript

//svg from raphael to canvas, then canvas2image to have transparent markup image, then put transparent markup image over base image, then create link to combined image

//http://ianli.com/sketchpad/
//http://raphaeljs.com/
//https://stackoverflow.com/questions/4086703/convert-raphael-svg-to-image-png-etc-client-side
//https://stackoverflow.com/questions/3975499/convert-svg-to-image-jpeg-png-etc-in-the-browser
//https://svgopen.org/2010/papers/62-From_SVG_to_Canvas_and_Back/index.html


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TGNS.Core.Data;
using TGNS.Core.Domain;
using TGNS.Portal.Models;

namespace TGNS.Portal.Controllers
{
    public class PenpointController : AuthenticatedController
    {
        private readonly IPenpointEditDataGetter _penpointEditDataGetter;
        private readonly IPenpointEditDataSetter _penpointEditDataSetter;

        public PenpointController()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Data"].ConnectionString;
            _penpointEditDataGetter = new PenpointEditDataGetter(connectionString);
            _penpointEditDataSetter = new PenpointEditDataSetter(connectionString);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var penpointViewModel = new PenpointViewModel();
            var editData = _penpointEditDataGetter.Get(id);
            penpointViewModel.EditData = editData;
            penpointViewModel.UserIsOwner = PlayerId == editData.PlayerId;
            return View(penpointViewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Save(string id, string imageUrl, string sketchJson)
        {
            var penpointEditData = _penpointEditDataGetter.Get(id);
            if (penpointEditData.PlayerId != PlayerId)
            {
                id = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace('+', '_').Replace('/', '-').Replace("=", "");
            }

            var message = string.Empty;
            var isError = false;
            var stacktrace = string.Empty;
            try
            {
                _penpointEditDataSetter.Set(id, PlayerId, imageUrl, sketchJson);
                message = "Penpoint saved successfully.";
            }
            catch (Exception ex)
            {
                isError = true;
                message = "Error saving Penpoint.";
                stacktrace = ex.StackTrace;
            }
            return Json(new { isError = isError, message = message, stacktrace = stacktrace, id = id });
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Clone(string id)
        {
            var penpointEditData = _penpointEditDataGetter.Get(id);
            var newId = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace('+', '_').Replace('/', '-').Replace("=", "");

            var message = string.Empty;
            var isError = false;
            var stacktrace = string.Empty;
            try
            {
                _penpointEditDataSetter.Set(newId, PlayerId, penpointEditData.ImageUrl, penpointEditData.SketchJson);
                message = "Penpoint cloned successfully.";
            }
            catch (Exception ex)
            {
                isError = true;
                message = "Error saving Penpoint.";
                stacktrace = ex.StackTrace;
            }
            return Json(new { isError = isError, message = message, stacktrace = stacktrace, id = newId });
        }

        public ActionResult GetImage(string url)
        {
            return ExternalGet(url);
        }

        /// <summary>
        /// Makes a GET request to a URL and returns the relayed result.
        /// </summary>
        private HttpWebResponseResult ExternalGet(string url)
        {
            var getRequest = (HttpWebRequest)WebRequest.Create(url);
            var getResponse = (HttpWebResponse)getRequest.GetResponse();

            return new HttpWebResponseResult(getResponse);
        }
    }

    /// <summary>
    /// Result for relaying an HttpWebResponse
    /// </summary>
    public class HttpWebResponseResult : ActionResult
    {
        private readonly HttpWebResponse _response;
        private readonly ActionResult _innerResult;

        /// <summary>
        /// Relays an HttpWebResponse as verbatim as possible.
        /// </summary>
        /// <param name="responseToRelay">The HTTP response to relay</param>
        public HttpWebResponseResult(HttpWebResponse responseToRelay)
        {
            if (responseToRelay == null)
            {
                throw new ArgumentNullException("responseToRelay");
            }

            _response = responseToRelay;

            Stream contentStream;
            if (responseToRelay.ContentEncoding.Contains("gzip"))
            {
                contentStream = new GZipStream(responseToRelay.GetResponseStream(), CompressionMode.Decompress);
            }
            else if (responseToRelay.ContentEncoding.Contains("deflate"))
            {
                contentStream = new DeflateStream(responseToRelay.GetResponseStream(), CompressionMode.Decompress);
            }
            else
            {
                contentStream = responseToRelay.GetResponseStream();
            }


            if (string.IsNullOrEmpty(responseToRelay.CharacterSet))
            {
                // File result
                _innerResult = new FileStreamResult(contentStream, responseToRelay.ContentType);
            }
            else
            {
                // Text result
                var contentResult = new ContentResult();
                contentResult = new ContentResult();
                contentResult.Content = new StreamReader(contentStream).ReadToEnd();
                _innerResult = contentResult;
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var clientResponse = context.HttpContext.Response;
            clientResponse.StatusCode = (int)_response.StatusCode;

            foreach (var headerKey in _response.Headers.AllKeys)
            {
                switch (headerKey)
                {
                    case "Content-Length":
                    case "Transfer-Encoding":
                    case "Content-Encoding":
                        // Handled by IIS
                        break;

                    default:
                        clientResponse.AddHeader(headerKey, _response.Headers[headerKey]);
                        break;
                }
            }

            _innerResult.ExecuteResult(context);
        }
    }

    public interface IPenpointEditDataGetter
    {
        IPenpointEditData Get(string id);
    }

    public interface IPenpointEditDataSetter
    {
        void Set(string id, long playerId, string imageUrl, string sketchJson);
    }

    public class PenpointEditDataSetter : DataAccessor, IPenpointEditDataSetter
    {
        public PenpointEditDataSetter(string connectionString) : base(connectionString)
        {
        }

        public void Set(string id, long playerId, string imageUrl, string sketchJson)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO penpoints (PenpointID, PenpointPlayerID, PenpointImageUrl, PenpointSketchJson) VALUES (@PenpointID, @PenpointPlayerID, @PenpointImageUrl, @PenpointSketchJson) ON DUPLICATE KEY UPDATE PenpointSketchJson=@PenpointSketchJson;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PenpointID", id);
                    command.Parameters.AddWithValue("@PenpointPlayerID", playerId);
                    command.Parameters.AddWithValue("@PenpointImageUrl", imageUrl);
                    command.Parameters.AddWithValue("@PenpointSketchJson", sketchJson);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public class PenpointEditDataGetter : DataAccessor, IPenpointEditDataGetter
    {
        public PenpointEditDataGetter(string connectionString) : base(connectionString)
        {
        }

        public IPenpointEditData Get(string id)
        {
            var playerId = default(long);
            var imageUrl = default(string);
            var sketchJson = default(string);
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT PenpointPlayerId, PenpointImageUrl, PenpointSketchJson FROM penpoints WHERE PenpointID = @PenpointID;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PenpointID", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            playerId = reader.GetInt64("PenpointPlayerId");
                            imageUrl = reader.GetString("PenpointImageUrl");
                            sketchJson = reader.GetString("PenpointSketchJson");
                        }
                    }
                }
            }
            sketchJson = string.IsNullOrWhiteSpace(sketchJson) ? "null" : sketchJson;
            var result = new PenpointEditData(id, playerId, imageUrl, sketchJson);
            return result;
        }
    }
}