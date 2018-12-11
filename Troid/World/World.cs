using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troid.Entities;

namespace Troid.World
{
    public class World
    {
        public Room CurrentRoom;

        private List<Room> rooms;

        public World()
        {
            rooms = new List<Room>();
        }

        public void AddRoom(Room room)
        {
            rooms.Add(room);

            if (CurrentRoom == null)
            {
                CurrentRoom = room;
            }
        }

        public void LoadRoom(int index)
        {
            Player player = CurrentRoom.GetPlayer();
            CurrentRoom.RemoveEntity(player);
            CurrentRoom = rooms[index];
            CurrentRoom.AddEntity(player);
        }

        public void Update(GameTime gameTime)
        {
            CurrentRoom.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentRoom.Draw(spriteBatch);
        }
    }
}
