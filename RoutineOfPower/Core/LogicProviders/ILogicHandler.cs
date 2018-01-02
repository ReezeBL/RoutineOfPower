using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loki.Bot;
using Loki.Game.Objects;

namespace RoutineOfPower.Core.LogicProviders
{
    public interface ILogicHandler : IMessageHandler
    {
        bool Enabled { get; set; }
        string Name { get; set; }

        int Priority { get; set; }

        UserControl InterfaceControl { get; }
        void CreateInterfaceControl();

        void Start();

        Task<LogicResult> OutCombatHandling();
        Task<LogicResult> CombatHandling(IList<Monster> targets);
    }
}