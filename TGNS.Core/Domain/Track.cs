using System.Collections;
using System.Collections.Generic;

namespace TGNS.Core.Domain
{
    public interface ITrack
    {
        string Id { get; }
        string Name { get; }
        string MapName { get; }
        IEnumerable<string> LocationNames { get; }
    }

    public class Track : ITrack
    {
        public Track(string id, string name, string mapName, IEnumerable<string> locationNames)
        {
            Id = id;
            Name = name;
            MapName = mapName;
            LocationNames = locationNames;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string MapName { get; private set; }
        public IEnumerable<string> LocationNames { get; private set; }
    }
}