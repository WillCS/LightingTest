using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingTest {
    class Map {
        private bool[,] _tiles;
        private int _width;
        private int _height;

        public int Width {
            get {
                return this._width;
            }
        }

        public int Height {
            get {
                return this._height;
            }
        }

        public Map(int width, int height) {
            this._tiles = new bool[width, height];
            this._width = width;
            this._height = height;
        }

        public bool this[int x, int y] {
            get {
                if(x < 0 || x >= this.Width || y < 0 || y >= this.Height) {
                    return false;
                }

                return this._tiles[x, y];
            }
            set {
                if(x < 0 || x >= this.Width || y < 0 || y >= this.Height) {
                    return;
                }

                this._tiles[x, y] = value;
            }
        }

        public static Map GetTestMap() {
            Map map = new Map(30, 30);
            Random rand = new Random();
            for(int x = 0; x < 30; x++) {
                for(int y = 0; y < 30; y++) {
                    map[x, y] = rand.Next(0, 10) == 1 ? true : false;
                }
            }
            return map;
        }
    }
}
