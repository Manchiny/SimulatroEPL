using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimulatorEPL
{
    [CreateAssetMenu]
    public class TeamsDb : ScriptableObject
    {
        [SerializeField]
        private List<Team> teams;

        public IReadOnlyList<Team> Teams => teams;

        public Team GetById(string id)
        {
            return teams.FirstOrDefault(team => id == team.Id);
        }
    }
}
