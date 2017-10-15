using System;
using System.ComponentModel.DataAnnotations;

namespace TGNS.Portal.Models
{
    public class BadgePlayerViewModel
    {
        public int ID { get; private set; }
        public string LevelName { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Earned")]
        public DateTime Created { get; private set; }
        public bool ShowInGame { get; private set; }
        public BadgePlayerViewModel(int id, string levelName, string name, string displayName, string description, DateTime created, bool showInGame)
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