using TGNS.Portal.Models;

namespace TGNS.Portal.Classes
{
    public interface IBadgePlayerAdapter
    {
        BadgePlayerViewModel Adapt(IBadgePlayerModel badgePlayerModel);
    }

    public class BadgePlayerAdapter : IBadgePlayerAdapter
    {
        public BadgePlayerViewModel Adapt(IBadgePlayerModel badgePlayerModel)
        {
            var result = new BadgePlayerViewModel(badgePlayerModel.ID, badgePlayerModel.LevelName, badgePlayerModel.Name, badgePlayerModel.DisplayName, badgePlayerModel.Description, badgePlayerModel.Created, badgePlayerModel.ShowInGame);
            return result;
        }
    }
}