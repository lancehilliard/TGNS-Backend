using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Portal.Models
{
    public class PenpointViewModel
    {
        public IPenpointEditData EditData { get; set; }
        public bool UserIsOwner { get; set; }
    }

    public interface IPenpointEditData
    {
        string Id { get; }
        long PlayerId { get; }
        string ImageUrl { get; }
        string SketchJson { get; }
    }

    public class PenpointEditData : IPenpointEditData
    {
        public PenpointEditData(string id, long playerId, string imageUrl, string sketchJson)
        {
            Id = id;
            PlayerId = playerId;
            ImageUrl = imageUrl;
            SketchJson = sketchJson;
        }

        public string Id { get; private set; }
        public long PlayerId { get; private set; }
        public string ImageUrl { get; private set; }
        public string SketchJson { get; private set; }
    }
}