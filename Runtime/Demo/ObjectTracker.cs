using UCGUI.Services;
using UnityEngine;

namespace UCGUI.Demo
{
    public class ObjectTracker : ImageComponent
    {
        [SerializeField] private GameObject objectToTrack;
        [SerializeField] private GameObject player;
        private Renderer _renderer;

        public override void Awake()
        {
            base.Awake();

            Sprite(ImageService.GetSpriteFromAsset("player", "Tracker"));
            _renderer = objectToTrack.GetComponent<Renderer>();
        }

        private void Update()
        {
            GetImage().enabled = _renderer.isVisible;
            this.Pos(objectToTrack.GetCanvasPos(GUIService.GetMainCamera(), GUIService.GetCanvas()));
            var scale = 10f / (player.transform.position - objectToTrack.transform.position).magnitude;
            this.LocalScale(new Vector3(scale, scale, scale));
        }
    }
}