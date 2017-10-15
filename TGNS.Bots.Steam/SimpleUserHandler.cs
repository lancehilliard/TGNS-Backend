using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using SteamBot;
using SteamKit2;
using SteamTrade;

namespace TGNS.Bots.Steam
{
    public class SimpleUserHandler : UserHandler
    {
        static readonly IList<uint> DevFriends = new List<uint> {160301, 6287842, 15924490, 3388480};

        bool IsDevFriend
        {
            get
            {
                var result = DevFriends.Contains(OtherSID.AccountID);
                return result;
            }
        }

        string PersonaName
        {
            get
            {
                var result = Bot.SteamFriends.GetFriendPersonaName(OtherSID);
                return result;
            }
        }

        public TF2Value AmountAdded;

        public SimpleUserHandler (Bot bot, SteamID sid) : base(bot, sid) {}

        public override bool OnGroupAdd()
        {
            return IsDevFriend;
        }

        public override bool OnFriendAdd()
        {
            var result = !string.Equals(ConfigurationManager.AppSettings["SteamBotUsername"], "tgns_dev") || IsDevFriend;
            return result;
        }

        public override void OnLoginCompleted()
        {
            string message = $"Online {(string.Equals(ConfigurationManager.AppSettings["SteamBotUsername"], "tgns_dev") ? "DEV" : "PROD")} ({Bot.SteamFriends.GetFriendCount()} Friends).";
            Bot.SteamFriends.SendChatMessage(new SteamID("STEAM_0:1:80150"), EChatEntryType.ChatMsg, message);
        }

        public override void OnChatRoomMessage(SteamID chatID, SteamID sender, string message)
        {
            Log.Info(Bot.SteamFriends.GetFriendPersonaName(sender) + ": " + message);
            base.OnChatRoomMessage(chatID, sender, message);
        }

        public string Greeting => $"Hello, {PersonaName} ({OtherSID.AccountID}). I'm a bot for TGNS. Thanks for playing on TGNS! I'll sometimes send you messages. You can always use the TGNS Portal's Notifications page (http://rr.tacticalgamer.com/Notifications) to configure me, and you can always unfriend me (*sniff*) to halt all notifications.";

        public override void OnFriendAdded()
        {
            base.OnFriendAdded();
            SendChatMessage(Greeting);
        }

        public override void OnFriendRemove() { }
        
        public override void OnMessage (string message, EChatEntryType type) 
        {
            SendChatMessage($"{Greeting} I'm not very creative yet with message responses, but there's always hope! :)");
        }

        //public override bool OnTradeRequest() 
        //{
        //    return true;
        //}

        //public override void OnTradeError (string error) 
        //{
        //    SendChatMessage("Oh, there was an error: {0}.", error);
        //    Log.Warn (error);
        //}

        //public override void OnTradeTimeout () 
        //{
        //    SendChatMessage("Sorry, but you were AFK and the trade was canceled.");
        //    Log.Info ("User was kicked because he was AFK.");
        //}

        //public override void OnTradeInit() 
        //{
        //    SendTradeMessage("Success. Please put up your items.");
        //}

        //public override void OnTradeAddItem (Schema.Item schemaItem, Inventory.Item inventoryItem) {}

        //public override void OnTradeRemoveItem (Schema.Item schemaItem, Inventory.Item inventoryItem) {}

        //public override void OnTradeMessage (string message) {}

        //public override void OnTradeReady (bool ready) 
        //{
        //    if (!ready)
        //    {
        //        Trade.SetReady (false);
        //    }
        //    else
        //    {
        //        if(Validate ())
        //        {
        //            Trade.SetReady (true);
        //        }
        //        SendTradeMessage("Scrap: {0}", AmountAdded.ScrapTotal);
        //    }
        //}

        //public override void OnTradeSuccess()
        //{
        //    Log.Success("Trade Complete.");
        //}

        //public override void OnTradeAwaitingConfirmation(long tradeOfferID)
        //{
        //    Log.Warn("Trade ended awaiting confirmation");
        //    SendChatMessage("Please complete the confirmation to finish the trade");
        //}

        //public override void OnTradeAccept() 
        //{
        //    if (Validate() || IsAdmin)
        //    {
        //        //Even if it is successful, AcceptTrade can fail on
        //        //trades with a lot of items so we use a try-catch
        //        try {
        //            if (Trade.AcceptTrade())
        //                Log.Success("Trade Accepted!");
        //        }
        //        catch {
        //            Log.Warn ("The trade might have failed, but we can't be sure.");
        //        }
        //    }
        //}

        //public bool Validate ()
        //{            
        //    AmountAdded = TF2Value.Zero;

        //    List<string> errors = new List<string> ();

        //    foreach (TradeUserAssets asset in Trade.OtherOfferedItems)
        //    {
        //        var item = Trade.OtherInventory.GetItem(asset.assetid);
        //        if (item.Defindex == 5000)
        //            AmountAdded += TF2Value.Scrap;
        //        else if (item.Defindex == 5001)
        //            AmountAdded += TF2Value.Reclaimed;
        //        else if (item.Defindex == 5002)
        //            AmountAdded += TF2Value.Refined;
        //        else
        //        {
        //            var schemaItem = Trade.CurrentSchema.GetItem (item.Defindex);
        //            errors.Add ("Item " + schemaItem.Name + " is not a metal.");
        //        }
        //    }

        //    if (AmountAdded == TF2Value.Zero)
        //    {
        //        errors.Add ("You must put up at least 1 scrap.");
        //    }

        //    // send the errors
        //    if (errors.Count != 0)
        //        SendTradeMessage("There were errors in your trade: ");
        //    foreach (string error in errors)
        //    {
        //        SendTradeMessage(error);
        //    }

        //    return errors.Count == 0;
        //}

    }
 
}

