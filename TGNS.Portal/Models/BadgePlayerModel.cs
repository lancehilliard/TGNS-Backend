using System;

namespace TGNS.Portal.Models
{
        public interface IBadgePlayerModel
        {
            DateTime Created { get; }
            string Description { get; }
            int ID { get; }
            string LevelName { get; }
            string Name { get; }
            string DisplayName { get; }
            bool ShowInGame { get; }
        }

        public class BadgePlayerModel : IBadgePlayerModel
        {
            public int ID { get; private set; }
            public string LevelName { get; private set; }
            public string Name { get; private set; }
            public string DisplayName { get; private set; }
            public string Description { get; private set; }
            public DateTime Created { get; private set; }
            public bool ShowInGame { get; private set; }
            public BadgePlayerModel(int id, string levelName, string name, string displayName, string description, DateTime created, bool showInGame)
            {
                ID = id;
                LevelName = levelName;
                Name = name;
                DisplayName = displayName;
                Description = description;
                Created = created;
                ShowInGame = showInGame;
            }
        }     
}