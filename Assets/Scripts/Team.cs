using System;
using UnityEngine;

namespace SimulatorEPL
{
    [Serializable]
    public class Team
    {
        [SerializeField]
        private string title;
        [SerializeField]
        private Sprite icon;

        public string Id => title;
        public string Title => title;
        public Sprite Icon => icon;
    }
}
