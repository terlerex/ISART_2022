using Godot;

namespace Correction.Scripts
{
    public class TitleCard : Control
    {

        [Export] private NodePath pathButtonPlay;
        [Export] private NodePath pathButtonQuit;
        
        Button buttonPlay;
        Button buttonQuit;
        
        const string EVENT_BUTTON_UP = "button_up";
        const string PATH_MAIN_SCENE = "res://Scenes/Main.tscn";

        public override void _Ready()
        {
            buttonPlay = GetNode<Button>(pathButtonPlay);
            buttonQuit = GetNode<Button>(pathButtonQuit);
            
            buttonPlay.Connect(EVENT_BUTTON_UP, this, nameof(OnPlay));
            buttonQuit.Connect(EVENT_BUTTON_UP, this, nameof(OnQuit));
        }

        void OnPlay()
        {
            GetTree().ChangeScene(PATH_MAIN_SCENE);
        }

        void OnQuit()
        {
            GetTree().Quit();
        }

        protected override void Dispose(bool disposing)
        {
            buttonPlay.Disconnect(EVENT_BUTTON_UP, this, nameof(OnPlay));
            buttonQuit.Disconnect(EVENT_BUTTON_UP, this, nameof(OnQuit));
        }
    }
}
