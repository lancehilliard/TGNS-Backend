using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TGNS.Core.Data;

namespace TGNS.Portal.Classes
{
    public interface IServerAdminJsonCreator
    {
        string Create();
    }

    public class ServerAdminJsonCreator : IServerAdminJsonCreator
    {
        public static void Main()
        {
            var serverAdminJsonCreator = new ServerAdminJsonCreator();
            var serverAdminJson = serverAdminJsonCreator.Create();
        }

        public string Create()
        {
            var primerSignersGetter = new PrimerSignersGetter();
            var cronUsersGetter = new CronUsersGetter();
            var fullAdminGroup = new Dictionary<string, object>
            {
                {"type", "disallowed"},
                {"commands", new List<string> {"sv_afkimmune", "sv_ejectionprotection", "sv_randomall", "sh_adminmenu"}}
            };
            var tempAdminGroup = new Dictionary<string, object>
            {
                {"type", "allowed"},
                {"commands", new List<string> {"sv_istempadmin", "sv_kick", "sv_maps", "sv_changemap", "changemap", "sv_balance", "sv_switchteam", "sv_setcaptain", "captains", "sv_mutes", "sv_chat", "PMADMIN", "cyclemap", "gb"}}
            };
            var guardianGroup = new Dictionary<string, object>
            {
                {"type", "allowed"},
                {"commands", new List<string> {"sv_isguardian", "gb"}}
            };
            var supportingMemberGroup = new Dictionary<string, object>
            {
                {"type", "allowed"},
                {"commands", new List<string> {"sv_hasreserve", "sv_hassupportingmembership", "sv_taglineannounce", "sv_smchat", "SMCHAT", "sv_chat", "sv_playcodes"}}
            };
            var primerGroup = new Dictionary<string, object>
            {
                {"type", "allowed"},
                {"commands", new List<string> {"sv_hasprimersignature", "sv_chat", "sv_playcodes"}}
            };
            var spectatorGroup = new Dictionary<string, object>
            {
                {"type", "allowed"},
                {"commands", new List<string> {"sv_afkimmune"}}
            };
            var groups = new Dictionary<string, object>
            {
                {"fulladmin_group", fullAdminGroup},
                {"tempadmin_group", tempAdminGroup},
                {"guardian_group", guardianGroup},
                {"sm_group", supportingMemberGroup},
                {"primer_group", primerGroup},
                {"spectator_group", spectatorGroup}
            };
            var primerSignerUsers = primerSignersGetter.Get().ToList();
            var supportingMemberUsers = cronUsersGetter.Get("URL_HERE").ToList();
            var adminUsers = cronUsersGetter.Get("URL_HERE").ToList();
            if (DateTime.Now < new DateTime(2016, 11, 25)) // one month
            {
                primerSignerUsers.Add(new Player("Firewolf34-UnableToPostInPrimerThread", 77404117));
            }
            if (DateTime.Now < new DateTime(2016, 12, 01)) // one month
            {
                primerSignerUsers.Add(new Player("Twiglingen-UnableToPostInPrimerThread", 70487954));
            }
            if (DateTime.Now < new DateTime(2017, 1, 4)) // one month
            {
                primerSignerUsers.Add(new Player("Sasa-UnableToPostInPrimerThread", 21563812));
            }
            if (DateTime.Now < new DateTime(2017, 4, 29)) // one month
            {
                supportingMemberUsers.Add(new Player("smiley-SupportingMembershipIsBillingHimButNotAppearingInForums", 37187440));
            }
            var nomPlayer = new Player("NoM-ServerProviderAndMaintainer", 19849485);
            supportingMemberUsers.Add(nomPlayer);
            Func<string, string, IEnumerable<IPlayer>, Dictionary < string, Dictionary < string, object>>> dictionaryCreator = (groupNameSuffix, groupName, groupMembers) => groupMembers.ToDictionary(x => $"{x.Name}-{x.PlayerId}-{groupNameSuffix}", x => new Dictionary<string, object> { { "id", x.PlayerId }, { "groups", new List<string> { groupName } } });
            var primerSignerUsersDictionary = dictionaryCreator("p", "primer_group", primerSignerUsers);
            var supportingMemberUsersDictionary = dictionaryCreator("S", "sm_group", supportingMemberUsers);
            var adminUsersDictionary = dictionaryCreator("A", "fulladmin_group", adminUsers);
            var gameServerRentalProvidersUsersDictionary = dictionaryCreator("gsrp", "game_server_rental_provider_group", new List<Player> { nomPlayer });
            var users = new List<Dictionary<string, Dictionary<string, object>>> { primerSignerUsersDictionary, supportingMemberUsersDictionary, adminUsersDictionary, gameServerRentalProvidersUsersDictionary }.SelectMany(x=>x).ToDictionary(x=>x.Key, x=>x.Value);
            var serverAdmin = new Dictionary<string, object> {{"groups", groups}, {"users", users}};
            var result = JsonConvert.SerializeObject(serverAdmin, Formatting.Indented);
            return result;
        }
    }
}