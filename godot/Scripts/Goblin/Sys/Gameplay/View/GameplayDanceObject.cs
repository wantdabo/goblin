using Godot;

namespace Goblin.Sys.Gameplay.View
{
    public class GameplayDanceObject
    {
        public Control node { get; private set; }
        private RichTextLabel desc;

        public GameplayDanceObject(Control node)
        {
            this.node = node;
            desc = node.FindChild("Desc", true, false) as RichTextLabel;
        }

        public void Settings(int value)
        {
            if (desc != null) desc.Text = value.ToString();
        }

        public void Play()
        {
            var x = (float)GD.RandRange(-50.0, 50.0);
            var quick = GD.RandRange(0, 2) == 2;
            var duration = quick ? 0.15f : 0.25f;
            var fromPos = node.Position + new Vector2(x, (float)GD.RandRange(25, 50));
            var toPos = node.Position + new Vector2(x, (float)GD.RandRange(50, 150));
            var toScale = Vector2.One * (quick ? 1.3f : 1f);

            node.Position = fromPos;
            var tween = node.CreateTween();
            tween.SetParallel(true);
            tween.TweenProperty(node, "position", toPos, duration);
            tween.TweenProperty(node, "scale", toScale, duration);
        }
    }
}
