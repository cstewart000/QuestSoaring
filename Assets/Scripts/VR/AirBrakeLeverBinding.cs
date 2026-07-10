using QuestSoaring.Aerodynamics;
using UnityEngine;

namespace QuestSoaring.VR
{
    public class AirBrakeLeverBinding : MonoBehaviour
    {
        [SerializeField] InteractableLever _lever;
        [SerializeField] GliderInputHub _hub;

        public void Bind(InteractableLever lever, GliderInputHub hub)
        {
            _lever = lever;
            _hub = hub;
        }

        void Update()
        {
            if (_lever == null || _hub == null) return;
            _hub.SetAirBrakes(_lever.NormalizedValue);
        }
    }
}
