using Exiled.API.Features;
using Exiled.API.Features.Toys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace EventManagerRework.Features.Component
{
    public class LightComponent : MonoBehaviour
    {
        private Player plr;
        private Light light;

        public void Start()
        {
            plr = Player.Get(gameObject);
            light = Light.Create(plr.Position, spawn: false);
            light.Spawn();
        }
        public void Update()
        {
            light.Color = plr.Role.Color;
            light.Position = plr.Position;
        }
    }
}
