using Microsoft.Xna.Framework;
using Phantoms.Entities.Sprites;
using Phantoms.Interfaces;
using System;
using static Phantoms.Global;

namespace Phantoms.Manipulators
{
    public class Spin
    {
        private float angleAmount;
        private IVisual spinningComponent;
        private event EventHandler onCicleCompleted;

        public float RotationAngle { get; private set; }
        public bool IsSpinning { get; private set; }
        public HorizontalDirection Direction { get; private set; }

        public Spin(IVisual spinningComponent, float angleAmount, HorizontalDirection direction, bool autoSpin = true, EventHandler onCicleCompleted = null)
        {
            angleAmount = direction == HorizontalDirection.Right ? Math.Abs(angleAmount) : -Math.Abs(angleAmount);
            Initialize(spinningComponent, angleAmount, direction, autoSpin, onCicleCompleted);
        }

        public Spin(IVisual spinningComponent, float angleAmount, bool autoSpin = true, EventHandler onCicleCompleted = null)
        {
            Initialize(spinningComponent, angleAmount, angleAmount < 0 ? HorizontalDirection.Left : HorizontalDirection.Right, autoSpin, onCicleCompleted);
        }

        private void Initialize(IVisual spinningComponent, float angleAmount, HorizontalDirection direction, bool autoSpin = true, EventHandler onCicleCompleted = null)
        {
            this.spinningComponent = spinningComponent;
            this.angleAmount = angleAmount;
            Direction = direction;
            IsSpinning = autoSpin;
            this.onCicleCompleted = onCicleCompleted;
        }

        public void Start()
        {
            IsSpinning = true;
        }

        public void Stop()
        {
            IsSpinning = false;
        }

        public void ToggleDirection()
        {
            angleAmount *= -1;
            Direction = Direction == HorizontalDirection.Left ? HorizontalDirection.Right : HorizontalDirection.Left;
            RotationAngle = Direction == HorizontalDirection.Right ? RotationAngle + 360 : RotationAngle - 360;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsSpinning)
                return;

            RotationAngle += angleAmount;

            if ((RotationAngle > 360 && Direction == HorizontalDirection.Right) || RotationAngle < -360 && Direction == HorizontalDirection.Left)
            {
                RotationAngle = 0;
                onCicleCompleted?.Invoke(this, EventArgs.Empty);
            }

            spinningComponent.Rotation = (float)(Math.PI * RotationAngle / 180.0);
        }
    }
}
