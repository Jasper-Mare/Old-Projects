using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace directXMonoGame {
    public class Game1 : Game {
        private GraphicsDeviceManager _graphics;
        Vector3 camTarget;
        Vector3 camPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        BasicEffect basicEffect;

        //geomtric info
        VertexPositionColor[] triangleVertices;
        VertexBuffer vertexBuffer;

        //orbit
        bool orbit;


        public Game1() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            //setup camera
            camTarget = new Vector3(0,0,0);
            camPosition = new Vector3(0, 0, -100);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1, 1000);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

            //basic effect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1f;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;

            //create our triangle
            triangleVertices = new VertexPositionColor[3];
            triangleVertices[0] = new VertexPositionColor(new Vector3(  0,  20, 0), Color.Red);
            triangleVertices[1] = new VertexPositionColor(new Vector3(-20, -20, 0), Color.Green);
            triangleVertices[2] = new VertexPositionColor(new Vector3( 20, -20, 0), Color.Blue);

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(triangleVertices);

            base.Initialize();
        }

        protected override void LoadContent() {
            
        }

        //<game loop>
        protected override void Update(GameTime gameTime) {
            if (IsActive) { // pause game if lost focus
                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }

                if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                    camPosition.X += 1;
                    camTarget.X   += 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                    camPosition.X -= 1;
                    camTarget.X   -= 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                    camPosition.Y -= 1;
                    camTarget.Y   -= 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                    camPosition.Y += 1;
                    camTarget.Y   += 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemPlus)) {
                    camPosition.Z += 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) {
                    camPosition.Z -= 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) {
                    orbit = !orbit;
                }
                if (orbit) {
                    Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1));
                    camPosition = Vector3.Transform(camPosition, rotationMatrix);
                }
                viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime) {
            basicEffect.Projection = projectionMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            //turn off backface culling
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);
            }

            base.Draw(gameTime);
        }
        //</game loop>

        protected override void OnActivated(object sender, EventArgs args) {
            Window.Title = "Active";
            base.OnActivated(sender, args);
        }

        protected override void OnDeactivated(object sender, EventArgs args) {
            Window.Title = "Inactive";
            base.OnDeactivated(sender, args);
        }
    }
}