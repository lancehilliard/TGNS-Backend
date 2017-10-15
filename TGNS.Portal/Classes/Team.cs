using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Portal.Classes
{
    public interface ITeam
    {
        int Number { get; }
        string Name { get; }
    }

    public class Team : ITeam
    {
        public Team(int number)
        {
            Number = number;
        }

        public int Number { get; private set; }

        public string Name
        {
            get
            {
                string result;
                switch (Number)
                {
                    case 0:
                        result = "Ready Room";
                        break;
                    case 1:
                        result = "Marines";
                        break;
                    case 2:
                        result = "Aliens";
                        break;
                    case 3:
                        result = "Spectator";
                        break;
                    default:
                        result = "Unknown";
                        break;
                }
                return result;
            }
        }
    }
}