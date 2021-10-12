using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoca.UI
{
    public class SpriteAnimator : MonoBehaviour
    {
        enum State { Stopped, Playing, Paused }

        [SerializeField]
        Image spriteImage;

        [SerializeField]
        string resourceFolder; // Put the sprites here

        [SerializeField]
        float speed = 1; // Default is 60 fps

        [SerializeField]
        bool loop = false;

        [SerializeField]
        bool pingPong = false;

        [SerializeField]
        bool reverse = false;

        [SerializeField]
        bool autoStart = false;

        [SerializeField]
        bool clearOnStop = false;

        [SerializeField]
        bool nativeSize = false;

        List<Sprite> sprites;
        float internalTime = 0.016f; // 60 fps
        //bool running = false;
        State state = State.Stopped;
        DateTime lastFrameTime;
        int currentSpriteIndex = 0;
        int loopCount = 0;
        int dir = 1;
        
        private void Awake()
        {
            // Load sprites from resources
            Sprite[] spriteArray = Resources.LoadAll<Sprite>(System.IO.Path.Combine(ResourceFolder.Sprites, resourceFolder));

            sprites = new List<Sprite>(spriteArray);

            

            if (autoStart)
            {
                Play();
            }
                
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (state != State.Playing)
                return;

            // Check for next frame
            float time = internalTime / speed;
            if((DateTime.UtcNow-lastFrameTime).TotalSeconds > time)
            {
                MoveToNextFrame();
            }
        }

        public void Play()
        {
            
            if (state == State.Playing)
                return; // Alread playing

            // Save the old state
            State oldState = state;
            // Set the new state
            state = State.Playing;
            
            
            if(oldState == State.Stopped)
            {
                // If it was stopped we must set the first sprite and adjust image
                // Get the first frame
                if (!reverse)
                {
                    dir = 1;
                    currentSpriteIndex = 0;
                }
                else
                {
                    dir = -1;
                    currentSpriteIndex = sprites.Count - 1;
                }
              
                // Show the frame
                spriteImage.sprite = sprites[currentSpriteIndex];

                // Adjust size if needed
                if (nativeSize)
                    spriteImage.SetNativeSize();

                // Set time
                lastFrameTime = DateTime.UtcNow;

                // Init loop count
                loopCount = 0;
            }
            
            
        }

        public void Pause()
        {
            if (state == State.Playing)
                state = State.Paused;
        }

        public void Stop()
        {
            if (state == State.Stopped)
                return;

            state = State.Stopped;

            if (clearOnStop)
                spriteImage.sprite = null;
        }

        #region private
        void MoveToNextFrame()
        {

            // Adjust the index
            int nextIndex = currentSpriteIndex + dir;
            //currentSpriteIndex += reverse ? -1 : 1;
            // Clamp
            if (nextIndex == sprites.Count || nextIndex == -1)
            {
                // Loop completed
                loopCount++;

                // Check for loop
                if ((!loop && !pingPong && loopCount > 0) || (!loop && pingPong && loopCount > 1))
                {
                    state = State.Stopped;
                    return;
                }

                // Reverse
                if (pingPong)
                    dir *= -1;

                // Continue
                if (nextIndex == sprites.Count)
                {
                    if (dir > 0)
                        nextIndex = 0;
                    else
                        nextIndex = sprites.Count - 2;
                }
                else
                {
                    if (dir < 0)
                        nextIndex = sprites.Count - 1;
                    else
                        nextIndex = 1;
                }
                    
                
            }

            // Update the index
            currentSpriteIndex = nextIndex;

            // Show the new frame
            spriteImage.sprite = sprites[currentSpriteIndex];

            // Update time
            lastFrameTime = DateTime.UtcNow;
        }
        #endregion
    }

}
