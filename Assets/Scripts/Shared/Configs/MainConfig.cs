using UnityEngine;
namespace Borgs
{
    [CreateAssetMenu(fileName = "WorldSettingsConfig", menuName = "Configurations/WorldSettingsConfig")]

    public class MainConfig : ScriptableObject
    {
        public bool IsDirty { get; set; }
        public BoundaryType AgentBoundaryType = BoundaryType.PlayPen;
        public BoundaryType MoveableObjectBoundaryType = BoundaryType.PlayPen;
        private void OnValidate()
        {
            IsDirty = true;
        }
    }
}