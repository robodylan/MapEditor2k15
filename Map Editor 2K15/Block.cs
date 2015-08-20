using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Map_Editor_2K15
{
    class Block
    {
        private int x;
        private int y;
        private int ID;
        private bool hasPhysics;

        public Block(int x, int y, int ID, bool hasPhysics)
        {
            this.x = x;
            this.y = y;
            this.ID = ID;
            this.hasPhysics = hasPhysics;
        }

        public Vector2 getPosition()
        {
            return new Vector2(this.x,this.y);
        }

        public int getID()
        {
            return ID;
        }
    }
}
