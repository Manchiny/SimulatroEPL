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
        [SerializeField]
        private float power;

        public string Id => title;
        public string Title => title;
        public Sprite Icon => icon;
        public float Power => power;

        public int Points { get; set; }
        public int TempPoints { get; set; }
    }
}
