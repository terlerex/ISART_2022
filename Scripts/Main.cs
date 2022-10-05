using System.Collections.Generic;
using Godot;

namespace Correction.Scripts
{
    public class Main : Node2D
    {
        //Player
        private KinematicBody2D player;
        private AnimatedSprite anim;

        //Animations
        private const string PATH_ANIM = "/anim";
        private const string WALK_ANIM = "Walk";
        private const string IDLE_ANIM = "Idle";

        //Nodepath
        [Export] NodePath playerPath;
        [Export] NodePath burgerPath;
        [Export] NodePath fishPath;

        //Variables
        [Export] float move_speed = 500f;
        [Export] float gravity = 90f;
        [Export] float minFallForce = 5f;
        [Export] float maxFallForce = 1000f;
        [Export] float jumpForce = 1000f;

        [Export] private float growFactor = 0.05f;
        [Export] private float shrinkFactor = 0.05f;

        //Player scale
        private float playerMaxScale = 3;
        private float playerMinScale;
    
        //Health events
        private float jumpCapacity = 1f;
        private float minJumpCapacity = 0.5f;
        private float maxJumpCapacity = 1f;
    
        private Vector2 direction;

        //Input
        const string ACTION_MOVE_RIGHT = "MoveRight";
        const string ACTION_MOVE_LEFT = "MoveLeft";
        const string ACTION_JUMP = "Jump";

        //List containing all the food
        private List<Area2D> lstBurger = new List<Area2D>();
        private List<Area2D> lstFish = new List<Area2D>();
    
        //Collision events
        const string EVENT_BODY_ENTERED = "body_entered";
    
        public override void _Ready()
        {
            player = (KinematicBody2D)GetNode(playerPath);
            anim = (AnimatedSprite) GetNode(playerPath + PATH_ANIM);
        
            playerMinScale = anim.Scale.x;
        
            Node2D lBurgerContainer = (Node2D)GetNode(burgerPath);
            Node2D lFurgerContainer = (Node2D)GetNode(fishPath);

            FromContainerToList(lBurgerContainer, lstBurger);
            FromContainerToList(lFurgerContainer, lstFish);
        
            addListener(lstBurger, nameof(OnBurgerAte), EVENT_BODY_ENTERED);
            addListener(lstFish, nameof(OnFishAte), EVENT_BODY_ENTERED);
        }

        public override void _PhysicsProcess(float delta)
        {
            PlayerMovements();
            PlayerAnimation();
        }

        void PlayerMovements()
        {
            direction.y += gravity;
        
            if (direction.y > maxFallForce) 
                direction.y = maxFallForce;
        
            if(player.IsOnFloor())
                direction.y = minFallForce;

            direction.x = Input.GetActionStrength(ACTION_MOVE_RIGHT) - Input.GetActionStrength(ACTION_MOVE_LEFT);
            if (direction.x != 0) anim.Scale = new Vector2(direction.x / 2, anim.Scale.y);
        
            direction.x *= move_speed * jumpCapacity;
        
            PlayerJump();
        
            direction = player.MoveAndSlide(direction, Vector2.Up);
        }

        void PlayerJump()
        {
            if (player.IsOnFloor() && Input.IsActionJustPressed(ACTION_JUMP))direction.y = -jumpForce * jumpCapacity;
        }

        void PlayerAnimation()
        {
            if (Input.IsActionPressed(ACTION_MOVE_LEFT) || Input.IsActionPressed(ACTION_MOVE_RIGHT))
            {
                if (anim.Animation != WALK_ANIM) anim.Animation = WALK_ANIM;
            }
            else
            {
                if (anim.Animation != IDLE_ANIM) anim.Animation = IDLE_ANIM;
            }
        }

        void FromContainerToList(Node2D pContainer, List<Area2D> pList)
        {
            foreach (Area2D item in pContainer.GetChildren())
            {
                pList.Add(item);
            }
        }

        void addListener(List<Area2D> pLst, string pMethodName, string pEventName)
        {
            foreach (Area2D item in pLst)
            {
                item.Connect(pEventName, this, pMethodName, new Godot.Collections.Array ( item ));
            }
        }

        void OnBurgerAte(Node pBody, Area2D pItem)
        {
            pItem.QueueFree();

            if(player.Scale.x < playerMaxScale)
                player.Scale += new Vector2(growFactor, growFactor);
        
            if(jumpCapacity > minJumpCapacity)
                jumpCapacity -= growFactor * jumpCapacity;
        }
    
        void OnFishAte(Node pBody, Area2D pItem)
        {
            pItem.QueueFree();

            if(player.Scale.x > playerMinScale)
                player.Scale -= new Vector2(shrinkFactor, shrinkFactor);
        
            if(jumpCapacity < maxJumpCapacity)
                jumpCapacity += shrinkFactor * jumpCapacity;
        }
    
        void OnCollectibleAte(Node pBody, Area2D pItem)
        {
            pItem.QueueFree();
        }
    
    }
}
